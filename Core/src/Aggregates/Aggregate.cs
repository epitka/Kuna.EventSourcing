using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Extensions;

namespace Senf.EventSourcing.Core.Aggregates
{
    public abstract class Aggregate<TState>
        : IAggregate<TState>
        where TState : AggregateState, new()
    {
        private readonly List<Event> pendingEvents = new();

        public long Version => this.MyState.Version;

        public Guid Id => this.MyState.Id;

        protected TState MyState { get; private set; } = default!;

        protected Aggregate()
        {
        }

        void IAggregate<TState>.InitWithState(TState state)
        {
            _ = state ?? throw new InvalidOperationException("Cannot initialize aggreate with null state.");

            if (this.MyState != null)
            {
                throw new InvalidOperationException("State is already initialized");
            }

            this.MyState = state;
            this.MyState.SetId(state.Id);
        }

        public void InitWith(IEnumerable<Event> events)
        {
            _ = events ?? throw new InvalidOperationException("Cannot initialize aggreate with null events.");

            if (this.Version > -1)
            {
                throw new InvalidOperationException("State is already initialized");
            }

            this.EnsureStateInitialized();

            foreach (var @event in events)
            {
                this.MyState.ApplyEvent(@event);
            }
        }

        /// <summary>
        /// Returns deep clone of the internal aggregate state
        /// This is expensive operation as it serializes internal state, so do not overuse.
        /// Ideally, one should never have to fetch whole state. If you need to expose some
        /// information, then create getter and clone object using DeepClone
        /// </summary>
        /// <returns></returns>
        public TState GetState()
        {
            this.EnsureStateInitialized();

            return this.MyState.DeepClone();
        }

        public IEnumerable<Event> GetPendingEvents()
        {
            return this.pendingEvents.ToArray();
        }

        public void ClearPendingEvents()
        {
            this.pendingEvents.Clear();
        }

        /*/// <summary>
        /// Do not use this method outside of the context of the tests. It's purpose is only to
        /// allow easier set up of the tests of the aggregate and by repository to hydrate aggregate from event stream.
        /// </summary>
        /// <param name="event"></param>
        public void ApplyEvent(Event @event)
        {
            this.MyState.ApplyEvent(@event);
        }*/

        protected void RaiseEvent(Event @event)
        {
            this.EnsureStateInitialized();

            @event.Version = this.Version + 1;

            this.MyState.ApplyEvent(@event);

            this.pendingEvents.Add(@event);
        }

        private void EnsureStateInitialized()
        {
            this.MyState ??= new TState();
        }
    }
}
