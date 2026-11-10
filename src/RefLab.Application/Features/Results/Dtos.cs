using RefLab.Application.Common;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;

namespace RefLab.Application.Features.Results;

/// <summary>
/// One critical limit of an analyte, as consulted on the method sheet.
/// </summary>
public sealed record CriticalValueDto(
    decimal? LowCriticalLimit,
    decimal? HighCriticalLimit,
    string? LowText,
    string? HighText,
    string? ExceptionText,
    string? Comment);

/// <summary>
/// The method sheet of an analyte.
/// </summary>
public sealed record MethodSheetDto(
    int TestCode,
    string TestName,
    string? SampleType,
    string? Description,
    string? Effect,
    string? Usage,
    IReadOnlyList<CriticalValueDto> CriticalValues);

/// <summary>
/// A derived clinical value.
/// </summary>
public sealed record DerivationResultDto(string Name, decimal Value, string? Unit);

/// <summary>
/// One line of a consolidated report.
/// </summary>
public sealed record ConsolidatedReportRowDto(
    string PatientName,
    string? LegacyIdentifier,
    string TestName,
    string? Value,
    string? Unit,
    string? Flag);

/// <summary>
/// A consolidated report over several orders.
/// </summary>
public sealed record ConsolidatedReportDto(
    string Title,
    DateTimeOffset From,
    DateTimeOffset To,
    int OrderCount,
    IReadOnlyList<ConsolidatedReportRowDto> Rows,
    RenderedDocumentDto? Document);

/// <summary>
/// An empty report template.
/// </summary>
public sealed record EmptyReportDto(
    string Title,
    string? Subtitle,
    IReadOnlyList<string> ColumnNames,
    RenderedDocumentDto? Document);

/// <summary>
/// The invoice of a patient order.
/// </summary>
public sealed record InvoiceDto(
    Guid OrderId,
    string? LegacyIdentifier,
    string PatientName,
    string CurrencyCode,
    decimal TotalPrice,
    decimal Discount,
    decimal AmountDue,
    IReadOnlyList<string> Lines,
    RenderedDocumentDto? Document);

/// <summary>
/// The identity card of a patient.
/// </summary>
public sealed record IdCardDto(
    Guid PatientId,
    string PatientName,
    Sex Sex,
    decimal? Age,
    string? Address,
    string? Phone,
    string? Barcode,
    RenderedDocumentDto? Document);
