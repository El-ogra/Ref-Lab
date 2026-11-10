using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Returns the follow-up register: every laboratory case with its progress and the next action required.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability of following up the recorded cases, amending them, entering the results and printing the reports.
/// </remarks>
public sealed record FollowUpCaseQuery(DateTimeOffset? From = null, DateTimeOffset? To = null, bool OpenOnly = false) : IRequest<IReadOnlyList<CaseFollowUpDto>>;


/// <summary>
/// Handles the FollowUpCase use case.
/// </summary>
public sealed class FollowUpCaseHandler : IRequestHandler<FollowUpCaseQuery, IReadOnlyList<CaseFollowUpDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public FollowUpCaseHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<IReadOnlyList<CaseFollowUpDto>> Handle(FollowUpCaseQuery request, CancellationToken cancellationToken)
    {
        var orders = _db.Set<PatientOrder>().AsQueryable();

        if (request.From.HasValue)
        {
            orders = orders.Where(order => order.RequestDate >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            orders = orders.Where(order => order.RequestDate <= request.To.Value);
        }

        var orderList = orders.ToArray();

        var patients = _db.Set<Patient>().ToArray();
        var tests = _db.Set<OrderTest>().ToArray();

        var cases = new List<CaseFollowUpDto>();

        foreach (var order in orderList)
        {
            var orderTests = tests.Where(test => test.OrderId == order.Id).ToArray();

            var reviewed = orderTests.Count(test => test.IsResultReviewed);
            var printed = orderTests.Count(test => test.IsResultPrinted);
            var handedOver = orderTests.Count(test => test.IsGiven);

            var isComplete = orderTests.Length > 0 && handedOver == orderTests.Length;

            if (request.OpenOnly && isComplete)
            {
                continue;
            }

            var patientName = patients
                .FirstOrDefault(patient => patient.Id == order.PatientId)?.Name ?? string.Empty;

            cases.Add(new CaseFollowUpDto(
                order.Id,
                order.PatientId.Value,
                patientName,
                order.LegacyIdentifier?.Value,
                order.RequestDate,
                order.Status,
                orderTests.Length,
                reviewed,
                printed,
                handedOver,
                isComplete,
                DetermineNextAction(orderTests.Length, reviewed, printed, handedOver)));
        }

        return Task.FromResult<IReadOnlyList<CaseFollowUpDto>>(cases.OrderByDescending(entry => entry.RequestDate).ToArray());
    }

    private static string DetermineNextAction(int requested, int reviewed, int printed, int handedOver)
    {
        if (requested == 0)
        {
            return "Order the requested tests.";
        }

        if (reviewed < requested)
        {
            return "Enter and confirm the outstanding results.";
        }

        if (printed < reviewed)
        {
            return "Print the laboratory report.";
        }

        if (handedOver < printed)
        {
            return "Hand over the results and settle the account.";
        }

        return "The case is complete.";
    }
}
