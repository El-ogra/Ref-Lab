using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.Enums;
using RefLab.Domain.SendOut;

namespace RefLab.Application.Features.SendOut;

/// <summary>
/// Records that a sample was split or withdrawn.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability of splitting and withdrawing samples, recorded against the microbiology tables.
/// </remarks>
public sealed record SplitOrWithdrawSampleCommand(Guid OrderTestId, SampleDispositionAction Action, string? DestinationLabName = null) : IRequest<SampleDispositionDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageAccounting;
}

/// <summary>
/// Handles the SplitOrWithdrawSample use case.
/// </summary>
public sealed class SplitOrWithdrawSampleHandler : IRequestHandler<SplitOrWithdrawSampleCommand, SampleDispositionDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    /// <summary>Initialises a new handler.</summary>
    public SplitOrWithdrawSampleHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IClock clock)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }
    /// <inheritdoc />
    public async Task<SampleDispositionDto> Handle(SplitOrWithdrawSampleCommand request, CancellationToken cancellationToken)
    {
        var orderTest = _db.Set<OrderTest>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderTestId)
            ?? throw new NotFoundException(nameof(OrderTest), request.OrderTestId);

        var disposition = SampleDisposition.Create(
            orderTest.Id,
            request.Action,
            _clock.UtcNow,
            _currentUser.OperatorName ?? "unknown",
            request.DestinationLabName);

        _unitOfWork.Add(disposition);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return SampleDispositionDto.FromDomain(disposition);
    }
}
