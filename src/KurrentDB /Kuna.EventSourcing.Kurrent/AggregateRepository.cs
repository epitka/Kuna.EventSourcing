using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Exceptions;
using KurrentDB.Client;

namespace Kuna.EventSourcing.Kurrent;

public abstract class AggregateRepository<TKey, TAggregate> : IAggregateRepository<TKey, TAggregate>
    where TAggregate : class, IAggregate<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly IStreamReader streamReader;
    private readonly IStreamWriter streamWriter;

    /// <summary>
    /// name of the aggregate following camel case notation, such as "order-"
    /// </summary>
    public abstract string StreamPrefix { get; }

    protected AggregateRepository(
        IStreamReader streamReader,
        IStreamWriter streamWriter)
    {
        this.streamReader = streamReader;
        this.streamWriter = streamWriter;
    }

    public virtual async Task<TAggregate> Get(TKey id, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        ct.ThrowIfCancellationRequested();

        var streamId = this.GetStreamId(id);

        var events = await this.streamReader.GetEvents(streamId, ct)
                               .ConfigureAwait(false);

        if (events == Enumerable.Empty<object>())
        {
            // Q: why is this reproting nullability warning?
            throw new AggregateNotFoundException<TAggregate>(id:id.ToString()!);
        }

        ct.ThrowIfCancellationRequested();

        var aggregate = new TAggregate();

        aggregate.InitWith(events);

        return aggregate;
    }

    public virtual async Task Save(
        TAggregate aggregate,
        CancellationToken ct)
    {
        var pendingEvents = aggregate.DequeuePendingEvents();

        if (pendingEvents.Length==0)
        {
            return;
        }

        if (aggregate.Id == null)
        {
            throw new InvalidOperationException("Aggregate does not have Id assigned");
        }

        var streamId = this.GetStreamId(aggregate.Id.Value);

        await this.streamWriter
                  .Write(streamId, aggregate.OriginalVersion, pendingEvents, ct)
                  .ConfigureAwait(false);
    }

    private string GetStreamId(TKey aggregateId)
    {
        return string.Concat(this.StreamPrefix, aggregateId.ToString());
    }
}
