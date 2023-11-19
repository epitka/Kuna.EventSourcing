using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuna.EventSourcing.Core
{
    public static class StateMutator
    {
        /// <summary>
        /// Used to initialize state with events. It will sequentially apply events and mutate state.
        /// </summary>
        /// <param name="events"></param>
       /// <returns>Returns version of the state after applying events</returns>
        public static void Mutate(object state, ref int currentVersion, IEnumerable<object> events)
        {
/*          if (this.Version > -1)
            {
                throw new InvalidOperationException("State is already initialized");
            }*/

            foreach (var @event in events)
            {
               Mutate(state, ref currentVersion, @event);
            }

            // this.OriginalVersion = this.Version;
        }


        /// <summary>
        /// By convention, methods that mutate state must be named Apply
        /// Returns version of the state after applying event
        /// </summary>
        /// <param name="event"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Returns version of the state after applying event</returns>
        public static void Mutate(object state, ref int currentVersion, object @event)
        {
            ((dynamic)state).Apply((dynamic)@event);

            currentVersion++;
        }
    }
}
