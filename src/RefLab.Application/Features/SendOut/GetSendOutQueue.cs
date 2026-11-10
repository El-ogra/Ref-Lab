using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.SendOut;

namespace RefLab.Application.Features.SendOut;

/// <summary>
/// Returns the send-out queue, so that an unsettled referral stays visible.
/// </summary>
/// <remarks>
/// Legacy evidence: dispatch and payment are tracked separately, so that an unsettled referral remains listable.
/// </remarks>
public sealed record GetSendOutQueueQuery(bool PendingOnly = false, Guid? OrderTestId = null) : IRequest<IReadOnlyList<SendOutRequestDto>>;


/// <summary>
/// Handles the GetSendOutQueue use case.
/// </summary>
public sealed class GetSendOutQueueHandler : IRequestHandler<GetSendOutQueueQuery, IReadOnlyList<SendOutRequestDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>Initialises a new handler.</summary>
    public GetSendOutQueueHandler(
        IAppDbContext db)
    {
        _db = db;
    }
    /// <inheritdoc />
    public Task<IReadOnlyList<SendOutRequestDto>> Handle(GetSendOutQueueQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<SendOutRequest>().AsQueryable();

        if (request.OrderTestId.HasValue)
        {
            query = query.Where(sendOut => sendOut.OrderTestId == request.OrderTestId.Value);
        }

        if (request.PendingOnly)
        {
            query = query.Where(sendOut => !sendOut.IsSettled);
        }

        var queue = query
            .OrderBy(sendOut => sendOut.DispatchedAt ?? DateTimeOffset.MinValue)
            .Select(sendOut => SendOutRequestDto.FromDomain(sendOut))
            .ToArray();

        return Task.FromResult<IReadOnlyList<SendOutRequestDto>>(queue);
    }
}
