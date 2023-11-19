namespace Kuna.EventSourcing.Core.Aggregates;

public interface IAggregateState<TKey> : IHaveVersion
{
    public Id<TKey> Id { get; }

    public void SetId(TKey aggregateId);

    public int OriginalVersion { get; set; }
}