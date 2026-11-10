using System.Globalization;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// A named group of clinical computation constants.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>constanttable</c> carries <c>constantdiscrp</c> (text 50)
/// and <c>constant1</c> to <c>constant7</c> (each text 50). The shipped table
/// holds two groups — <c>CBCCONS</c> and <c>PTCONS</c>.
/// </para>
/// <para>
/// The values are stored as <em>text</em> so that a non-numeric token such as
/// <c>2CVJW</c> can coexist with the numbers. Ref Lab preserves that capability:
/// a constant is either numeric or free text, and the caller decides which.
/// </para>
/// </remarks>
public sealed class ClinicalConstant
{
    /// <summary>The number of constant slots a group provides.</summary>
    public const int SlotCount = 7;

    private ClinicalConstant()
    {
    }

    /// <summary>The group name, for example <c>CBCCONS</c>.</summary>
    public string GroupName { get; private set; } = string.Empty;

    /// <summary>The constant values, indexed from 1 to <see cref="SlotCount"/>.</summary>
    public IReadOnlyList<string?> Values { get; private set; } = Array.Empty<string?>();

    /// <summary>The constant slot holding the haemoglobin to haematocrit factor.</summary>
    public const string CbcGroupName = "CBCCONS";

    /// <summary>The constant slot holding the prothrombin time control factors.</summary>
    public const string PtGroupName = "PTCONS";

    /// <summary>Creates a constant group.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The group name is empty or more than <see cref="SlotCount"/> values are supplied.
    /// </exception>
    public static ClinicalConstant Create(string groupName, params string?[] values)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new InvalidTestDefinitionException("A clinical constant group must have a name.");
        }

        if (values.Length > SlotCount)
        {
            throw new InvalidTestDefinitionException(
                $"A clinical constant group provides at most {SlotCount} slots.");
        }

        var normalized = new string?[SlotCount];

        for (var index = 0; index < values.Length; index++)
        {
            var value = values[index];
            normalized[index] = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        return new ClinicalConstant { GroupName = groupName.Trim(), Values = normalized };
    }

    /// <summary>Returns the raw text held in a constant slot.</summary>
    public string? GetText(int slot)
    {
        ValidateSlot(slot);
        return Values[slot - 1];
    }

    /// <summary>
    /// Returns the numeric value held in a constant slot.
    /// </summary>
    /// <exception cref="InvalidClinicalConstantException">
    /// The slot is empty or does not hold a number.
    /// </exception>
    public decimal GetNumber(int slot)
    {
        ValidateSlot(slot);

        var text = Values[slot - 1];

        if (text is null ||
            !decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            throw new InvalidClinicalConstantException(GroupName, slot);
        }

        return value;
    }

    /// <summary>Attempts to read the numeric value held in a constant slot.</summary>
    public bool TryGetNumber(int slot, out decimal value)
    {
        ValidateSlot(slot);

        var text = Values[slot - 1];
        value = default;

        return text is not null &&
               decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    private static void ValidateSlot(int slot)
    {
        if (slot < 1 || slot > SlotCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(slot),
                slot,
                $"A constant slot must be between 1 and {SlotCount}.");
        }
    }
}
