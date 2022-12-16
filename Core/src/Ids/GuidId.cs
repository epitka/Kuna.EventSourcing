﻿using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace Senf.EventSourcing.Core.Ids;

public sealed class GuidId
    : IEquatable<GuidId>,
      IComparable<GuidId>
{
    public GuidId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be Guid.Empty.");
        }

        this.Value = id;
    }

    private GuidId()
    {
    }

    public Guid Value { get; }

    public static implicit operator Guid(GuidId obj)
    {
        return obj.Value;
    }

    public static implicit operator Guid?(GuidId? obj)
    {
        return obj?.Value;
    }

    public static implicit operator GuidId(Guid obj)
    {
        return new GuidId(obj);
    }

    public static implicit operator GuidId?(Guid? obj)
    {
        return obj == null
            ? null
            : new GuidId(obj.Value);
    }

    /// <summary>
    /// Generates next Guid based id
    /// </summary>
    /// <returns>Id</returns>
    public static GuidId Create()
    {
        return new GuidId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return this.Value.ToString();
    }

    public int CompareTo(GuidId? other)
    {
        if (other is null)
        {
            return -1;
        }

        return this.Value.CompareTo(other.Value);
    }


    public bool Equals(GuidId? other)
    {
        return other is not null && this.Value.Equals(other.Value);
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is GuidId other && this.Equals(other);
    }

    public override int GetHashCode() => this.Value.GetHashCode();

    public static bool operator ==(GuidId a, GuidId b) => a.Value.CompareTo(b.Value) == 0;
    public static bool operator !=(GuidId a, GuidId b) => !(a.Value == b.Value);
}