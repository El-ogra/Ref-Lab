using System.Diagnostics.CodeAnalysis;

namespace RefLab.Domain.ValueObjects;

/// <summary>
/// The composite order/visit identifier of a patient order.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the legacy system used a 13-character identifier whose
/// ninth character carried the branch code of the operator who registered the
/// order. The laboratory is now a single-site system with no branches, so that
/// position is no longer used.
/// </para>
/// <para>
/// The segments that <em>were</em> established by the legacy analysis are kept
/// exactly where they were, so that the two documented readings still work:
/// <c>MID(patientid,3,2)</c> is the person key and <c>mid(patientid,10,3)</c>
/// is the display code. The position the branch occupied is absorbed by the
/// adjacent segment whose meaning was never established.
/// </para>
/// <list type="table">
/// <item><term>1–2</term><description>Leading segment — semantics never established.</description></item>
/// <item><term>3–4</term><description>The person key, as established by the legacy analysis.</description></item>
/// <item><term>5–9</term><description>Middle segment — semantics never established; absorbs the freed position.</description></item>
/// <item><term>10–12</term><description>The display code, as established by the legacy analysis.</description></item>
/// <item><term>13</term><description>Trailing character — semantics never established.</description></item>
/// </list>
/// <para>
/// The identifier layout remains a hard contract: reformatting it breaks the
/// patient identity, the display code and the turnover reporting at once.
/// </para>
/// </remarks>
public sealed class OrderIdentifier : IEquatable<OrderIdentifier>
{
    /// <summary>The exact character length of the identifier.</summary>
    public const int Length = 13;

    private OrderIdentifier(string value)
    {
        Value = value;
    }

    /// <summary>The raw identifier exactly as stored.</summary>
    public string Value { get; private set; }

    /// <summary>Positions 1-2. Semantics not established; exposed verbatim.</summary>
    public string LeadingSegment => Value.Substring(0, 2);

    /// <summary>Positions 3-4. The person key used by turnover reporting.</summary>
    public string PersonKey => Value.Substring(2, 2);

    /// <summary>Positions 5-9. Semantics not established; exposed verbatim.</summary>
    public string MiddleSegment => Value.Substring(4, 5);

    /// <summary>Positions 10-12. The short display code.</summary>
    public string DisplayCode => Value.Substring(9, 3);

    /// <summary>Position 13. Semantics not established; exposed verbatim.</summary>
    public char TrailingCharacter => Value[12];

    /// <summary>
    /// Creates an identifier from an existing stored value.
    /// </summary>
    /// <exception cref="Exceptions.InvalidOrderIdentifierException">
    /// The value is not exactly <see cref="Length"/> characters long.
    /// </exception>
    public static OrderIdentifier Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length != Length)
        {
            throw new Exceptions.InvalidOrderIdentifierException(
                value.Length,
                Length,
                "A legacy order identifier must be exactly 13 characters long.");
        }

        return new OrderIdentifier(value);
    }

    /// <summary>
    /// Rebuilds an identifier from its positional segments.
    /// </summary>
    /// <exception cref="Exceptions.InvalidOrderIdentifierException">
    /// A segment has an invalid length.
    /// </exception>
    public static OrderIdentifier FromSegments(
        string leadingSegment,
        string personKey,
        string middleSegment,
        string displayCode,
        char trailingCharacter)
    {
        ArgumentNullException.ThrowIfNull(leadingSegment);
        ArgumentNullException.ThrowIfNull(personKey);
        ArgumentNullException.ThrowIfNull(middleSegment);
        ArgumentNullException.ThrowIfNull(displayCode);

        if (leadingSegment.Length != 2)
        {
            throw new Exceptions.InvalidOrderIdentifierException(
                leadingSegment.Length,
                2,
                "The leading segment (positions 1-2) must be exactly 2 characters long.");
        }

        if (personKey.Length != 2)
        {
            throw new Exceptions.InvalidOrderIdentifierException(
                personKey.Length,
                2,
                "The person key (positions 3-4) must be exactly 2 characters long.");
        }

        if (middleSegment.Length != 5)
        {
            throw new Exceptions.InvalidOrderIdentifierException(
                middleSegment.Length,
                5,
                "The middle segment (positions 5-9) must be exactly 5 characters long.");
        }

        if (displayCode.Length != 3)
        {
            throw new Exceptions.InvalidOrderIdentifierException(
                displayCode.Length,
                3,
                "The display code (positions 10-12) must be exactly 3 characters long.");
        }

        var value = string.Concat(leadingSegment, personKey, middleSegment, displayCode, trailingCharacter);

        return new OrderIdentifier(value);
    }

    /// <inheritdoc />
    public bool Equals(OrderIdentifier? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as OrderIdentifier);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>Compares two identifiers by their ordinal value.</summary>
    public static bool operator ==(OrderIdentifier? left, OrderIdentifier? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Compares two identifiers by their ordinal value.</summary>
    public static bool operator !=(OrderIdentifier? left, OrderIdentifier? right) => !(left == right);
}
