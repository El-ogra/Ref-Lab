using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Configuration;
using RefLab.Application.Features.Reporting;
using RefLab.Domain.Configuration;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the report setup screen.
/// </summary>
/// <remarks>
/// The screen binds the six printer roles, the paper size, the report margins
/// and the global header and footer texts. It is the screen the reference
/// product reached through the report setup window.
/// </remarks>
public sealed partial class ReportSetupViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public ReportSetupViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Report setup";

    /// <summary>The printer bindings.</summary>
    public ObservableCollection<PrinterBinding> Printers { get; } = new();

    /// <summary>The printer roles.</summary>
    public IReadOnlyList<PrinterRole> PrinterRoles { get; } = Enum.GetValues<PrinterRole>();

    /// <summary>The paper sizes.</summary>
    public IReadOnlyList<PaperSize> PaperSizes { get; } = Enum.GetValues<PaperSize>();

    /// <summary>The role whose printer is being bound.</summary>
    [ObservableProperty]
    private PrinterRole _selectedRole = PrinterRole.Report;

    /// <summary>The name of the printer bound to the selected role.</summary>
    [ObservableProperty]
    private string? _printerName;

    /// <summary>The paper size applied to every printed report.</summary>
    [ObservableProperty]
    private PaperSize _paperSize = PaperSize.A4;

    /// <summary>The first global header text.</summary>
    [ObservableProperty]
    private string? _globalHeaderText;

    /// <summary>The second global header text.</summary>
    [ObservableProperty]
    private string? _globalHeaderSecondaryText;

    /// <summary>The footer text.</summary>
    [ObservableProperty]
    private string? _footerText;

    /// <summary>The left report margin.</summary>
    [ObservableProperty]
    private decimal _leftMargin;

    /// <summary>The right report margin.</summary>
    [ObservableProperty]
    private decimal _rightMargin;

    /// <summary>The top report margin.</summary>
    [ObservableProperty]
    private decimal _topMargin;

    /// <summary>The bottom report margin.</summary>
    [ObservableProperty]
    private decimal _bottomMargin;

    /// <summary>The background colour of the report.</summary>
    [ObservableProperty]
    private string? _backgroundColour;

    /// <summary>The layout text of the report.</summary>
    [ObservableProperty]
    private string? _layoutText;

    /// <summary>Reloads the settings.</summary>
    /// <remarks>
    /// The Application layer exposes no read-only settings query among the
    /// seventy use cases. The configure use case changes nothing when it is
    /// sent without an argument and returns the current settings, so it is
    /// used as the read path of this screen.
    /// </remarks>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var settings = await _executor.SendAsync(new ConfigureLaboratorySettingsCommand());

            Apply(settings);
        }).ConfigureAwait(true);
    }

    /// <summary>Binds a printer to the selected role.</summary>
    [RelayCommand]
    private async Task BindPrinterAsync()
    {
        await RunAsync(async () =>
        {
            var settings = await _executor.SendAsync(new ConfigurePrintersCommand(SelectedRole, PrinterName));

            Apply(settings);

            StatusMessage = $"The {SelectedRole} printer was bound.";
        }).ConfigureAwait(true);
    }

    /// <summary>Saves the paper size and the global header and footer texts.</summary>
    [RelayCommand]
    private async Task SaveHeaderFooterAsync()
    {
        await RunAsync(async () =>
        {
            var settings = await _executor.SendAsync(new ConfigureReportHeaderFooterCommand(
                PaperSize,
                GlobalHeaderText,
                GlobalHeaderSecondaryText,
                FooterText));

            Apply(settings);

            StatusMessage = "The header and footer were saved.";
        }).ConfigureAwait(true);
    }

    /// <summary>Saves the margins, the background colour and the layout text.</summary>
    [RelayCommand]
    private async Task SaveLayoutAsync()
    {
        await RunAsync(async () =>
        {
            var settings = await _executor.SendAsync(new CustomiseReportLayoutCommand(
                LeftMargin,
                RightMargin,
                TopMargin,
                BottomMargin,
                BackgroundColour,
                LayoutText));

            Apply(settings);

            StatusMessage = "The report layout was saved.";
        }).ConfigureAwait(true);
    }

    private void Apply(LaboratorySettingsDto settings)
    {
        PaperSize = settings.PaperSize;
        GlobalHeaderText = settings.GlobalHeaderText;
        GlobalHeaderSecondaryText = settings.GlobalHeaderSecondaryText;
        FooterText = settings.ConnectionReceiptFooter;
        LeftMargin = settings.LeftMargin;
        RightMargin = settings.RightMargin;
        TopMargin = settings.TopMargin;
        BottomMargin = settings.BottomMargin;
        BackgroundColour = settings.BackgroundColour;

        Printers.Clear();

        foreach (var role in PrinterRoles)
        {
            settings.Printers.TryGetValue(role.ToString(), out var printer);

            Printers.Add(new PrinterBinding(role, printer));
        }
    }

    /// <summary>One printer binding.</summary>
    /// <param name="Role">The printer role.</param>
    /// <param name="PrinterName">The name of the bound printer.</param>
    public sealed record PrinterBinding(PrinterRole Role, string? PrinterName);
}
