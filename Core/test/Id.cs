namespace Senf.EventSourcing.Core.Tests;

public static class Id
{
    // TODO: temp until we introduce typed key
    public static Guid Create()
    {
        return Guid.NewGuid();
    }
}
