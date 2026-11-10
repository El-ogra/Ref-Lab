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
public sealed record GetLaboratoryStatisticsQuery(DateTimeOffset From, DateTimeOffset To) : IRequest<LaboratoryStatisticsDto>;


/// <summary>
/// Handles the GetLaboratoryStatistics use case.
/// </summary>
public sealed class GetLaboratoryStatisticsHandler : IRequestHandler<GetLaboratoryStatisticsQuery, LaboratoryStatisticsDto>
{
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

        return Task.FromResult<LaboratoryStatisticsDto>(new LaboratoryStatisticsDto(
            request.From,
            request.To,
            orders.Select(order => order.PatientId).Distinct().Count(),
            orders.Length,
            tests.Length,
            byMonth,
            bySex,
            bySubGroup));
    }
}
