using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// A test in the catalogue as presented to the application layer.
/// </summary>
public sealed record TestDefinitionDto(
    int Code,
    string Name,
    string SimpleName,
    string? ArabicName,
    string? Abbreviation,
    int SubGroupCode,
    string? SubGroupName,
    int ArrangementNumber,
    bool IsActive,
    bool IsSendOut,
    string? OtherLabName,
    string? SampleType,
    decimal PatientPrice,
    decimal LabPrice,
    decimal OtherLabPrice,
    bool IsDeleteProtected,
    bool PrintWithGroup,
    decimal? LowCriticalLimit,
    decimal? HighCriticalLimit)
{
    /// <summary>Projects a test definition onto its DTO.</summary>
    public static TestDefinitionDto FromDomain(TestDefinition test) =>
        new(
            test.Code,
            test.Name,
            test.SimpleName,
            test.ArabicName,
            test.Abbreviation,
            test.SubGroupCode,
            test.SubGroupName,
            test.ArrangementNumber,
            test.IsActive,
            test.IsSendOut,
            test.OtherLabName,
            test.SampleType,
            test.PatientPrice,
            test.LabPrice,
            test.OtherLabPrice,
            test.IsDeleteProtected,
            test.PrintWithGroup,
            test.CriticalInterval.LowCriticalLimit,
            test.CriticalInterval.HighCriticalLimit);
}

/// <summary>
/// A reference range as presented to the application layer.
/// </summary>
public sealed record ReferenceRangeDto(
    int Id,
    int TestCode,
    string TestSimpleName,
    SexScope SexScope,
    decimal? AgeFrom,
    decimal? AgeTo,
    AgeUnit AgeUnit,
    decimal? LowLimit,
    decimal? HighLimit,
    string? LowExceptionText,
    string? HighExceptionText,
    string? RangeText,
    string? Unit,
    decimal? LowCriticalLimit,
    decimal? HighCriticalLimit,
    string? CriticalExceptionText)
{
    /// <summary>Projects a reference range onto its DTO.</summary>
    public static ReferenceRangeDto FromDomain(ReferenceRange range) =>
        new(
            range.Id,
            range.TestCode,
            range.TestSimpleName,
            range.SexScope,
            range.AgeBand.From,
            range.AgeBand.To,
            range.AgeBand.Unit,
            range.Interval.LowLimit,
            range.Interval.HighLimit,
            range.Interval.LowExceptionText,
            range.Interval.HighExceptionText,
            range.Interval.RangeText,
            range.Interval.Unit,
            range.Interval.CriticalInterval.LowCriticalLimit,
            range.Interval.CriticalInterval.HighCriticalLimit,
            range.Interval.CriticalInterval.CriticalExceptionText);
}

/// <summary>
/// A clinical sub-group as presented to the application layer.
/// </summary>
public sealed record ClinicalSubGroupDto(int Code, string Name, string? ArabicName, bool IsDeleteProtected)
{
    /// <summary>Projects a clinical sub-group onto its DTO.</summary>
    public static ClinicalSubGroupDto FromDomain(ClinicalSubGroup group) =>
        new(group.Code, group.Name, group.ArabicName, group.IsDeleteProtected);
}

/// <summary>
/// A measurement unit as presented to the application layer.
/// </summary>
public sealed record TestUnitDto(int Id, string Name)
{
    /// <summary>Projects a measurement unit onto its DTO.</summary>
    public static TestUnitDto FromDomain(TestUnit unit) => new(unit.Id, unit.Name);
}

/// <summary>
/// A unit conversion factor as presented to the application layer.
/// </summary>
public sealed record ConversionFactorDto(
    string TestSimpleName,
    string FromUnit,
    string ToUnit,
    decimal Factor,
    int FromDecimals,
    int ToDecimals)
{
    /// <summary>Projects a conversion factor onto its DTO.</summary>
    public static ConversionFactorDto FromDomain(ConversionFactor factor) =>
        new(
            factor.TestSimpleName,
            factor.FromUnit,
            factor.ToUnit,
            factor.Factor,
            factor.FromDecimals,
            factor.ToDecimals);
}

/// <summary>
/// An abbreviation as presented to the application layer.
/// </summary>
public sealed record AbbreviationDto(int Id, string Text, string Meaning)
{
    /// <summary>Projects an abbreviation onto its DTO.</summary>
    public static AbbreviationDto FromDomain(Abbreviation abbreviation) =>
        new(abbreviation.Id, abbreviation.Text, abbreviation.Meaning);
}

/// <summary>
/// The measurement-unit and conversion dictionaries as a single snapshot.
/// </summary>
public sealed record UnitsAndConversionsDto(
    IReadOnlyList<TestUnitDto> Units,
    IReadOnlyList<ConversionFactorDto> Conversions);
