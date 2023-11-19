using System.Runtime.Serialization;

namespace Kuna.EventSourcing.Core.Exceptions;

public class AggregateNotFoundException<TAggregate>(string id): AggregateException<TAggregate>(id)
{
}