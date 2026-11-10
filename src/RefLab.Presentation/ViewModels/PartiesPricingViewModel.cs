using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Billing;
using RefLab.Application.Features.Parties;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the parties and pricing screen.
/// </summary>
public sealed partial class PartiesPricingViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public PartiesPricingViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Parties and pricing";

    /// <summary>The referring parties.</summary>
    public ObservableCollection<ReferringPartyDto> ReferringParties { get; } = new();

    /// <summary>The treating physicians.</summary>
    public ObservableCollection<TreatingPhysicianDto> TreatingPhysicians { get; } = new();

    /// <summary>The price lists.</summary>
    public ObservableCollection<PriceListDto> PriceLists { get; } = new();

    /// <summary>The referring party being maintained.</summary>
    [ObservableProperty]
    private ReferringPartyDto? _selectedParty;

    /// <summary>The treating physician being maintained.</summary>
    [ObservableProperty]
    private TreatingPhysicianDto? _selectedPhysician;

    /// <summary>The identifier of the treating physician being maintained.</summary>
    [ObservableProperty]
    private int _physicianId = 1;

    /// <summary>The code of the treating physician being maintained.</summary>
    [ObservableProperty]
    private string _physicianCode = string.Empty;

    /// <summary>The name of the treating physician being maintained.</summary>
    [ObservableProperty]
    private string _physicianName = string.Empty;

    /// <summary>The telephone number of the treating physician being maintained.</summary>
    [ObservableProperty]
    private string? _physicianPhone;

    /// <summary>The identifier of the price list entry being maintained.</summary>
    [ObservableProperty]
    private long _priceListId = 1;

    /// <summary>The name of the price list being maintained.</summary>
    [ObservableProperty]
    private string _priceListName = string.Empty;

    /// <summary>The test the price list entry refers to.</summary>
    [ObservableProperty]
    private string _priceListTestSimpleName = string.Empty;

    /// <summary>The price of the price list entry.</summary>
    [ObservableProperty]
    private decimal _priceListPrice;

    /// <summary>The destination directory of the printed price list.</summary>
    [ObservableProperty]
    private string _destinationDirectory = string.Empty;

    /// <summary>The file the last print produced.</summary>
    [ObservableProperty]
    private string? _lastPrintedPath;

    /// <summary>Reloads the listings.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var parties = await _executor.SendAsync(new GetReferringPartiesQuery());

            ReferringParties.Clear();

            foreach (var party in parties)
            {
                ReferringParties.Add(party);
            }
        }).ConfigureAwait(true);
    }

    /// <summary>Maintains a treating physician.</summary>
    [RelayCommand]
    private async Task SavePhysicianAsync()
    {
        if (string.IsNullOrWhiteSpace(PhysicianName))
        {
            ErrorMessage = "A physician name is required.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new ManageTreatingPhysiciansCommand(
                PhysicianId,
                PhysicianCode,
                PhysicianName,
                PhysicianPhone));

            StatusMessage = "The treating physician was saved.";
        }).ConfigureAwait(true);
    }

    /// <summary>Maintains a price list entry.</summary>
    [RelayCommand]
    private async Task SavePriceListAsync()
    {
        if (string.IsNullOrWhiteSpace(PriceListName) || string.IsNullOrWhiteSpace(PriceListTestSimpleName))
        {
            ErrorMessage = "A price list name and a test are required.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new ManagePriceListsCommand(
                PriceListId,
                PriceListName,
                PriceListTestSimpleName,
                PriceListPrice));

            StatusMessage = "The price list entry was saved.";
        }).ConfigureAwait(true);
    }

    /// <summary>Prints a price list.</summary>
    [RelayCommand]
    private async Task PrintPriceListAsync()
    {
        if (string.IsNullOrWhiteSpace(PriceListName))
        {
            ErrorMessage = "A price list name is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(DestinationDirectory))
        {
            ErrorMessage = "Choose a destination directory first.";
            return;
        }

        await RunAsync(async () =>
        {
            var printed = await _executor.SendAsync(new PrintPriceListCommand(PriceListName, DestinationDirectory));

            LastPrintedPath = printed.FullPath;

            StatusMessage = $"The price list was written to {LastPrintedPath}.";
        }).ConfigureAwait(true);
    }
}
