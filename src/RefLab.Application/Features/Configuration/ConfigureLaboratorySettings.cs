using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Configuration;
using RefLab.Domain.Configuration;

namespace RefLab.Application.Features.Configuration;

/// <summary>
/// Sets the laboratory-wide operating settings: the paper size, the report sequence origin, the barcode and work-sheet flags and the accounting default.
/// </summary>
/// <remarks>
/// Legacy evidence: the settings file carries the paper size, the report sequence origin, the barcode auto-create flag, the work-sheet printing flag, the history display mode and the default account type.
/// </remarks>
public sealed record ConfigureLaboratorySettingsCommand(PaperSize? PaperSize = null, int? ReportSequenceStart = null, bool? IsBarcodeAutoCreated = null, bool? IsWorkSheetPrinted = null, int? HistoryDisplayMode = null, string? DefaultAccountType = null) : IRequest<LaboratorySettingsDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageSettings;
}

/// <summary>
/// Handles the ConfigureLaboratorySettings use case.
/// </summary>
public sealed class ConfigureLaboratorySettingsHandler : IRequestHandler<ConfigureLaboratorySettingsCommand, LaboratorySettingsDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ConfigureLaboratorySettingsHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<LaboratorySettingsDto> Handle(ConfigureLaboratorySettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = _db.Set<LaboratorySettings>().FirstOrDefault();

        if (settings is null)
        {
            settings = LaboratorySettings.Create();
            _unitOfWork.Add(settings);
        }

        if (request.PaperSize.HasValue)
        {
            settings.SetPaperSize(request.PaperSize.Value);
        }

        if (request.ReportSequenceStart.HasValue)
        {
            settings.SetReportSequenceStart(request.ReportSequenceStart.Value);
        }

        if (request.IsBarcodeAutoCreated.HasValue)
        {
            settings.SetBarcodeAutoCreated(request.IsBarcodeAutoCreated.Value);
        }

        if (request.IsWorkSheetPrinted.HasValue)
        {
            settings.SetWorkSheetPrinted(request.IsWorkSheetPrinted.Value);
        }

        if (request.HistoryDisplayMode.HasValue)
        {
            settings.SetHistoryDisplayMode(request.HistoryDisplayMode.Value);
        }

        if (request.DefaultAccountType is not null)
        {
            settings.SetDefaultAccountType(request.DefaultAccountType);
        }

        _unitOfWork.Update(settings);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return LaboratorySettingsDto.FromDomain(settings);
    }
}
