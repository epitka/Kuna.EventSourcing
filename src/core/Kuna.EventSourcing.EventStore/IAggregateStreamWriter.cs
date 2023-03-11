using EventStore.Client;
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Exceptions;

namespace Kuna.EventSourcing.EventStore;

public interface IAggregateStreamWriter
{
    Task Write(
        string streamId,
        StreamRevision expectedVersion,
        IEnumerable<IAggregateEvent> events,
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
        StreamRevision expectedVersion,
        IEnumerable<IAggregateEvent> events,
        CancellationToken ct)
    {
        var eventData = events.Select(@event => this.eventDataFactory.From(@event));

        try
        {
            await this.client
                      .AppendToStreamAsync(streamId, expectedVersion, eventData, cancellationToken: ct)
                      .ConfigureAwait(false);
        }
        catch (WrongExpectedVersionException ex)
        {
            throw new InvalidExpectedVersionException(
                $"Invalid version, stream: {streamId}, expected: {expectedVersion}, actual: {ex.ActualVersion} ",
                ex);
        }
    }
}
