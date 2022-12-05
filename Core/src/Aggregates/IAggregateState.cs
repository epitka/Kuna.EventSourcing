namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregateState<TKey>
{
    public Id<TKey> Id { get; }

    public void SetId(TKey aggregateId);

    public int Version { get; set; }

    public int OriginalVersion { get; set; }
}