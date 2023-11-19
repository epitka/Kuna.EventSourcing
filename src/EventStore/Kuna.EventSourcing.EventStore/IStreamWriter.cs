using EventStore.Client;
using Kuna.EventSourcing.Core.Aggregates;

namespace Kuna.EventSourcing.EventStore;

public interface IStreamWriter
{
    Task Write(
        string streamId,
        StreamRevision expectedVersion,
        IEnumerable<object> events,
        CancellationToken ct);
}
