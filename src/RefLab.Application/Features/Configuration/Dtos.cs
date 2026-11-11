using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;

namespace RefLab.Application.Features.Configuration;

/// <summary>
/// The laboratory settings as presented to the application layer.
/// </summary>
public sealed record LaboratorySettingsDto(
    PaperSize PaperSize,
    int ReportSequenceStart,
    bool IsBarcodeAutoCreated,
    bool IsWorkSheetPrinted,
    int HistoryDisplayMode,
    string? GlobalHeaderText,
    string? GlobalHeaderSecondaryText,
    string? ReportFooterText,
    string? LayoutText,
    string? DoctorSignature,
    string? DoctorName,
    string? ThanksText,
    string? LabLogoPath,
    string? ReportLogoPath,
    int? LogoWidthPx,
    int? LogoHeightPx,
    ReportHeadingMode HeadingMode,
    decimal LeftMargin,
    decimal RightMargin,
    decimal TopMargin,
    decimal BottomMargin,
    string? BackgroundColour,
    string? DefaultAccountType,
    string? ConnectionReceiptHeader,
    string? ConnectionReceiptFooter,
    IReadOnlyDictionary<string, string> Printers)
{
    /// <summary>Projects the laboratory settings onto its DTO.</summary>
    public static LaboratorySettingsDto FromDomain(LaboratorySettings settings) =>
        new(
            settings.PaperSize,
            settings.ReportSequenceStart,
            settings.IsBarcodeAutoCreated,
            settings.IsWorkSheetPrinted,
            settings.HistoryDisplayMode,
            settings.GlobalHeaderText,
            settings.GlobalHeaderSecondaryText,
            settings.ReportFooterText,
            settings.LayoutText,
            settings.DoctorSignature,
            settings.DoctorName,
            settings.ThanksText,
            settings.LabLogoPath,
            settings.ReportLogoPath,
            settings.LogoWidthPx,
            settings.LogoHeightPx,
            settings.HeadingMode,
            settings.LeftMargin,
            settings.RightMargin,
            settings.TopMargin,
            settings.BottomMargin,
            settings.BackgroundColour,
            settings.DefaultAccountType,
            settings.ConnectionReceiptHeader,
            settings.ConnectionReceiptFooter,
            settings.Printers.ToDictionary(pair => pair.Key.ToString(), pair => pair.Value));
}
