using Kuna.EventSourcing.Core;
using static Kuna.EventSourcing.Core.Projections.StateMutator;

namespace Kuna.EventSourcing.Kurrent
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

        public async Task SaveEvents(string streamId, object[] events, ulong? expectedVersion, CancellationToken ct)
        {
            await writer.Write(streamId, expectedVersion, events, ct);
        }

        public async Task<TState> Project<TState>(string streamId, CancellationToken ct)
            where TState : new()
        {
            ulong? version = null;
            var events = await this.LoadEvents(streamId, ct);

            var state = new TState();

            var result = this.Project(state, ref version, events);

            return result;
        }

        public TState Project<TState>(TState state, ref ulong? version, object[] events)
        {
            ArgumentNullException.ThrowIfNull(state);

            Mutate(state, ref version, events);

            return state;
        }
    }
}
