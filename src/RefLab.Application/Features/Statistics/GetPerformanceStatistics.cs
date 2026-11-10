using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Statistics;

/// <summary>
/// Computes the performance figures of the laboratory over a period: the reviewed, printed and handed-over results, the critical results and the average turnaround.
/// </summary>
/// <remarks>
/// Legacy evidence: the statistics module monitors the performance of the laboratory from the transactional tables.
/// </remarks>
public sealed record GetPerformanceStatisticsQuery(DateTimeOffset From, DateTimeOffset To) : IRequest<PerformanceStatisticsDto>;


/// <summary>
/// Handles the GetPerformanceStatistics use case.
/// </summary>
public sealed class GetPerformanceStatisticsHandler : IRequestHandler<GetPerformanceStatisticsQuery, PerformanceStatisticsDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetPerformanceStatisticsHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<PerformanceStatisticsDto> Handle(GetPerformanceStatisticsQuery request, CancellationToken cancellationToken)
    {
        var orders = _db.Set<PatientOrder>()
            .Where(order => order.RequestDate >= request.From && order.RequestDate <= request.To)
            .ToArray();

        var orderIds = orders.Select(order => order.Id).ToArray();

        var tests = _db.Set<OrderTest>()
            .Where(test => orderIds.Contains(test.OrderId))
            .ToArray();

        var printed = tests.Where(test => test.IsResultPrinted && test.PrintedAt.HasValue).ToArray();

        var turnaround = printed
            .Select(test => (test.PrintedAt!.Value - test.RequestDate).TotalMinutes)
            .ToArray();

        var averageTurnaround = turnaround.Length == 0
            ? 0m
            : decimal.Round((decimal)(turnaround.Average()), 2, MidpointRounding.AwayFromZero);

        var critical = tests.Count(test =>
            ResultClassificationPolicy.Classify(test.Result, test.ReferenceSnapshot) == Domain.Enums.ResultClassification.Critical);

        return Task.FromResult<PerformanceStatisticsDto>(new PerformanceStatisticsDto(
            request.From,
            request.To,
            tests.Count(test => test.IsResultReviewed),
            printed.Length,
            tests.Count(test => test.IsGiven),
            critical,
            averageTurnaround));
    }
}
