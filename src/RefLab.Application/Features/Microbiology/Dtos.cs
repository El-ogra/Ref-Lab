using RefLab.Domain.Microbiology;

namespace RefLab.Application.Features.Microbiology;

/// <summary>
/// One antimicrobial susceptibility of a culture result.
/// </summary>
public sealed record AntibioticSensitivityDto(string AntibioticSymbol, string Reading, string? Unit);

/// <summary>
/// A microbiology culture result as presented to the application layer.
/// </summary>
public sealed record CultureResultDto(
    Guid Id,
    Guid PatientId,
    Guid? OrderId,
    DateTimeOffset RequestDate,
    string CultureId,
    string Organism,
    string? SecondOrganism,
    string? Condition,
    string? Colony,
    string? Reactions,
    IReadOnlyList<AntibioticSensitivityDto> Susceptibilities)
{
    /// <summary>Projects a culture result onto its DTO.</summary>
    public static CultureResultDto FromDomain(CultureResult culture) =>
        new(
            culture.Id,
            culture.PatientId.Value,
            culture.OrderId,
            culture.RequestDate,
            culture.CultureId,
            culture.Organism,
            culture.SecondOrganism,
            culture.Condition,
            culture.Colony,
            culture.Reactions,
            culture.Susceptibilities
                .Select(susceptibility =>
                    new AntibioticSensitivityDto(susceptibility.AntibioticSymbol, susceptibility.Reading, susceptibility.Unit))
                .ToArray());
}

/// <summary>
/// An antibiotic as presented to the application layer.
/// </summary>
public sealed record AntibioticDto(
    int Id,
    string Symbol,
    string ScientificName,
    string? CultureType,
    IReadOnlyList<string> CommonNames,
    bool IsAllowedInPregnancy,
    bool IsAllowedInChildren,
    string? Sensitivity,
    string? SensitivityUnit)
{
    /// <summary>Projects an antibiotic onto its DTO.</summary>
    public static AntibioticDto FromDomain(Antibiotic antibiotic) =>
        new(
            antibiotic.Id,
            antibiotic.Symbol,
            antibiotic.ScientificName,
            antibiotic.CultureType,
            antibiotic.CommonNames,
            antibiotic.IsAllowedInPregnancy,
            antibiotic.IsAllowedInChildren,
            antibiotic.Sensitivity,
            antibiotic.SensitivityUnit);
}
