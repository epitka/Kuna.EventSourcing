namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregateState
{
    public Guid Id { get; }

    public void SetId(Guid aggregateId);

    public int Version { get; set; }

    public int OriginalVersion { get; set; }
}