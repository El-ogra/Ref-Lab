using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Creates a clinical test group — a named set of tests — from the tests the
/// operator has selected.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the reference product carries a custom test-group editor
/// beside the clinical sub-group editor. It lists the groups that exist, the
/// tests that belong to the selected group, and the tests the operator has
/// picked for the group being composed; the group's membership is then saved.
/// </para>
/// <para>
/// The membership of a test group is the clinical sub-group the test belongs
/// to, so a group is created by giving the selected tests a new sub-group
/// code. The code is allocated as the next free one, so that the operator never
/// has to invent one.
/// </para>
/// </remarks>
public sealed record CreateTestGroupCommand(
    string Name,
    IReadOnlyList<int> TestCodes,
    string? ArabicName = null) : IRequest<ClinicalSubGroupDto>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageTestCatalogue;
}

/// <summary>
/// Handles the CreateTestGroup use case.
/// </summary>
public sealed class CreateTestGroupHandler : IRequestHandler<CreateTestGroupCommand, ClinicalSubGroupDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public CreateTestGroupHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ClinicalSubGroupDto> Handle(
        CreateTestGroupCommand request,
        CancellationToken cancellationToken)
    {
        var groups = _db.Set<ClinicalSubGroup>().ToArray();

        var code = groups.Length == 0 ? 1 : groups.Max(group => group.Code) + 1;

        var group = ClinicalSubGroup.Create(code, request.Name, request.ArabicName);

        _unitOfWork.Add(group);

        var definitions = _db.Set<TestDefinition>().ToArray();

        foreach (var test in definitions.Where(test => request.TestCodes.Contains(test.Code)))
        {
            test.MoveTo(code, test.ArrangementNumber);
            _unitOfWork.Update(test);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ClinicalSubGroupDto.FromDomain(group);
    }
}
