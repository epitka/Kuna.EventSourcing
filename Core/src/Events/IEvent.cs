namespace Senf.EventSourcing.Core.Events
{
    public interface IEvent
    {
        long Version { get; set; }
    }
}
