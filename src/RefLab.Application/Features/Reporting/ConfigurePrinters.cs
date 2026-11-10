using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Configuration;
using RefLab.Domain.Configuration;

namespace RefLab.Application.Features.Reporting;

/// <summary>
/// Binds a printer to one of the six laboratory printer roles, or unbinds it.
/// </summary>
/// <remarks>
/// Legacy evidence: six printer roles are established — the report printer, an alternate report printer, the barcode printer, the ZARF printer, the work-sheet printer and the identity-card printer.
/// </remarks>
public sealed record ConfigurePrintersCommand(PrinterRole Role, string? PrinterName = null) : IRequest<LaboratorySettingsDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageSettings;
}

/// <summary>
/// Handles the ConfigurePrinters use case.
/// </summary>
public sealed class ConfigurePrintersHandler : IRequestHandler<ConfigurePrintersCommand, LaboratorySettingsDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ConfigurePrintersHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<LaboratorySettingsDto> Handle(ConfigurePrintersCommand request, CancellationToken cancellationToken)
    {
        var settings = _db.Set<LaboratorySettings>().FirstOrDefault();

        if (settings is null)
        {
            settings = LaboratorySettings.Create();
            _unitOfWork.Add(settings);
        }

        if (request.PrinterName is null)
        {
            settings.UnbindPrinter(request.Role);
        }
        else
        {
            settings.BindPrinter(request.Role, request.PrinterName);
        }

        _unitOfWork.Update(settings);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return LaboratorySettingsDto.FromDomain(settings);
    }
}
