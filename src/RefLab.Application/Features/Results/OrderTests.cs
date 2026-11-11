using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Application.Features.Results;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Results;

/// <summary>
/// Orders one or more tests onto a patient order and freezes the applicable
/// reference range onto every requested test.
/// </summary>
public sealed record OrderTestsCommand(
    Guid OrderId,
    IReadOnlyList<int> TestCodes,
    IReadOnlyDictionary<int, int>? GroupMembers = null) : IRequest<IReadOnlyList<OrderTestDto>>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.OrderTests;
}

/// <summary>
/// Handles the ordering of tests.
/// </summary>
public sealed class OrderTestsHandler : IRequestHandler<OrderTestsCommand, IReadOnlyList<OrderTestDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public OrderTestsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OrderTestDto>> Handle(
        OrderTestsCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TestCodes.Count == 0)
        {
            throw new RequestValidationException(new[] { "At least one test must be requested." });
        }

        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), request.OrderId);

        var patient = _db.Set<Patient>()
            .FirstOrDefault(candidate => candidate.Id == order.PatientId)
            ?? throw new NotFoundException(nameof(Patient), order.PatientId.Value);

        var definitions = _db.Set<TestDefinition>()
            .Where(definition => request.TestCodes.Contains(definition.Code))
            .ToArray();

        if (definitions.Length != request.TestCodes.Distinct().Count())
        {
            var found = definitions.Select(definition => definition.Code).ToArray();
            var missing = request.TestCodes.Where(code => !found.Contains(code)).ToArray();

            throw new NotFoundException(nameof(TestDefinition), string.Join(", ", missing));
        }

        var ranges = _db.Set<ReferenceRange>()
            .Where(range => request.TestCodes.Contains(range.TestCode))
            .ToArray();

        var now = _clock.UtcNow;
        var operatorName = _currentUser.OperatorName ?? "unknown";
        var arrangement = order.Tests.Count;

        var created = new List<OrderTest>();

        foreach (var definition in definitions.OrderBy(definition => definition.Code))
        {
            var orderTest = OrderTest.Create(
                order.Id,
                definition.Code,
                definition.SimpleName,
                definition.SubGroupCode,
                now,
                operatorName,
                arrangement++,
                request.GroupMembers?.TryGetValue(definition.Code, out var mainTestCode) == true ? mainTestCode : null);

            var range = ReferenceRangeSelector.Select(
                ranges,
                definition.Code,
                patient.Sex,
                patient.Age ?? 0m,
                patient.AgeUnit ?? AgeUnit.Years);

            if (range is not null)
            {
                orderTest.AttachReferenceSnapshot(ReferenceSnapshot.Capture(range, now));
            }

            orderTest.SetPrice(Money.Create(definition.PatientPrice, order.CurrencyCode));
            orderTest.SetReportingSnapshots(
                definition.Name,
                definition.Abbreviation,
                definition.HistoryName,
                range?.Interval.Unit,
                definition.LogGroup);

            order.AddTest(orderTest);

            _unitOfWork.Add(orderTest);
            created.Add(orderTest);
        }

        // The order's totals are a property of the tests it carries, so they
        // are recomputed whenever tests are added. The commission base is the
        // net amount of the order, because the referring party is paid on what
        // the patient actually owes.
        ApplyOrderAmounts(order);

        _unitOfWork.Update(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return created.Select(OrderTestDto.FromDomain).ToArray();
    }

    /// <summary>
    /// Recomputes the totals and the commission base of an order from the
    /// prices of the tests it carries.
    /// </summary>
    private static void ApplyOrderAmounts(PatientOrder order)
    {
        var total = Money.Create(
            order.Tests.Sum(test => test.Price?.Amount ?? 0m),
            order.CurrencyCode);

        var discount = order.Discount;

        var discounted = discount is null
            ? total
            : Money.Create(
                Math.Max(0m, total.Amount - discount.Apply(total.Amount)),
                order.CurrencyCode);

        order.SetAmounts(total, discounted, discounted, discount, order.Discount100);
    }
}
