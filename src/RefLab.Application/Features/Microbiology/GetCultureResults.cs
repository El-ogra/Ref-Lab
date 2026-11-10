using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Microbiology;

namespace RefLab.Application.Features.Microbiology;

/// <summary>
/// Returns the culture results, for one patient or for one culture identifier.
/// </summary>
/// <remarks>
/// Legacy evidence: the culture table is keyed by the patient and by the request date, and by the culture identifier.
/// </remarks>
public sealed record GetCultureResultsQuery(Guid? PatientId = null, string? CultureId = null) : IRequest<IReadOnlyList<CultureResultDto>>;


/// <summary>
/// Handles the GetCultureResults use case.
/// </summary>
public sealed class GetCultureResultsHandler : IRequestHandler<GetCultureResultsQuery, IReadOnlyList<CultureResultDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetCultureResultsHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<IReadOnlyList<CultureResultDto>> Handle(GetCultureResultsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<CultureResult>().AsQueryable();

        if (request.PatientId.HasValue)
        {
            query = query.Where(culture => culture.PatientId.Value == request.PatientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.CultureId))
        {
            var cultureId = request.CultureId.Trim();
            query = query.Where(culture => culture.CultureId == cultureId);
        }

        var cultures = query
            .OrderByDescending(culture => culture.RequestDate)
            .Select(culture => CultureResultDto.FromDomain(culture))
            .ToArray();

        return Task.FromResult<IReadOnlyList<CultureResultDto>>(cultures);
    }
}
