using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Configuration;
using RefLab.Domain.Configuration;

namespace RefLab.Application.Features.Reporting;

/// <summary>
/// Sets the paper size and the global header and footer texts of the printed report.
/// </summary>
/// <remarks>
/// Legacy evidence: the paper size is a single laboratory-wide setting, while the two global header texts carry the laboratory identity above the per-profile titles.
/// </remarks>
public sealed record ConfigureReportHeaderFooterCommand(PaperSize? PaperSize = null, string? GlobalHeaderText = null, string? GlobalHeaderSecondaryText = null, string? FooterText = null) : IRequest<LaboratorySettingsDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageSettings;
}

/// <summary>
/// Handles the ConfigureReportHeaderFooter use case.
/// </summary>
public sealed class ConfigureReportHeaderFooterHandler : IRequestHandler<ConfigureReportHeaderFooterCommand, LaboratorySettingsDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public ConfigureReportHeaderFooterHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<LaboratorySettingsDto> Handle(ConfigureReportHeaderFooterCommand request, CancellationToken cancellationToken)
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

        if (request.GlobalHeaderText is not null || request.GlobalHeaderSecondaryText is not null)
        {
            settings.SetGlobalHeaderTexts(
                request.GlobalHeaderText ?? settings.GlobalHeaderText,
                request.GlobalHeaderSecondaryText ?? settings.GlobalHeaderSecondaryText);
        }

        // The report footer belongs to the report. It is a document element of
        // its own and must not overwrite the footer of the connection receipt,
        // which is a different document.
        if (request.FooterText is not null)
        {
            settings.SetReportFooterText(request.FooterText);
        }

        _unitOfWork.Update(settings);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return LaboratorySettingsDto.FromDomain(settings);
    }
}
