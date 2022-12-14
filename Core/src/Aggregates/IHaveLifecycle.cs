namespace Senf.EventSourcing.Core.Aggregates;

public interface IHaveLifecycle
{
    public void OnAfterLoad();

    public void OnBeforeSave();

}
