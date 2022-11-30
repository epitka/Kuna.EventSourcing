using EventStore.Client;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Core.EventStore;

public interface IAggregateStreamWriter
{
    Task Write(
        string streamId,
        long expectedVersion,
        IEnumerable<Event> events,
        CancellationToken ct);
}

public class AggregateStreamWriter
    : IAggregateStreamWriter
{
    private readonly EventStoreClient client;
    private readonly IEventDataFactory eventDataFactory;

    public AggregateStreamWriter(
        EventStoreClient client,
        IEventDataFactory eventDataFactory)
    {
        this.client = client;
        this.eventDataFactory = eventDataFactory;
    }

    public async Task Write(
        string streamId,
        long expectedVersion,
        IEnumerable<Event> events,
        CancellationToken ct)
    {
        var eventData = events.Select(@event => this.eventDataFactory.From(@event));

        try
        {
            await this.client
                      .AppendToStreamAsync(
                          streamId,
                          StreamRevision.FromInt64(expectedVersion),
                          eventData,
                          cancellationToken: ct);
        }
        catch (WrongExpectedVersionException ex)
        {
            throw new InvalidExpectedVersionException(
                $"Invalid version,stream: {streamId}, expected: {expectedVersion}, actual: {ex.ActualVersion} ",
                ex);
        }
    }
}
