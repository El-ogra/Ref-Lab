using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Application.Behaviors;
using RefLab.Domain.Identity;
using RefLab.Application.Features.Configuration;
using RefLab.Domain.Configuration;

namespace RefLab.Application.Features.Configuration;

/// <summary>
/// Customises the printed report layout: the margins, the background colour and the layout text.
/// </summary>
/// <remarks>
/// Legacy evidence: the report margins are stored per profile, the background colour and the twenty colour keys are stored in the profile configuration, and the vendor guide documents the report customisation capability.
/// </remarks>
public sealed record CustomiseReportLayoutCommand(decimal? LeftMargin = null, decimal? RightMargin = null, decimal? TopMargin = null, decimal? BottomMargin = null, string? BackgroundColour = null, string? LayoutText = null) : IRequest<LaboratorySettingsDto>, IRequirePermission

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

        if (request.LayoutText is not null)
        {
            settings.SetGlobalHeaderTexts(request.LayoutText, settings.GlobalHeaderSecondaryText);
        }

        _unitOfWork.Update(settings);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return LaboratorySettingsDto.FromDomain(settings);
    }
}
