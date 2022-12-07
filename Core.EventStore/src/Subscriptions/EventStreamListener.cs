using System.Reflection;
using EventStore.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Core.EventStore.Subscriptions;

public record StreamSubscriptionSettings(string streamName);

public class StreamListener
{
    private readonly EventStorePersistentSubscriptionsClient client;
    private readonly IEventBus eventBus;
    private readonly IEventSerializer eventSerializer;
    private readonly Func<ResolvedEvent, Task> eventHandler;
    private readonly ILogger logger;
    private StreamSubscriptionSettings streamSettings = default!;
    private PersistentSubscriptionSettings subscriptionSettings = default!;
    private CancellationToken cancellationToken;

    private string SubscriptionGroupName { get; }

    public StreamListener(
        EventStorePersistentSubscriptionsClient client,
        IEventBus eventBus,
        IEventSerializer eventSerializer,
        Func<ResolvedEvent, Task> eventHandler,
        ILogger logger)
    {
        this.client = client;
        this.eventBus = eventBus;
        this.eventSerializer = eventSerializer;
        this.eventHandler = eventHandler;
        this.logger = logger;

        // each microservice will be a separate subscription group
        this.SubscriptionGroupName = Assembly.GetEntryAssembly()!.GetName()!.Name!;
    }

    public async Task Start(
        StreamSubscriptionSettings streamSettings,
        PersistentSubscriptionSettings subscriptionSettings,
        CancellationToken ct)
    {
        this.cancellationToken = ct;

        this.subscriptionSettings = subscriptionSettings;
        this.streamSettings = streamSettings;
        await this.EnsureSubscriptionGroupExists(streamSettings, subscriptionSettings, ct);

        await this.StartSubscription(ct);
    }

    private async Task EnsureSubscriptionGroupExists(
        StreamSubscriptionSettings streamSubscriptionSettings,
        PersistentSubscriptionSettings settings,
        CancellationToken ct)
    {
        try
        {
            await this.client.CreateAsync(
                streamSubscriptionSettings.streamName,
                this.SubscriptionGroupName,
                settings,
                cancellationToken: ct);
        }
        catch (Exception e)
        {
            this.logger.LogError("Could not create subscription group", e);
            throw;
        }

        this.logger.LogTrace(
            "Persistent Subsription group {SubscrptionGrou} created",
            this.SubscriptionGroupName);
    }

    private async Task StartSubscription(CancellationToken ct)
    {
        await this.client.SubscribeToStreamAsync(
            this.streamSettings.streamName,
            this.SubscriptionGroupName,
            this.OnEventAppeared,
            this.OnSubscriptionDropped,
            cancellationToken: ct);

        this.logger.LogTrace(
            "Persistent subscription to stream {StreamName} created",
            this.streamSettings.streamName);
    }

    private async Task OnEventAppeared(
        PersistentSubscription subscription,
        ResolvedEvent resolvedEvent,
        int? retryCount,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            var @event = this.eventSerializer.Deserialize(resolvedEvent);

            if (@event == null)
            {
                await subscription.Ack(resolvedEvent);
                return;
            }

            await this.eventBus.Publish(@event, ct);

            await subscription.Ack(resolvedEvent);
        }
        catch (InvalidOperationException ex)
        {
            await subscription.Nack(PersistentSubscriptionNakEventAction.Park, ex.Message, resolvedEvent);
        }
        catch (UnrecoverableException ex)
        {
            await subscription.Nack(PersistentSubscriptionNakEventAction.Park, ex.Message, resolvedEvent);
        }
        catch (Exception ex)
        {
            if (retryCount == this.subscriptionSettings.MaxRetryCount)
            {
                await subscription.Nack(PersistentSubscriptionNakEventAction.Park, ex.Message, resolvedEvent);
            }
            else
            {
                await subscription.Nack(PersistentSubscriptionNakEventAction.Retry, ex.Message, resolvedEvent);
            }
        }
    }

    private void OnSubscriptionDropped(
        PersistentSubscription subscription,
        SubscriptionDroppedReason dropReason,
        Exception? exception)
    {
        this.logger.LogWarning(
            "Subscription dropped. Reason: {DropReason}, Exception: {@Exception}",
            dropReason,
            exception
        );

        if (exception is RpcException { StatusCode: StatusCode.Cancelled })
        {
            return;
        }

        this.Resubscribe();
    }


    private void Resubscribe()
    {
        throw new NotImplementedException("Finish resubscribe");
    }
}
