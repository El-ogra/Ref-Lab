using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Features.Catalogue;

/// <summary>
/// Returns a page of the test catalogue.
/// </summary>
public sealed record GetTestCatalogueQuery(
    string? Name = null,
    int? SubGroupCode = null,
    bool ActiveOnly = true,
    PageRequest? Page = null) : IRequest<PagedResult<TestDefinitionDto>>;

/// <summary>
/// Handles the test catalogue query.
/// </summary>
public sealed class GetTestCatalogueHandler : IRequestHandler<GetTestCatalogueQuery, PagedResult<TestDefinitionDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetTestCatalogueHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<PagedResult<TestDefinitionDto>> Handle(
        GetTestCatalogueQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page ?? PageRequest.First();

        var query = _db.Set<TestDefinition>().AsQueryable();

        if (request.ActiveOnly)
        {
            query = query.Where(definition => definition.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(definition =>
                definition.Name.Contains(name) || definition.SimpleName.Contains(name));
        }

        if (request.SubGroupCode.HasValue)
        {
            query = query.Where(definition => definition.SubGroupCode == request.SubGroupCode.Value);
        }

        var totalCount = query.Count();

        var items = query
            .OrderBy(definition => definition.SubGroupCode)
            .ThenBy(definition => definition.ArrangementNumber)
            .ThenBy(definition => definition.Code)
            .Skip(page.Skip)
            .Take(page.Take)
            .Select(definition => TestDefinitionDto.FromDomain(definition))
            .ToArray();

        return Task.FromResult(new PagedResult<TestDefinitionDto>(
            items,
            totalCount,
            page.PageNumber,
            page.PageSize));
    }
}
