using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Rules;

/// <summary>
/// Allocates the composite order identifier.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the identifier is 13 characters long. Its ninth position
/// carried the branch code in the legacy multi-branch product; the laboratory is
/// now a single-site installation, so the position is no longer used and the
/// freed position is absorbed by the adjacent segment whose meaning was never
/// established.
/// </para>
/// <para>
/// The identifier layout remains the most consequential data contract in the
/// product: reformatting it breaks the patient identity, the display code and
/// the turnover reporting at the same time. The allocator therefore enforces the
/// layout rather than accepting an arbitrary string.
/// </para>
/// </remarks>
public static class IdentifierAllocator
{
    /// <summary>
    /// Allocates an identifier from its positional segments.
    /// </summary>
    /// <param name="leadingSegment">Positions 1-2. Semantics not established.</param>
    /// <param name="personKey">Positions 3-4. The person key.</param>
    /// <param name="middleSegment">Positions 5-9. Semantics not established.</param>
    /// <param name="displayCode">Positions 10-12. The short display code.</param>
    /// <param name="trailingCharacter">Position 13. Semantics not established.</param>
    public static OrderIdentifier Allocate(
        string leadingSegment,
        string personKey,
        string middleSegment,
        string displayCode,
        char trailingCharacter) =>
        OrderIdentifier.FromSegments(
            leadingSegment,
            personKey,
            middleSegment,
            displayCode,
            trailingCharacter);
}
