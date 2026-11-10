using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Behaviors;
using RefLab.Application.Common;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.SendOut;

namespace RefLab.Application.Features.SendOut;

/// <summary>
/// Settles a referral with an external laboratory.
/// </summary>
public sealed record SettleSendOutSampleCommand(Guid SendOutRequestId) : IRequest<SendOutState>, IRequirePermission
{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageAccounting;
}

/// <summary>
/// Handles the settlement of a send-out sample.
/// </summary>
public sealed class SettleSendOutSampleHandler : IRequestHandler<SettleSendOutSampleCommand, SendOutState>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public SettleSendOutSampleHandler(IAppDbContext db, IUnitOfWork unitOfWork, IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<SendOutState> Handle(
        SettleSendOutSampleCommand request,
        CancellationToken cancellationToken)
    {
        var sendOut = _db.Set<SendOutRequest>()
            .FirstOrDefault(candidate => candidate.Id == request.SendOutRequestId)
            ?? throw new NotFoundException(nameof(SendOutRequest), request.SendOutRequestId);

        sendOut.MarkSettled(_clock.UtcNow);

        _unitOfWork.Update(sendOut);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return sendOut.State;
    }
}
