namespace Kuna.EventSourcing.Core.Aggregates;

/// <summary>
/// interface that can be used in conjunction with IRepository as hooks to be invoked
/// on the aggregate.
/// </summary>
public interface IAggregateLifecycle
{
    public void OnAfterLoad();

    public void OnBeforeSave();

}
