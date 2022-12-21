using DeepEqual.Syntax;

namespace Senf.EventSourcing.Testing;

public static class DeepEqualExtensions
{
    public static void ShouldBeDeepEqualTo<TKey>(this IAggregateState<TKey> obj, IAggregateState<TKey> other)
    {
        obj.WithDeepEqual(other)
           .IgnoreDestinationProperty(x => x.Version)
           .IgnoreDestinationProperty(x => x.OriginalVersion)
           .Assert();
    }
}
