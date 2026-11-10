using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Amends the descriptive attributes, the prices or the active state of a catalogue test.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability of amending a test; the test number 4 — the urine analysis — may not be renumbered or renamed.
/// </remarks>
public sealed record UpdateTestDefinitionCommand(int Code, string? Name = null, string? SimpleName = null, string? Abbreviation = null, string? ArabicName = null, string? SampleType = null, string? Description = null, decimal? PatientPrice = null, decimal? LabPrice = null, decimal? OtherLabPrice = null, bool? IsActive = null) : IRequest<TestDefinitionDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageTestCatalogue;
}

/// <summary>
/// Handles the UpdateTestDefinition use case.
/// </summary>
public sealed class UpdateTestDefinitionHandler : IRequestHandler<UpdateTestDefinitionCommand, TestDefinitionDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public UpdateTestDefinitionHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<TestDefinitionDto> Handle(UpdateTestDefinitionCommand request, CancellationToken cancellationToken)
    {
        var definition = _db.Set<TestDefinition>()
            .FirstOrDefault(candidate => candidate.Code == request.Code)
            ?? throw new NotFoundException(nameof(TestDefinition), request.Code);

        if (request.Name is not null)
        {
            definition.Rename(request.Name);
        }

        definition.UpdateDetails(
            simpleName: request.SimpleName,
            abbreviation: request.Abbreviation,
            arabicName: request.ArabicName,
            sampleType: request.SampleType,
            description: request.Description);

        if (request.PatientPrice.HasValue || request.LabPrice.HasValue || request.OtherLabPrice.HasValue)
        {
            definition.UpdatePrices(
                request.PatientPrice ?? definition.PatientPrice,
                request.LabPrice ?? definition.LabPrice,
                request.OtherLabPrice ?? definition.OtherLabPrice);
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
            {
                definition.Activate();
            }
            else
            {
                definition.Deactivate();
            }
        }

        _unitOfWork.Update(definition);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return TestDefinitionDto.FromDomain(definition);
    }
}
