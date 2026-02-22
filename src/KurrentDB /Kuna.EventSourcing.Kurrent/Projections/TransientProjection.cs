
using static Kuna.EventSourcing.Core.Projections.StateMutator;

namespace Kuna.EventSourcing.Kurrent.Projections
{
    /// <summary>
    /// Used to project events to arbitrary transient read model.
    /// Most likely usage is to quickly create read models on the fly without need to create separate persistent projection.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="streamReader"></param>
    public class TransientProjection<TState>(IStreamReader streamReader)
        where TState : new()
    {
        private readonly IStreamReader reader = streamReader;

        /// <summary>
        /// Reads events from stream and applies them to state
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<TState> GetFor(string streamId, CancellationToken ct)
        {
            // TODO: this is basically same as what aggregate repository does, loads events and applies them to state.
            // can it be unified?
            var toReturn = new TState();

            var events = await this.reader.GetEvents(streamId, ct);

            ulong? version = null;

            Mutate(toReturn, ref version, events);

            return toReturn;
        }
    }
}
