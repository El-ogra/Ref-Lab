namespace RefLab.Application.Features.Statistics;

/// <summary>
/// The number of patients recorded in a period.
/// </summary>
public sealed record PatientCountDto(string Period, int Count);

/// <summary>
/// The laboratory statistics computed over a period.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the statistics module
/// <c>&#x0627;&#x0644;&#x0625;&#x062d;&#x0635;&#x0627;&#x0626;&#x064a;&#x0627;&#x062a; &#x0648;&#x0645;&#x062a;&#x0627;&#x0628;&#x0639;&#x0629; &#x0623;&#x062f;&#x0627;&#x0621; &#x0627;&#x0644;&#x0645;&#x0639;&#x0645;&#x0644;</c>
/// — patient counts by year sorted by month and by sex, patient counts by month,
/// the request rate of a test group and the sample count for the year.
/// The statistics are computed from the transactional tables; the legacy system
/// keeps no aggregate store.
/// </remarks>
public sealed record LaboratoryStatisticsDto(
    DateTimeOffset From,
    DateTimeOffset To,
    int PatientCount,
    int OrderCount,
    int RequestedTestCount,
    IReadOnlyList<PatientCountDto> ByMonth,
    IReadOnlyList<PatientCountDto> BySex,
    IReadOnlyList<PatientCountDto> BySubGroup,
    IReadOnlyList<PatientCountDto> ByYear,
    IReadOnlyList<PatientCountDto> ByMonthAndSex,
    IReadOnlyList<TestGroupRateDto> RequestRates,
    IReadOnlyList<SampleCountDto> SamplesByYear);

/// <summary>
/// The request rate of a clinical test group.
/// </summary>
/// <remarks>
/// The reference system counts the request rate of a test group: how many
/// requests the group received, against how many patients asked for them.
/// </remarks>
public sealed record TestGroupRateDto(
    string SubGroupCode,
    int RequestCount,
    int PatientCount,
    decimal RequestsPerPatient);

/// <summary>
/// The number of samples a laboratory received in a year.
/// </summary>
public sealed record SampleCountDto(string Year, int SampleCount);

/// <summary>
/// The performance monitoring figures of the laboratory.
/// </summary>
public sealed record PerformanceStatisticsDto(
    DateTimeOffset From,
    DateTimeOffset To,
    int ReviewedResultCount,
    int PrintedResultCount,
    int HandedOverResultCount,
    int CriticalResultCount,
    decimal AverageTurnaroundMinutes);
