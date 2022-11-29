﻿
using Senf.EventSourcing.Core.Aggregates;

namespace Senf.EventSourcing.Core;

public interface IAggregateRepository<TAggregate>
    where TAggregate : class, IAggregate, new()
{
    ///<summary>Fetches aggregate from the persistent store.
    /// Throws <see cref="AggregateNotFoundException"/> if aggregate is not found.
    /// Throws <see cref="AggregateDeletedException"/> if aggregate is deleted.</summary>
    /// <exception cref="AggregateNotFoundException"> Should be thrown by implementer </exception>
    /// <exception cref="AggregateDeletedException"> Should be thrown by implementer </exception>
    Task<TAggregate> Get(Guid id, CancellationToken cancellationToken);

    ///<summary>Save instance of the aggregate to persistent storage.
    /// Throws <see cref="AggregateConcurrencyException"/> in case of concurrency errors</summary>
    /// <exception cref="AggregateConcurrencyException"> Should be thrown by implementer </exception>
    Task<long> Save(
        TAggregate aggregate,
        params KeyValuePair<string, string>[] metaData);
}
