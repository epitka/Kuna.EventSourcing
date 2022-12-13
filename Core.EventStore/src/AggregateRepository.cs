using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Core.EventStore;

public abstract class AggregateRepository<TKey, TAggregate> : IAggregateRepository<TKey, TAggregate>
    where TAggregate : class, IAggregate<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly IAggregateStreamReader streamReader;
    private readonly IAggregateStreamWriter streamWriter;

    /// <summary>
    /// name of the aggregate following camel case notation, such as "order-"
    /// </summary>
    public abstract string StreamPrefix { get; }

    protected AggregateRepository(
        IAggregateStreamReader streamReader,
        IAggregateStreamWriter streamWriter)
    {
        this.streamReader = streamReader;
        this.streamWriter = streamWriter;
    }

    public virtual async Task<TAggregate> Get(TKey aggregateId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var streamId = this.GetStreamId(aggregateId);

        var events = await this.streamReader.GetEvents(streamId, ct);

        if (events == Enumerable.Empty<object>())
        {
            throw new AggregateNotFoundException(aggregateId, typeof(TAggregate));
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

        if (!pendingEvents.Any())
        {
            return;
        }

        var streamId = this.GetStreamId(aggregate.Id.Value);

        await this.streamWriter.Write(streamId, aggregate.OriginalVersion.ToStreamRevision(), pendingEvents, ct);
    }

    private string GetStreamId(TKey aggregateId)
    {
        return string.Concat(this.StreamPrefix, aggregateId.ToString());
    }
}
