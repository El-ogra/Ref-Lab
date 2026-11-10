using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Billing;
using RefLab.Domain.Billing;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// Returns the referring parties of the laboratory.
/// </summary>
public sealed record GetReferringPartiesQuery(string? Name = null)
    : IRequest<IReadOnlyList<ReferringPartyDto>>;

/// <summary>
/// Handles the referring-party query.
/// </summary>
public sealed class GetReferringPartiesHandler
    : IRequestHandler<GetReferringPartiesQuery, IReadOnlyList<ReferringPartyDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetReferringPartiesHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ReferringPartyDto>> Handle(
        GetReferringPartiesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Set<ReferringParty>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(party => party.Name.Contains(name) || party.Code.Contains(name));
        }

        var parties = query
            .OrderBy(party => party.Name)
            .Select(party => ReferringPartyDto.FromDomain(party))
            .ToArray();

        return Task.FromResult<IReadOnlyList<ReferringPartyDto>>(parties);
    }
}
