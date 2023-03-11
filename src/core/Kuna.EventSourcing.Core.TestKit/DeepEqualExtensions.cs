using DeepEqual.Syntax;
using Kuna.EventSourcing.Core.Aggregates;

namespace Kuna.EventSourcing.Core.TestKit;

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
