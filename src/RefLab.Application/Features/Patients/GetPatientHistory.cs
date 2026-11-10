using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// Returns the medical history sheets of a patient.
/// </summary>
public sealed record GetPatientHistoryQuery(Guid PatientId) : IRequest<IReadOnlyList<PatientHistoryDto>>;

/// <summary>
/// Handles the patient history query.
/// </summary>
public sealed class GetPatientHistoryHandler
    : IRequestHandler<GetPatientHistoryQuery, IReadOnlyList<PatientHistoryDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetPatientHistoryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<PatientHistoryDto>> Handle(
        GetPatientHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var sheets = _db.Set<PatientHistory>()
            .Where(sheet => sheet.PatientId.Value == request.PatientId)
            .OrderBy(sheet => sheet.Selector)
            .Select(sheet => PatientHistoryDto.FromDomain(sheet))
            .ToArray();

        return Task.FromResult<IReadOnlyList<PatientHistoryDto>>(sheets);
    }
}
