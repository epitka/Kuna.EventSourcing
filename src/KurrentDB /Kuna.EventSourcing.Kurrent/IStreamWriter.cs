using Kuna.EventSourcing.Core.Exceptions;
using KurrentDB.Client;

namespace Kuna.EventSourcing.Kurrent;

public interface IStreamWriter
{
    Task Write(
        string streamId,
        ulong? expectedVersion,
        IEnumerable<object> events,
        CancellationToken ct);
}

public class StreamWriter
    : IStreamWriter
{
    private readonly KurrentDBClient client;
    private readonly IEventDataFactory eventDataFactory;

    public StreamWriter(
        KurrentDBClient client,
        IEventDataFactory eventDataFactory)
    {
        this.client = client;
        this.eventDataFactory = eventDataFactory;
    }

    public async Task Write(
        string streamId,
        ulong? expectedVersion,
        IEnumerable<object> events,
        CancellationToken ct)
    {
        var eventData = events.Select(@event => this.eventDataFactory.From(@event));

        try
        {
            var streamState = expectedVersion.HasValue ? StreamState.StreamRevision(expectedVersion.Value) : StreamState.NoStream;
            await this.client
                      .AppendToStreamAsync(streamId, streamState, eventData, cancellationToken: ct)
                      .ConfigureAwait(false);
        }
        catch (WrongExpectedVersionException ex)
        {
            throw new AggregateInvalidExpectedVersionException(
                streamId,
                $"Invalid version, stream: {streamId}, expected: {expectedVersion}, actual: {ex.ActualVersion} ",
                ex);
        }
    }
}
