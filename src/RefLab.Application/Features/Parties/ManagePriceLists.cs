using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Billing;
using RefLab.Domain.Billing;

namespace RefLab.Application.Features.Parties;

/// <summary>
/// Adds a price to a price list, or amends the price of an existing line.
/// </summary>
/// <remarks>
/// Legacy evidence: the price list holds the analyte, the price-list name and the price, and the vendor guide documents the capability of adding a new price list.
/// </remarks>
public sealed record ManagePriceListsCommand(long Id, string PriceListName, string TestSimpleName, decimal Price) : IRequest<PriceListDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManagePricing;
}

/// <summary>
/// Handles the ManagePriceLists use case.
/// </summary>
public sealed class ManagePriceListsHandler : IRequestHandler<ManagePriceListsCommand, PriceListDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ManagePriceListsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<PriceListDto> Handle(ManagePriceListsCommand request, CancellationToken cancellationToken)
    {
        var priceList = _db.Set<PriceList>()
            .FirstOrDefault(candidate => candidate.Id == request.Id);

        if (priceList is null)
        {
            priceList = PriceList.Create(request.Id, request.TestSimpleName, request.PriceListName, request.Price);
            _unitOfWork.Add(priceList);
        }
        else
        {
            priceList.SetPrice(request.Price);
            _unitOfWork.Update(priceList);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return PriceListDto.FromDomain(priceList);
    }
}
