using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Domain.Rules;

/// <summary>
/// Resolves the clinical computation constants held as data.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the <c>constanttable</c> holds its values in text columns
/// so that a non-numeric token such as <c>2CVJW</c> can coexist with the
/// numbers. Two groups ship with the product — <c>CBCCONS</c> and
/// <c>PTCONS</c> — and the values in the database match the manual exactly.
/// </para>
/// <para>
/// Keeping the constants as data means a laboratory can correct a factor
/// without a software change. Ref Lab preserves that property: the factors are
/// resolved at run time from the configured constant groups, never hard-coded
/// into the derivation logic.
/// </para>
/// </remarks>
public static class ClinicalConstantResolver
{
    /// <summary>
    /// Resolves a constant group by its name.
    /// </summary>
    /// <returns>The group, or <see langword="null"/> when it is not configured.</returns>
    public static ClinicalConstant? Resolve(IEnumerable<ClinicalConstant> constants, string groupName)
    {
        ArgumentNullException.ThrowIfNull(constants);
        ArgumentNullException.ThrowIfNull(groupName);

        var normalized = groupName.Trim();

        return constants.FirstOrDefault(constant =>
            string.Equals(constant.GroupName, normalized, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Resolves a numeric constant from a group.
    /// </summary>
    /// <exception cref="Exceptions.InvalidClinicalConstantException">
    /// The group is not configured, or the slot does not hold a number.
    /// </exception>
    public static decimal ResolveNumber(IEnumerable<ClinicalConstant> constants, string groupName, int slot)
    {
        var group = Resolve(constants, groupName)
            ?? throw new InvalidClinicalConstantException(groupName, slot);

        return group.GetNumber(slot);
    }

    /// <summary>
    /// Resolves a textual constant from a group.
    /// </summary>
    public static string? ResolveText(IEnumerable<ClinicalConstant> constants, string groupName, int slot)
    {
        var group = Resolve(constants, groupName);
        return group?.GetText(slot);
    }
}
