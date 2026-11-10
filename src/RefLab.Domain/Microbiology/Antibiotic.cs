using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Microbiology;

/// <summary>
/// An antibiotic in the laboratory catalogue.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the <c>antibiotic</c> table carries 25 columns and 38 rows,
/// including <c>antisymbol</c>, <c>scientificname</c>, eight common-name columns
/// <c>anticomname1</c> to <c>anticomname8</c>, eight print flags
/// <c>anticomprint1</c> to <c>anticomprint8</c>, the pregnancy and child flags
/// <c>prgnancy</c> and <c>child</c>, and the susceptibility fields
/// <c>Sensitivity</c> and <c>Sensitiveunit</c>.
/// </para>
/// <para>
/// A second, reduced table <c>antibio</c> carries 96 rows of
/// <c>antisymbol</c> and <c>scientificname</c> only. The relationship between
/// the two structures is not established, so Ref Lab models one catalogue and
/// keeps the reduced table's symbol-to-name mapping as its identifier.
/// </para>
/// </remarks>
public sealed class Antibiotic
{
    private Antibiotic()
    {
    }

    /// <summary>The antibiotic identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The short symbol printed on the susceptibility report.</summary>
    public string Symbol { get; private set; } = string.Empty;

    /// <summary>The scientific name of the antibiotic.</summary>
    public string ScientificName { get; private set; } = string.Empty;

    /// <summary>The culture result type the antibiotic is reported against.</summary>
    public string? CultureType { get; private set; }

    /// <summary>The common names the antibiotic is known by, in report order.</summary>
    public IReadOnlyList<string> CommonNames { get; private set; } = Array.Empty<string>();

    /// <summary>Whether the antibiotic may be used during pregnancy.</summary>
    public bool IsAllowedInPregnancy { get; private set; }

    /// <summary>Whether the antibiotic may be used in children.</summary>
    public bool IsAllowedInChildren { get; private set; }

    /// <summary>The susceptibility text applied to the antibiotic.</summary>
    public string? Sensitivity { get; private set; }

    /// <summary>The unit the susceptibility is expressed in.</summary>
    public string? SensitivityUnit { get; private set; }

    /// <summary>Creates an antibiotic.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The identifier is not positive or the symbol is empty.
    /// </exception>
    public static Antibiotic Create(
        int id,
        string symbol,
        string scientificName,
        string? cultureType = null,
        bool isAllowedInPregnancy = false,
        bool isAllowedInChildren = false,
        IReadOnlyList<string>? commonNames = null)
    {
        if (id <= 0)
        {
            throw new InvalidTestDefinitionException("An antibiotic identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new InvalidTestDefinitionException("An antibiotic must have a symbol.");
        }

        if (string.IsNullOrWhiteSpace(scientificName))
        {
            throw new InvalidTestDefinitionException("An antibiotic must have a scientific name.");
        }

        var antibiotic = new Antibiotic
        {
            Id = id,
            Symbol = symbol.Trim(),
            ScientificName = scientificName.Trim(),
            CultureType = string.IsNullOrWhiteSpace(cultureType) ? null : cultureType.Trim(),
            IsAllowedInPregnancy = isAllowedInPregnancy,
            IsAllowedInChildren = isAllowedInChildren,
            CommonNames = commonNames?
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .ToArray() ?? Array.Empty<string>(),
        };

        return antibiotic;
    }

    /// <summary>Updates the susceptibility applied to the antibiotic.</summary>
    public void SetSusceptibility(string? sensitivity, string? unit)
    {
        Sensitivity = string.IsNullOrWhiteSpace(sensitivity) ? null : sensitivity.Trim();
        SensitivityUnit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();
    }

    /// <summary>Updates the clinical suitability flags of the antibiotic.</summary>
    public void SetClinicalFlags(bool isAllowedInPregnancy, bool isAllowedInChildren)
    {
        IsAllowedInPregnancy = isAllowedInPregnancy;
        IsAllowedInChildren = isAllowedInChildren;
    }

    /// <summary>Updates the common names of the antibiotic.</summary>
    public void SetCommonNames(IReadOnlyList<string> commonNames)
    {
        ArgumentNullException.ThrowIfNull(commonNames);

        CommonNames = commonNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .ToArray();
    }
}
