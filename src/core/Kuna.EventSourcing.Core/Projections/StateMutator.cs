using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuna.EventSourcing.Core.Projections
{
    public static class StateMutator
    {
        /// <summary>
        /// Used to initialize state with events. State can be any arbitrary model, as long as it follow convention of
        /// naming mutation methods as Apply. It will sequentially apply events and mutate state.
        /// </summary>
        /// <param name="events"></param>
        /// <returns>Returns version of the state after applying events</returns>
        public static void Mutate(object state, ref int currentVersion, IEnumerable<object> events)
        {

            foreach (var @event in events)
            {
                Mutate(state, ref currentVersion, @event);
            }
        }


        /// <summary>
        /// By convention, methods that mutate state must be named Apply
        /// Returns version of the state after applying event
        /// </summary>
        /// <param name="event"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Returns version of the state after applying event</returns>
        public static void Mutate(dynamic state, ref int currentVersion, object @event)
        {
            state.Apply((dynamic)@event);

            currentVersion++;
        }
    }
}
