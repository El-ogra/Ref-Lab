using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Configuration;
using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;

namespace RefLab.Application.Features.Configuration;

/// <summary>
/// Customises the printed report layout: the margins, the background colour and the layout text.
/// </summary>
/// <remarks>
/// Legacy evidence: the report margins are stored per profile, the background colour and the twenty colour keys are stored in the profile configuration, and the vendor guide documents the report customisation capability.
/// </remarks>
public sealed record CustomiseReportLayoutCommand(
    decimal? LeftMargin = null,
    decimal? RightMargin = null,
    decimal? TopMargin = null,
    decimal? BottomMargin = null,
    string? BackgroundColour = null,
    string? LayoutText = null,
    string? DoctorSignature = null,
    string? DoctorName = null,
    string? ThanksText = null,
    string? LabLogoPath = null,
    string? ReportLogoPath = null,
    int? LogoWidthPx = null,
    int? LogoHeightPx = null,
    ReportHeadingMode? HeadingMode = null) : IRequest<LaboratorySettingsDto>, IRequirePermission

{
    /// <inheritdoc />
    public Permission Permission => Permission.ManageSettings;
}

/// <summary>
/// Handles the CustomiseReportLayout use case.
/// </summary>
public sealed class CustomiseReportLayoutHandler : IRequestHandler<CustomiseReportLayoutCommand, LaboratorySettingsDto>
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new handler.</summary>
    public CustomiseReportLayoutHandler(
        IAppDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
    /// <inheritdoc />
    public async Task<LaboratorySettingsDto> Handle(CustomiseReportLayoutCommand request, CancellationToken cancellationToken)
    {
        var settings = _db.Set<LaboratorySettings>().FirstOrDefault();

        if (settings is null)
        {
            settings = LaboratorySettings.Create();
            _unitOfWork.Add(settings);
        }

        if (request.LeftMargin.HasValue || request.RightMargin.HasValue ||
            request.TopMargin.HasValue || request.BottomMargin.HasValue)
        {
            settings.SetMargins(
                request.LeftMargin ?? settings.LeftMargin,
                request.RightMargin ?? settings.RightMargin,
                request.TopMargin ?? settings.TopMargin,
                request.BottomMargin ?? settings.BottomMargin);
        }

        if (request.BackgroundColour is not null)
        {
            settings.SetBackgroundColour(request.BackgroundColour);
        }

        // The layout text describes the report layout. It is not the global
        // header, which carries the laboratory identity printed above the
        // report titles, so the two are kept apart.
        if (request.LayoutText is not null)
        {
            settings.SetLayoutText(request.LayoutText);
        }

        if (request.DoctorSignature is not null ||
            request.DoctorName is not null ||
            request.ThanksText is not null ||
            request.LabLogoPath is not null ||
            request.ReportLogoPath is not null ||
            request.LogoWidthPx.HasValue ||
            request.LogoHeightPx.HasValue ||
            request.HeadingMode.HasValue)
        {
            settings.SetBranding(
                request.DoctorSignature ?? settings.DoctorSignature,
                request.DoctorName ?? settings.DoctorName,
                request.ThanksText ?? settings.ThanksText,
                request.LabLogoPath ?? settings.LabLogoPath,
                request.ReportLogoPath ?? settings.ReportLogoPath,
                request.LogoWidthPx ?? settings.LogoWidthPx,
                request.LogoHeightPx ?? settings.LogoHeightPx,
                request.HeadingMode ?? settings.HeadingMode);
        }

        _unitOfWork.Update(settings);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return LaboratorySettingsDto.FromDomain(settings);
    }
}
