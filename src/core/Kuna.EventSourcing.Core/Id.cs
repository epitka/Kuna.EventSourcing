namespace Kuna.EventSourcing.Core;

public sealed class Id<T>(T value) : IEquatable<T>
{
    public T Value { get; } = value;

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
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == this.GetType() && this.Equals((Id<T>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(this.Value!);
    }

    public static implicit operator T(Id<T> obj)
    {
        return obj.Value;
    }

    public static implicit operator Id<T>?(T obj)
    {
        return obj == null
            ? null
            : new Id<T>(obj);
    }

    public override string ToString()
    {
        if (this.Value == null)
        {
            return "N/A";
        }

        // Q: why is this reporting a nullability warning?
        return this.Value.ToString()!;
    }
}
