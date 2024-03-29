using EventStore.Client;

namespace Kuna.EventSourcing.EventStore;

public interface IStreamReader
{
    /// <summary>
    /// Reads stream of aggregate events and deserializes them to instances of the mapped event type
    /// if stream does not exist, returns empty enumerable
    /// </summary>
    /// <param name="streamId">Id of the stream, such as {streamPrefix}-{aggregateId}, order-57e1e0fc8cb1407694a752ac014ba27a</param>
    /// <param name="ct"></param>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<object>> GetEvents(string streamId, CancellationToken ct);
}

public sealed class StreamReader
    : IStreamReader
{
    private readonly EventStoreClient client;
    private readonly IEventStoreSerializer eventStoreSerializer;

    public StreamReader(
        EventStoreClient client,
        IEventStoreSerializer eventStoreSerializer)
    {
        this.client = client;
        this.eventStoreSerializer = eventStoreSerializer;
    }

    /// <summary>
    /// Reads stream of aggregate events and deserializes them to instances of the mapped event type
    /// if stream does not exist, returns empty enumerable
    /// </summary>
    /// <param name="streamId">Id of the stream, such as {streamPrefix}-{aggregateId}, order-57e1e0fc8cb1407694a752ac014ba27a</param>
    /// <param name="ct"></param>
    /// <exception cref="ArgumentException"></exception>
    public async Task<IEnumerable<object>> GetEvents(string streamId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var events = this.client
                         .ReadStreamAsync(
                             Direction.Forwards,
                             streamId,
                             StreamPosition.Start,
                             cancellationToken: ct);

        if (events.ReadState.Result == ReadState.StreamNotFound)
        {
            return Enumerable.Empty<object>();
        }

        var toReturn = new List<object>();

        await foreach (var resolvedEvent in events)
        {
            var @event = this.eventStoreSerializer.DeserializeData(resolvedEvent);

            if (@event == null)
            {
                throw new ArgumentException($"Unable to find suitable type for event name {resolvedEvent.Event.EventType}.");
            }

            toReturn.Add(@event);
        }

        return toReturn;
    }
}