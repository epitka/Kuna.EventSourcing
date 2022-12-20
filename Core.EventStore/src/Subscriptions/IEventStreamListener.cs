using System.Reflection;
using EventStore.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Core.EventStore.Subscriptions;

public interface IEventStreamListener
{
    Task Start(
        StreamSubscriptionSettings subscriptionSettings,
        CancellationToken ct);

    Task Stop(TimeSpan delay);
}

public class EventStreamListener : IEventStreamListener
{
    private readonly EventStorePersistentSubscriptionsClient client;
    private readonly IEventStoreSerializer eventStoreSerializer;
    private readonly IEventDispatcher eventDispatcher;
    private readonly ILogger logger;
    private StreamSubscriptionSettings subscriptionSettings = default!;
    private bool started;
    private CancellationTokenSource cts = default!;

    private string SubscriptionGroupName { get; }

    public EventStreamListener(
        EventStorePersistentSubscriptionsClient client,
        IEventStoreSerializer eventStoreSerializer,
        IEventDispatcher eventDispatcher,
        ILogger logger)
    {
        this.client = client;
        this.eventStoreSerializer = eventStoreSerializer;
        this.eventDispatcher = eventDispatcher;
        this.logger = logger;

        // each microservice will be a separate subscription group
        this.SubscriptionGroupName = Assembly.GetEntryAssembly()!.GetName()!.Name!;
    }

    public async Task Start(
        StreamSubscriptionSettings subscriptionSettings,
        CancellationToken ct)
    {
        this.cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        if (this.started)
        {
            throw new InvalidOperationException("Cannot reuse listener, already started");
        }

        this.subscriptionSettings = subscriptionSettings;

        await this.EnsureSubscriptionGroupExists(this.subscriptionSettings, this.cts.Token)
                  .ConfigureAwait(false);

        await this.Start(this.cts.Token)
                  .ConfigureAwait(false);

        this.started = true;
    }

    private async Task EnsureSubscriptionGroupExists(
        StreamSubscriptionSettings subscriptionSettings,
        CancellationToken ct)
    {
        try
        {

            var persistentSubscriptionSettings = new PersistentSubscriptionSettings(
                resolveLinkTos: true,
                startFrom: subscriptionSettings.StartFrom,
                consumerStrategyName: subscriptionSettings.ConsumerStrategy,
                maxRetryCount: subscriptionSettings.MaxRetryCount,
                extraStatistics: subscriptionSettings.ExtraStatistics,
                messageTimeout: subscriptionSettings.MessageTimeout,
                liveBufferSize: subscriptionSettings.LiveBufferSize,
                readBatchSize: subscriptionSettings.ReadBatchSize,
                historyBufferSize: subscriptionSettings.HistoryBufferSize,
                checkPointAfter: subscriptionSettings.CheckpointAfter,
                checkPointLowerBound: subscriptionSettings.CheckPointLowerBound,
                checkPointUpperBound: subscriptionSettings.CheckPointUpperBound);

            await this.client.CreateAsync(
                          subscriptionSettings.StreamName,
                          this.SubscriptionGroupName,
                          persistentSubscriptionSettings,
                          cancellationToken: ct)
                      .ConfigureAwait(false);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.AlreadyExists)
        {
            // if subscription already exists, just skip
        }

        this.logger.LogTrace(
            "Persistent Subsription group {SubscrptionGrou} created",
            this.SubscriptionGroupName);
    }

    private async Task Start(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await this.client.SubscribeToStreamAsync(
                                          this.subscriptionSettings.StreamName,
                                          this.SubscriptionGroupName,
                                          this.OnEventAppeared,
                                          this.OnSubscriptionDropped,
                                          cancellationToken: ct)
                                      .ConfigureAwait(false);

        this.logger.LogTrace(
            "Persistent subscription to stream {StreamName} created",
            this.subscriptionSettings.StreamName);
    }

    public async Task Stop(TimeSpan delay)
    {
        await Task.Delay(delay)
                  .ConfigureAwait(false);;
        this.cts.Cancel();
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
           var result = this.eventStoreSerializer.Deserialize(resolvedEvent);
            // TODO: Add OpenTelemetry tracing here
            // Propagate CorrelationId and set CausationId using EventId
            // Use OpenTelemetry api instead of Activity
            //var metaData = this.eventSerializer.DeserializeMetaData(resolvedEvent);

            if (result.Event == null)
            {
                await subscription.Ack(resolvedEvent)
                                  .ConfigureAwait(false);

                return;
            }

            await this.eventDispatcher.Publish(result.Event, result.EventType, ct)
                      .ConfigureAwait(false);

            await subscription.Ack(resolvedEvent)
                              .ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            await subscription.Nack(PersistentSubscriptionNakEventAction.Park, ex.Message, resolvedEvent)
                              .ConfigureAwait(false);
        }
        catch (UnrecoverableException ex)
        {
            await subscription.Nack(PersistentSubscriptionNakEventAction.Park, ex.Message, resolvedEvent)
                              .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (retryCount == this.subscriptionSettings.MaxRetryCount)
            {
                await subscription.Nack(PersistentSubscriptionNakEventAction.Park, ex.Message, resolvedEvent)
                                  .ConfigureAwait(false);
            }
            else
            {
                await subscription.Nack(PersistentSubscriptionNakEventAction.Retry, ex.Message, resolvedEvent)
                                  .ConfigureAwait(false);
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

        if (exception is RpcException { StatusCode: StatusCode.Cancelled or StatusCode.Unavailable })
        {
            return;
        }

        if (dropReason == SubscriptionDroppedReason.Disposed)
        {
            return;
        }

        this.Resubscribe();
    }

    private void Resubscribe()
    {
        if (this.cts.IsCancellationRequested)
        {
            return;
        }

        throw new NotImplementedException("Finish resubscribe");
    }
}
