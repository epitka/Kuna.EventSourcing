using Kuna.EventSourcing.Core.Aggregates;

namespace Kuna.EventSourcing.Core.Exceptions;

public abstract class AggregateException<TAggregate>(string id) : Exception
{
    public string Id { get; } = id;

    public Type Type { get; } = typeof(TAggregate);

}