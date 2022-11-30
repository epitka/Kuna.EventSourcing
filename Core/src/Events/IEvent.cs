namespace Senf.EventSourcing.Core.Events
{
    public interface IEvent
    {
        // TODO: do we need version at all ?
        // TODO: should we populate each event with timestamp from server converted to node instance ?
        long? Version { get; set; }
    }
}
