using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Returns one patient order with its requested tests.
/// </summary>
public sealed record GetPatientOrderQuery(Guid OrderId) : IRequest<PatientOrderDto>;

/// <summary>
/// Handles the patient order query.
/// </summary>
public sealed class GetPatientOrderHandler : IRequestHandler<GetPatientOrderQuery, PatientOrderDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetPatientOrderHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<PatientOrderDto> Handle(GetPatientOrderQuery request, CancellationToken cancellationToken)
    {
        var order = _db.Set<PatientOrder>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderId)
            ?? throw new NotFoundException(nameof(PatientOrder), request.OrderId);

        var tests = _db.Set<OrderTest>()
            .Where(test => test.OrderId == order.Id)
            .OrderBy(test => test.ArrangementNumber)
            .ToArray();

        var projected = PatientOrderDto.FromDomain(order);

        var withTests = projected with { Tests = tests.Select(OrderTestDto.FromDomain).ToArray() };

        return Task.FromResult(withTests);
    }
}
