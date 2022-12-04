using System.Diagnostics;
using System.Reflection;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore.Subscriptions;

public class EventStreamListener
{
    private readonly IEventBus bus;
    private readonly EventStoreClient client;
    private readonly IEventSerializer serializer;
    private readonly ILogger logger;

    public EventStreamListener(IEventBus bus)
    {
        this.bus = bus;
    }

    public string Group { get; }

    public EventStreamListener(
        EventStoreClient client,
        IEventSerializer serializer,
        ILogger<EventStreamListener> logger)
    {
        this.client = client;
        this.serializer = serializer;
        this.logger = logger;

        this.Group = Assembly.GetEntryAssembly().GetName().Name;
    }

    public async Task SubscribeToAllStream(
        SubscriptionFilterOptions subscriptionFilterOptions,
        CancellationToken ct)
    {
        await this.StartSubscription(subscriptionFilterOptions, ct);
    }

    private async Task StartSubscription(
        SubscriptionFilterOptions subscriptionFilterOptions,
        CancellationToken ct)
    {
        await client.SubscribeToAllAsync(
            FromAll.End,
            this.OnEventAppeared,
            false,
            this.OnSubscriptionDropped,
            subscriptionFilterOptions,
            cancellationToken:ct);
    }

    private void OnSubscriptionDropped(
        StreamSubscription subscription,
        SubscriptionDroppedReason dropReason,
        Exception? exception)
    {
        throw new NotImplementedException();
    }

    private Task OnEventAppeared(
        StreamSubscription subscription,
        ResolvedEvent resolvedEvent,
        CancellationToken ct)
    {
        IEvent @event;

        // deserialize event
        @event = this.serializer.Deserialize(resolvedEvent);

        this.bus.Publish(@event, ct);
    }

}
