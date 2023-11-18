namespace Kuna.EventSourcing.Core.Aggregates;

public interface IAggregateState<TKey>
{
    public Id<TKey> Id { get; }

    public void SetId(TKey aggregateId);

    public int Version { get; }

    public int OriginalVersion { get; }
}