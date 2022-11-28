namespace Senf.EventSourcing.Core.Events
{
    public record Event : IEvent
    {
        public long Version { get; set; } = -1;
    }
}
