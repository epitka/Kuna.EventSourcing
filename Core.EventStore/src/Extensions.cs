using EventStore.Client;

namespace Kuna.EventSourcing.Core.EventStore;

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

    public static StreamRevision ToStreamRevision(this long value)
    {
        return value switch
        {
            -1    => StreamRevision.None,
            > -1  => new StreamRevision(Convert.ToUInt64(value)),
            var _ => throw new Exception("Revision is less than -1."),
        };
    }
}
