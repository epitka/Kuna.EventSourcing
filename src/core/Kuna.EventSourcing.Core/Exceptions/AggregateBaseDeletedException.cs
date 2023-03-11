using System.Runtime.Serialization;

namespace Kuna.EventSourcing.Core.Exceptions;

[DataContract]
public class AggregateBaseDeletedException : AggregateBaseException
{
    public AggregateBaseDeletedException(object id, Type type) : base(id, type)
    {
    }

    public AggregateBaseDeletedException(object id, Type type, string message) : base(id, type, message)
    {
    }

    public AggregateBaseDeletedException(object id, Type type, string message, Exception innerException) : base(id, type, message, innerException)
    {
    }
}