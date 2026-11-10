using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.SendOut;

namespace RefLab.Application.Features.SendOut;

/// <summary>
/// Enrols a requested test on the send-out list, with its destination laboratory and its prices.
/// </summary>
/// <remarks>
/// Legacy evidence: a test flagged as performed by another laboratory enters the send-out list automatically, with the destination laboratory and the send-out prices.
/// </remarks>
public sealed record CreateSendOutRequestCommand(Guid OrderTestId, string DestinationLabName, decimal? OutPrice = null, decimal? OutPriceBeforeDiscount = null) : IRequest<SendOutRequestDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageAccounting;
}

/// <summary>
/// Handles the CreateSendOutRequest use case.
/// </summary>
public sealed class CreateSendOutRequestHandler : IRequestHandler<CreateSendOutRequestCommand, SendOutRequestDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public CreateSendOutRequestHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<SendOutRequestDto> Handle(CreateSendOutRequestCommand request, CancellationToken cancellationToken)
    {
        var orderTest = _db.Set<OrderTest>()
            .FirstOrDefault(candidate => candidate.Id == request.OrderTestId)
            ?? throw new NotFoundException(nameof(OrderTest), request.OrderTestId);

        var duplicate = _db.Set<SendOutRequest>()
            .Any(sendOut => sendOut.OrderTestId == orderTest.Id);

        if (duplicate)
        {
            throw new ConflictException("This test is already enrolled on the send-out list.");
        }

        var sendOut = SendOutRequest.Create(
            orderTest.Id,
            request.DestinationLabName,
            request.OutPrice,
            request.OutPriceBeforeDiscount);

        _unitOfWork.Add(sendOut);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return SendOutRequestDto.FromDomain(sendOut);
    }
}
