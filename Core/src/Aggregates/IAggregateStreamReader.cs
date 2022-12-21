namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregateStreamReader
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