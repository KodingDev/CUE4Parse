using System;
using System.Linq;

namespace CUE4Parse.Utils;

/// <summary>
/// Represents a version number for games with comparable semantics.
/// </summary>
public sealed class ArbitraryVersion : IComparable<ArbitraryVersion>, IEquatable<ArbitraryVersion>
{
    private readonly string _version;
    private readonly int[] _components;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArbitraryVersion"/> class.
    /// </summary>
    /// <param name="version">The version string in dot-separated format (e.g., "1.2.3").</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="version"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any component in the version string is not a valid integer.</exception>
    public ArbitraryVersion(string version)
    {
        ArgumentException.ThrowIfNullOrEmpty(version, nameof(version));

        _version = version;
        var parts = version.Split('.');
        _components = new int[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], out _components[i]))
            {
                throw new ArgumentException($"Version component '{parts[i]}' must be a valid integer", nameof(version));
            }
        }
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="ArbitraryVersion"/>.
    /// </summary>
    /// <param name="version">The version string to convert.</param>
    public static implicit operator ArbitraryVersion(string version) => new(version);

    /// <summary>
    /// Determines whether one version is greater than another.
    /// </summary>
    public static bool operator >(ArbitraryVersion? left, ArbitraryVersion? right)
    {
        if (left is null) return false;
        if (right is null) return true;
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Determines whether one version is less than another.
    /// </summary>
    public static bool operator <(ArbitraryVersion? left, ArbitraryVersion? right)
    {
        if (left is null) return right is not null;
        if (right is null) return false;
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Determines whether one version is greater than or equal to another.
    /// </summary>
    public static bool operator >=(ArbitraryVersion? left, ArbitraryVersion? right)
    {
        if (left is null) return right is null;
        if (right is null) return true;
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Determines whether one version is less than or equal to another.
    /// </summary>
    public static bool operator <=(ArbitraryVersion? left, ArbitraryVersion? right)
    {
        if (left is null) return true;
        if (right is null) return false;
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Determines whether two versions are equal.
    /// </summary>
    public static bool operator ==(ArbitraryVersion? left, ArbitraryVersion? right)
    {
        if (left is null) return right is null;
        if (right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two versions are not equal.
    /// </summary>
    public static bool operator !=(ArbitraryVersion? left, ArbitraryVersion? right) => !(left == right);

    /// <summary>
    /// Compares this instance with a specified <see cref="ArbitraryVersion"/> and returns an
    /// integer that indicates whether this instance precedes, follows, or appears in the same
    /// position in the sort order as the specified object.
    /// </summary>
    /// <param name="other">The version to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared.
    /// Less than zero: This instance precedes <paramref name="other"/> in the sort order.
    /// Zero: This instance occurs in the same position in the sort order as <paramref name="other"/>.
    /// Greater than zero: This instance follows <paramref name="other"/> in the sort order or <paramref name="other"/> is null.
    /// </returns>
    public int CompareTo(ArbitraryVersion? other) => other is null ? 1 : Compare(this, other);

    /// <summary>
    /// Determines whether this instance and a specified object, which must also be a <see cref="ArbitraryVersion"/> object, have the same value.
    /// </summary>
    /// <param name="obj">The object to compare to this instance.</param>
    /// <returns>true if <paramref name="obj"/> is a <see cref="ArbitraryVersion"/> and its value is the same as this instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => Equals(obj as ArbitraryVersion);

    /// <summary>
    /// Determines whether this instance and another specified <see cref="ArbitraryVersion"/> object have the same value.
    /// </summary>
    /// <param name="other">The <see cref="ArbitraryVersion"/> to compare to this instance.</param>
    /// <returns>true if the value of the <paramref name="other"/> parameter is the same as this instance; otherwise, false.</returns>
    public bool Equals(ArbitraryVersion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Compare(this, other) == 0;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode() =>
        _components.Aggregate(17, (hash, component) => hash * 23 + component);

    /// <summary>
    /// Converts the value of this instance to its equivalent string representation.
    /// </summary>
    /// <returns>The string representation of this instance.</returns>
    public override string ToString() => _version;

    private static int Compare(ArbitraryVersion left, ArbitraryVersion right)
    {
        int maxLength = Math.Max(left._components.Length, right._components.Length);

        for (int i = 0; i < maxLength; i++)
        {
            int leftValue = i < left._components.Length ? left._components[i] : 0;
            int rightValue = i < right._components.Length ? right._components[i] : 0;

            if (leftValue != rightValue)
                return leftValue.CompareTo(rightValue);
        }

        return 0;
    }
}