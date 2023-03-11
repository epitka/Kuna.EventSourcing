using System.Runtime.Serialization;

namespace Kuna.EventSourcing.Core.Exceptions;

[DataContract]
public class AggregateBaseNotFoundException : AggregateBaseException
{
    public AggregateBaseNotFoundException(object id, Type type) : base(id, type)
    {
    }

    public AggregateBaseNotFoundException(object id, Type type, string message) : base(id, type, message)
    {
    }

    public AggregateBaseNotFoundException(object id, Type type, string message, Exception innerException)
        : base(id, type, message, innerException)
    {
    }
}