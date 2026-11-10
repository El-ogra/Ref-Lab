namespace RefLab.Application.Features.Worksheets;

/// <summary>
/// One row of a work sheet.
/// </summary>
public sealed record WorkSheetRowDto(string Key, string Label, IReadOnlyList<string> Cells);

/// <summary>
/// A work sheet over a set of patients or tests in a period.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the two work-sheet capabilities
/// — a work sheet over a group of patients in a period, and a work sheet over a
/// test or test group in a period. The work-sheet content is generated from the
/// transactional tables, not from a stored aggregate.
/// </remarks>
public sealed record WorkSheetDto(
    string Title,
    DateTimeOffset From,
    DateTimeOffset To,
    string Scope,
    int RowCount,
    IReadOnlyList<WorkSheetRowDto> Rows);
