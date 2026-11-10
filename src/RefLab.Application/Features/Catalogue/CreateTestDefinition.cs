using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Adds a test to the laboratory catalogue.
/// </summary>
/// <remarks>
/// Legacy evidence: the catalogue carries 609 rows across 38 columns, and the vendor guide documents the capability of adding a new test.
/// </remarks>
public sealed record CreateTestDefinitionCommand(int Code, string Name, string SimpleName, int SubGroupCode, decimal PatientPrice, decimal LabPrice, decimal OtherLabPrice, string? SampleType = null, string? ArabicName = null, string? Abbreviation = null) : IRequest<TestDefinitionDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageTestCatalogue;
}

/// <summary>
/// Handles the CreateTestDefinition use case.
/// </summary>
public sealed class CreateTestDefinitionHandler : IRequestHandler<CreateTestDefinitionCommand, TestDefinitionDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public CreateTestDefinitionHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<TestDefinitionDto> Handle(CreateTestDefinitionCommand request, CancellationToken cancellationToken)
    {
        var duplicate = _db.Set<TestDefinition>().Any(definition => definition.Code == request.Code);

        if (duplicate)
        {
            throw new ConflictException($"Test {request.Code} already exists in the catalogue.");
        }

        var definition = TestDefinition.Create(
            request.Code,
            request.Name,
            request.SimpleName,
            request.SubGroupCode);

        definition.UpdatePrices(request.PatientPrice, request.LabPrice, request.OtherLabPrice);
        definition.UpdateDetails(
            abbreviation: request.Abbreviation,
            arabicName: request.ArabicName,
            sampleType: request.SampleType);

        _unitOfWork.Add(definition);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return TestDefinitionDto.FromDomain(definition);
    }
}
