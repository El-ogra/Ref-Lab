using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Resolves the reference range that applies to a given patient and analyte.
/// </summary>
public sealed record GetReferenceRangeForPatientQuery(
    Guid PatientId,
    int TestCode) : IRequest<ReferenceRangeDto>;

/// <summary>
/// Handles the resolution of the applicable reference range.
/// </summary>
public sealed class GetReferenceRangeForPatientHandler
    : IRequestHandler<GetReferenceRangeForPatientQuery, ReferenceRangeDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetReferenceRangeForPatientHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<ReferenceRangeDto> Handle(
        GetReferenceRangeForPatientQuery request,
        CancellationToken cancellationToken)
    {
        var patient = _db.Set<Patient>()
            .FirstOrDefault(candidate => candidate.Id.Value == request.PatientId)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var ranges = _db.Set<ReferenceRange>()
            .Where(range => range.TestCode == request.TestCode)
            .ToArray();

        var selected = ReferenceRangeSelector.Select(
            ranges,
            request.TestCode,
            patient.Sex,
            patient.Age ?? 0m,
            patient.AgeUnit ?? AgeUnit.Years);

        if (selected is null)
        {
            throw new NotFoundException(nameof(ReferenceRange), request.TestCode);
        }

        return Task.FromResult(ReferenceRangeDto.FromDomain(selected));
    }
}
