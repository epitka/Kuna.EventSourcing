namespace Kuna.EventSourcing.Core.Aggregates;

public interface IAggregateRepository<in TKey, TAggregate>
    where TAggregate : class, IAggregate<TKey>, new()
{
    ///<summary>Fetches aggregate from the persistent store.
    /// Throws <see cref="AggregateNotFoundException"/> if aggregate is not found.
    /// </summary>
    /// <exception cref="AggregateNotFoundException"> Should be thrown by implementer </exception>
    Task<TAggregate> Get(TKey id, CancellationToken cancellationToken);

    ///<summary>Save instance of the aggregate to persistent storage.
    /// Throws <see cref="AggregateConcurrencyException"/> in case of concurrency errors
    /// </summary>
    /// <exception cref="AggregateConcurrencyException"> Should be thrown by implementer </exception>
    Task Save(
        TAggregate aggregate,
        CancellationToken ct);
}
