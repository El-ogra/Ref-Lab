using System.Diagnostics.CodeAnalysis;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// The permanent identity of a patient.
/// </summary>
/// <remarks>
/// <para>
/// The legacy system has <em>no</em> patient master table — the table named
/// <c>patients</c> holds operators. Patient identity was expressed only through
/// the person key embedded in the composite order identifier. Ref Lab separates
/// the two concepts: a patient owns a stable identity, and every order that
/// belongs to the patient carries the legacy identifier for traceability.
/// </para>
/// <para>
/// The underlying value is a GUID so that identity can be assigned by the
/// application layer without a database round trip.
/// </para>
/// </remarks>
public readonly struct PatientId : IEquatable<PatientId>
{
    /// <summary>Initialises a new patient identifier.</summary>
    public PatientId(Guid value) => Value = value;

    /// <summary>The underlying GUID value.</summary>
    public Guid Value { get; }

    /// <summary>Creates a patient identifier from an existing GUID.</summary>
    /// <exception cref="ArgumentException">The GUID is empty.</exception>
    public static PatientId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A patient identifier cannot be the empty GUID.", nameof(value));
        }

        return new PatientId(value);
    }

    /// <summary>Creates a new, random patient identifier.</summary>
    public static PatientId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public bool Equals(PatientId other) => Value.Equals(other.Value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is PatientId other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value.ToString("D");

    /// <summary>Compares two patient identifiers.</summary>
    public static bool operator ==(PatientId left, PatientId right) => left.Equals(right);

    /// <summary>Compares two patient identifiers.</summary>
    public static bool operator !=(PatientId left, PatientId right) => !left.Equals(right);

    /// <summary>Converts to the underlying GUID.</summary>
    public static implicit operator Guid(PatientId id) => id.Value;
}
