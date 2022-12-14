namespace Senf.EventSourcing.Core.Tests;

public static class Id
{
    public static Guid Create()
    {
        return Guid.NewGuid();
    }
}
