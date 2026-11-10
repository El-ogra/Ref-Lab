using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Configuration;
using RefLab.Domain.Configuration;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the laboratory settings screen.
/// </summary>
public sealed partial class LaboratorySettingsViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public LaboratorySettingsViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Laboratory settings";

    /// <summary>The paper sizes.</summary>
    public IReadOnlyList<PaperSize> PaperSizes { get; } = Enum.GetValues<PaperSize>();

    /// <summary>The paper size applied to every printed report.</summary>
    [ObservableProperty]
    private PaperSize _paperSize = PaperSize.A4;

    /// <summary>The origin of the report sequence numbers.</summary>
    [ObservableProperty]
    private int _reportSequenceStart = 1984;

    /// <summary>Whether a barcode is created for every order.</summary>
    [ObservableProperty]
    private bool _isBarcodeAutoCreated = true;

    /// <summary>Whether work sheets are printed automatically.</summary>
    [ObservableProperty]
    private bool _isWorkSheetPrinted = true;

    /// <summary>The medical-history display mode.</summary>
    [ObservableProperty]
    private int _historyDisplayMode;

    /// <summary>The default account type applied to new orders.</summary>
    [ObservableProperty]
    private string? _defaultAccountType;

    /// <summary>The header text of the connection receipt.</summary>
    [ObservableProperty]
    private string? _connectionReceiptHeader;

    /// <summary>The footer text of the connection receipt.</summary>
    [ObservableProperty]
    private string? _connectionReceiptFooter;

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

    /// <summary>Saves the laboratory settings.</summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        await RunAsync(async () =>
        {
            var settings = await _executor.SendAsync(new ConfigureLaboratorySettingsCommand(
                PaperSize,
                ReportSequenceStart,
                IsBarcodeAutoCreated,
                IsWorkSheetPrinted,
                HistoryDisplayMode,
                DefaultAccountType));

            Apply(settings);

            StatusMessage = "The laboratory settings were saved.";
        }).ConfigureAwait(true);
    }

    /// <summary>Saves the connection receipt.</summary>
    [RelayCommand]
    private async Task SaveConnectionReceiptAsync()
    {
        await RunAsync(async () =>
        {
            var settings = await _executor.SendAsync(new ConfigureConnectionReceiptCommand(
                ConnectionReceiptHeader,
                ConnectionReceiptFooter));

            Apply(settings);

            StatusMessage = "The connection receipt was saved.";
        }).ConfigureAwait(true);
    }

    private void Apply(LaboratorySettingsDto settings)
    {
        PaperSize = settings.PaperSize;
        ReportSequenceStart = settings.ReportSequenceStart;
        IsBarcodeAutoCreated = settings.IsBarcodeAutoCreated;
        IsWorkSheetPrinted = settings.IsWorkSheetPrinted;
        HistoryDisplayMode = settings.HistoryDisplayMode;
        DefaultAccountType = settings.DefaultAccountType;
        ConnectionReceiptHeader = settings.ConnectionReceiptHeader;
        ConnectionReceiptFooter = settings.ConnectionReceiptFooter;
    }
}
