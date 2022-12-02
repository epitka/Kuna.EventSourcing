using EventStore.Client;

namespace Senf.EventSourcing.Core.EventStore;

public static class Extensions
{
    public static StreamRevision ToStreamRevision(this int value)
    {
        return value switch
        {
            -1   => StreamRevision.None,
            > -1 => new StreamRevision(Convert.ToUInt64(value)),
            var _    => throw new Exception("Revision is less than -1."),
        };
    }
}
