using Fare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuna.EventSourcing.Core
{
    /// <summary>
    /// General purpose interface for loading, creating transient projections of state, and saving events
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Loads events from stream
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        Task<object[]> LoadEvents(string streamId, CancellationToken ct);


        /// <summary>
        /// Saves  events to stream
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="events"></param>
        Task SaveEvents(string streamId, object[] events, int expectedVersion, CancellationToken ct);

        /// <summary>
        /// Loads events from the strea, creates new instance of TState and applies events to it
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="streamId"></param>
        /// <returns></returns>
        Task<TState> Project<TState>(string streamId, CancellationToken ct)
            where TState : new();

        /// <summary>
        /// Projects events to  existing instance of the state
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        TState Project<TState>(TState state, ref int version, object[] events);
    }
}
