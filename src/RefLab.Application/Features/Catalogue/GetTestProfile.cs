using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// A test together with every reference range that applies to it.
/// </summary>
public sealed record TestProfileDto(
    TestDefinitionDto Test,
    ClinicalSubGroupDto? SubGroup,
    IReadOnlyList<ReferenceRangeDto> ReferenceRanges);

/// <summary>
/// Returns the profile of one test.
/// </summary>
public sealed record GetTestProfileQuery(int TestCode) : IRequest<TestProfileDto>;

/// <summary>
/// Handles the test profile query.
/// </summary>
public sealed class GetTestProfileHandler : IRequestHandler<GetTestProfileQuery, TestProfileDto>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetTestProfileHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<TestProfileDto> Handle(GetTestProfileQuery request, CancellationToken cancellationToken)
    {
        var definition = _db.Set<TestDefinition>()
            .FirstOrDefault(candidate => candidate.Code == request.TestCode)
            ?? throw new NotFoundException(nameof(TestDefinition), request.TestCode);

        var ranges = _db.Set<ReferenceRange>()
            .Where(range => range.TestCode == request.TestCode)
            .OrderBy(range => range.SexScope)
            .ThenBy(range => range.Id)
            .Select(range => ReferenceRangeDto.FromDomain(range))
            .ToArray();

        var subGroup = _db.Set<ClinicalSubGroup>()
            .FirstOrDefault(candidate => candidate.Code == definition.SubGroupCode);

        var profile = new TestProfileDto(
            TestDefinitionDto.FromDomain(definition),
            subGroup is null ? null : ClinicalSubGroupDto.FromDomain(subGroup),
            ranges);

        return Task.FromResult(profile);
    }
}
