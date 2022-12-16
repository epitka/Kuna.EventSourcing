namespace Senf.EventSourcing.Core.Aggregates;

public sealed class Id<T> : IEquatable<T>
{
    public Id(T value)
    {
        this.Value = value;
    }

    public T Value
    {
        get;
    }

    public bool Equals(Id<T> other)
    {
        return EqualityComparer<T>.Default.Equals(this.Value, other.Value);
    }

    public bool Equals(T? other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((Id<T>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(this.Value!);
    }
}
