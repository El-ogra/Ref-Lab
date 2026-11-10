using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Identity;

/// <summary>
/// Lists the operator accounts of the laboratory.
/// </summary>
/// <remarks>
/// Legacy evidence: the operator table is the laboratory's user register.
/// </remarks>
public sealed record GetOperatorsQuery(string? Name = null, bool ActiveOnly = true) : IRequest<IReadOnlyList<OperatorDto>>;


/// <summary>
/// Handles the GetOperators use case.
/// </summary>
public sealed class GetOperatorsHandler : IRequestHandler<GetOperatorsQuery, IReadOnlyList<OperatorDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetOperatorsHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<IReadOnlyList<OperatorDto>> Handle(GetOperatorsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<Operator>().AsQueryable();

        if (request.ActiveOnly)
        {
            query = query.Where(@operator => @operator.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(@operator => @operator.Name.Contains(name));
        }

        var operators = query
            .OrderBy(@operator => @operator.Name)
            .Select(@operator => OperatorDto.FromDomain(@operator))
            .ToArray();

        return Task.FromResult<IReadOnlyList<OperatorDto>>(operators);
    }
}
