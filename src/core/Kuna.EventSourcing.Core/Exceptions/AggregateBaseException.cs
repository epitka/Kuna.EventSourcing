namespace Kuna.EventSourcing.Core.Exceptions;

public abstract class AggregateBaseException : Exception
{
    protected AggregateBaseException(object id, Type type)
    {
        this.Id = id;
        this.Type = type;
    }

    protected AggregateBaseException(object id, Type type, string message) : base(message)
    {
        this.Id = id;
        this.Type = type;
    }

    protected AggregateBaseException(object id, Type type, string message, Exception innerException)
        : base(message, innerException)
    {
        this.Id = id;
        this.Type = type;
    }

    public object Id { get; }

    public Type Type { get; }
}