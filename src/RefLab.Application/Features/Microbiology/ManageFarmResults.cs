using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Domain.Farming;

namespace RefLab.Application.Features.Microbiology;

/// <summary>
/// Records a farm result transaction, creating the farm register entry when it is not yet known.
/// </summary>
/// <remarks>
/// Legacy evidence: the farm table carries the farm identity and its contact details, and the farm transaction table carries the date, the issued and received amounts and the note.
/// </remarks>
public sealed record ManageFarmResultsCommand(int Id, int FarmId, string FarmName, DateTimeOffset TransactionDate, decimal IssuedAmount = 0, decimal ReceivedAmount = 0, string? Note = null, string? Address = null, string? Phone = null) : IRequest<FarmTransactionDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.EnterResults;
}

/// <summary>
/// Handles the ManageFarmResults use case.
/// </summary>
public sealed class ManageFarmResultsHandler : IRequestHandler<ManageFarmResultsCommand, FarmTransactionDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ManageFarmResultsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<FarmTransactionDto> Handle(ManageFarmResultsCommand request, CancellationToken cancellationToken)
    {
        var farm = _db.Set<Farm>()
            .FirstOrDefault(candidate => candidate.Id == request.FarmId);

        if (farm is null)
        {
            farm = Farm.Create(request.FarmId, request.FarmName, request.Address, request.Phone);
            _unitOfWork.Add(farm);
        }
        else
        {
            farm.UpdateContactDetails(request.Address ?? farm.Address, request.Phone ?? farm.Phone, null, farm.Note);
            _unitOfWork.Update(farm);
        }

        var transaction = _db.Set<FarmTransaction>()
            .FirstOrDefault(candidate => candidate.Id == request.Id);

        if (transaction is null)
        {
            transaction = FarmTransaction.Create(
                request.Id,
                request.FarmId,
                request.FarmName,
                request.TransactionDate,
                request.IssuedAmount,
                request.ReceivedAmount,
                request.Note);

            _unitOfWork.Add(transaction);
        }
        else
        {
            transaction.Update(request.IssuedAmount, request.ReceivedAmount, request.Note);
            _unitOfWork.Update(transaction);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return FarmTransactionDto.FromDomain(transaction);
    }
}
