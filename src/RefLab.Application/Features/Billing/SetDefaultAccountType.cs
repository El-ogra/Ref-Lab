using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Configuration;
using RefLab.Domain.Configuration;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// Sets the default account type applied to the new orders of the laboratory.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the default account type of the system, covering prices, discounts, exchange rates and withdrawn samples. The value domain of the legacy account-type column was never established, so the type is carried as free text.
/// </remarks>
public sealed record SetDefaultAccountTypeCommand(string? AccountType = null) : IRequest<LaboratorySettingsDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageAccounting;
}

/// <summary>
/// Handles the SetDefaultAccountType use case.
/// </summary>
public sealed class SetDefaultAccountTypeHandler : IRequestHandler<SetDefaultAccountTypeCommand, LaboratorySettingsDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public SetDefaultAccountTypeHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<LaboratorySettingsDto> Handle(SetDefaultAccountTypeCommand request, CancellationToken cancellationToken)
    {
        var settings = _db.Set<LaboratorySettings>().FirstOrDefault();

        if (settings is null)
        {
            settings = LaboratorySettings.Create();
            _unitOfWork.Add(settings);
        }

        settings.SetDefaultAccountType(request.AccountType);

        _unitOfWork.Update(settings);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return LaboratorySettingsDto.FromDomain(settings);
    }
}
