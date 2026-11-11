using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Statistics;

/// <summary>
/// Computes the laboratory statistics over a period: the patient counts, the order count and the request rate of each clinical sub-group.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the patient count by year sorted by month and by sex, the patient count by month, the request rate of a test group and the sample count for the year. The statistics are computed from the transactional tables; the legacy system keeps no aggregate store.
/// </remarks>
public sealed record GetLaboratoryStatisticsQuery(
    DateTimeOffset From,
    DateTimeOffset To,
    int? SubGroupCode = null,
    int? Year = null) : IRequest<LaboratoryStatisticsDto>;


/// <summary>
/// Handles the GetLaboratoryStatistics use case.
/// </summary>
public sealed class GetLaboratoryStatisticsHandler : IRequestHandler<GetLaboratoryStatisticsQuery, LaboratoryStatisticsDto>
{
    /// <summary>The label used when the sex of a patient cannot be read.</summary>
    private const string SexUnknown = "Unknown";

    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetLaboratoryStatisticsHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<LaboratoryStatisticsDto> Handle(GetLaboratoryStatisticsQuery request, CancellationToken cancellationToken)
    {
        var orders = _db.Set<PatientOrder>()
            .Where(order => order.RequestDate >= request.From && order.RequestDate <= request.To)
            .ToArray();

        var orderIds = orders.Select(order => order.Id).ToArray();

        var tests = _db.Set<OrderTest>()
            .Where(test => orderIds.Contains(test.OrderId))
            .ToArray();

        var patients = _db.Set<Patient>().ToArray();

        var byMonth = orders
            .GroupBy(order => order.RequestDate.ToString("yyyy-MM", System.Globalization.CultureInfo.InvariantCulture))
            .OrderBy(group => group.Key)
            .Select(group => new PatientCountDto(group.Key, group.Count()))
            .ToArray();

        var bySex = patients
            .Where(patient => orders.Any(order => order.PatientId == patient.Id))
            .GroupBy(patient => patient.Sex.ToString())
            .OrderBy(group => group.Key)
            .Select(group => new PatientCountDto(group.Key, group.Count()))
            .ToArray();

        var bySubGroup = tests
            .GroupBy(test => test.SubGroupCode)
            .OrderBy(group => group.Key)
            .Select(group => new PatientCountDto(group.Key.ToString(System.Globalization.CultureInfo.InvariantCulture), group.Count()))
            .ToArray();

        // The patient count for a year, and the same count sorted by month and
        // by sex.
        var byYear = orders
            .GroupBy(order => order.RequestDate.Year)
            .OrderBy(group => group.Key)
            .Select(group => new PatientCountDto(
                group.Key.ToString(System.Globalization.CultureInfo.InvariantCulture),
                group.Select(order => order.PatientId).Distinct().Count()))
            .ToArray();

        var sexByPatient = patients.ToDictionary(patient => patient.Id, patient => patient.Sex.ToString());

        var byMonthAndSex = orders
            .GroupBy(order => (
                Month: order.RequestDate.ToString("yyyy-MM", System.Globalization.CultureInfo.InvariantCulture),
                Sex: sexByPatient.GetValueOrDefault(order.PatientId, SexUnknown)))
            .OrderBy(group => group.Key.Month, StringComparer.Ordinal)
            .ThenBy(group => group.Key.Sex, StringComparer.Ordinal)
            .Select(group => new PatientCountDto(
                group.Key.Month + " " + group.Key.Sex,
                group.Select(order => order.PatientId).Distinct().Count()))
            .ToArray();

        // The request rate of a test group: how many requests the group
        // received against how many patients asked for them.
        var groupTests = request.SubGroupCode.HasValue
            ? tests.Where(test => test.SubGroupCode == request.SubGroupCode.Value).ToArray()
            : tests;

        var requestRates = groupTests
            .GroupBy(test => test.SubGroupCode)
            .OrderBy(group => group.Key)
            .Select(group =>
            {
                var patients_ = group.Select(test => test.OrderId).Distinct().Count();

                var rate = patients_ == 0
                    ? 0m
                    : decimal.Round((decimal)group.Count() / patients_, 2, MidpointRounding.AwayFromZero);

                return new TestGroupRateDto(
                    group.Key.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    group.Count(),
                    patients_,
                    rate);
            })
            .ToArray();

        // The number of samples the laboratory received in each year. A sample
        // is a requested test, so the year's samples are the tests of the
        // orders of that year.
        var testsByOrder = tests
            .GroupBy(test => test.OrderId)
            .ToDictionary(group => group.Key, group => group.Count());

        var samplesByYear = orders
            .GroupBy(order => order.RequestDate.Year)
            .OrderBy(group => group.Key)
            .Select(group => new SampleCountDto(
                group.Key.ToString(System.Globalization.CultureInfo.InvariantCulture),
                group.Sum(order => testsByOrder.GetValueOrDefault(order.Id, 0))))
            .ToArray();

        return Task.FromResult<LaboratoryStatisticsDto>(new LaboratoryStatisticsDto(
            request.From,
            request.To,
            orders.Select(order => order.PatientId).Distinct().Count(),
            orders.Length,
            tests.Length,
            byMonth,
            bySex,
            bySubGroup,
            byYear,
            byMonthAndSex,
            requestRates,
            samplesByYear));
    }
}
