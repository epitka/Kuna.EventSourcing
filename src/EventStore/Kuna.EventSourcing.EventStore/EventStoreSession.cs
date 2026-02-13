using EventStore.Client;
using Kuna.EventSourcing.Core;
using static Kuna.EventSourcing.Core.Projections.StateMutator;

namespace Kuna.EventSourcing.EventStore
{
    /// <inheritdoc/>
    public class EventStoreSession(
        IStreamReader reader,
        IStreamWriter writer) : ISession
    {
        public async Task<object[]> LoadEvents(string streamId, CancellationToken ct)
        {
            var events = await reader.GetEvents(streamId, ct);
            return events.ToArray();
        }

        public async Task SaveEvents(string streamId, object[] events, int expectedVersion, CancellationToken ct)
        {
            await writer.Write(streamId, StreamRevision.FromInt64(expectedVersion), events, ct);
        }

        public async Task<TState> Project<TState>(string streamId, CancellationToken ct)
            where TState : new()
        {
            var version = -1;
            var events = await this.LoadEvents(streamId, ct);

            var state = new TState();

            var result = Project(state, ref version, events);

            return result;
        }

        public TState Project<TState>(TState state, ref int version, object[] events)
        {
            ArgumentNullException.ThrowIfNull(state, nameof(state));

            Mutate(state, ref version, events);

            return state;
        }
    }
}
