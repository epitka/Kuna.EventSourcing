using System.Runtime.Serialization;

namespace Kuna.EventSourcing.Core.Exceptions;

[DataContract]
public class AggregateDeletedException : AggregateExceptionBase
{
    public AggregateDeletedException(object id, Type type) : base(id, type)
    {
    }

    public AggregateDeletedException(object id, Type type, string message) : base(id, type, message)
    {
    }

    public AggregateDeletedException(object id, Type type, string message, Exception innerException) : base(id, type, message, innerException)
    {
    }
}