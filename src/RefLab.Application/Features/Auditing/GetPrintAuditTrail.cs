using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Features.Auditing;
using RefLab.Domain.Auditing;

namespace RefLab.Application.Features.Auditing;

/// <summary>
/// Returns the print audit trail for a requested test or for the whole laboratory.
/// </summary>
public sealed record GetPrintAuditTrailQuery(Guid? OrderTestId, PageRequest Page)
    : IRequest<PagedResult<PrintAuditDto>>;

/// <summary>
/// Handles the print audit trail query.
/// </summary>
public sealed class GetPrintAuditTrailHandler
    : IRequestHandler<GetPrintAuditTrailQuery, PagedResult<PrintAuditDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetPrintAuditTrailHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<PagedResult<PrintAuditDto>> Handle(
        GetPrintAuditTrailQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Set<PrintAudit>().AsQueryable();

        if (request.OrderTestId.HasValue)
        {
            query = query.Where(audit => audit.OrderTestId == request.OrderTestId.Value);
        }

        var totalCount = query.Count();

        var items = query
            .OrderByDescending(audit => audit.PrintedAt)
            .Skip(request.Page.Skip)
            .Take(request.Page.Take)
            .Select(audit => PrintAuditDto.FromDomain(audit))
            .ToArray();

        return Task.FromResult(new PagedResult<PrintAuditDto>(
            items,
            totalCount,
            request.Page.PageNumber,
            request.Page.PageSize));
    }
}
