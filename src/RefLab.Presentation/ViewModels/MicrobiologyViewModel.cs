using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Microbiology;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the microbiology and farm results screen.
/// </summary>
public sealed partial class MicrobiologyViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public MicrobiologyViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Microbiology and farm results";

    /// <summary>The culture results.</summary>
    public ObservableCollection<CultureResultDto> Cultures { get; } = new();

    /// <summary>The antibiotic catalogue.</summary>
    public ObservableCollection<AntibioticDto> Antibiotics { get; } = new();

    /// <summary>The farms.</summary>
    public ObservableCollection<FarmDto> Farms { get; } = new();

    /// <summary>The farm transactions.</summary>
    public ObservableCollection<FarmTransactionDto> FarmTransactions { get; } = new();

    /// <summary>The patient whose cultures are listed.</summary>
    [ObservableProperty]
    private Guid? _patientId;

    /// <summary>The culture identifier.</summary>
    [ObservableProperty]
    private string _cultureId = string.Empty;

    /// <summary>The organism that was isolated.</summary>
    [ObservableProperty]
    private string _organism = string.Empty;

    /// <summary>The identifier of the antibiotic being maintained.</summary>
    [ObservableProperty]
    private int _antibioticId = 1;

    /// <summary>The antibiotic symbol of a susceptibility.</summary>
    [ObservableProperty]
    private string _antibioticSymbol = string.Empty;

    /// <summary>The reading of a susceptibility.</summary>
    [ObservableProperty]
    private string _reading = string.Empty;

    /// <summary>The unit of a susceptibility reading.</summary>
    [ObservableProperty]
    private string? _readingUnit;

    /// <summary>The susceptibilities of the culture being entered.</summary>
    public ObservableCollection<AntibioticSensitivityDto> Susceptibilities { get; } = new();

    /// <summary>The farm whose transactions are entered.</summary>
    [ObservableProperty]
    private int _farmId;

    /// <summary>The name of the farm.</summary>
    [ObservableProperty]
    private string _farmName = string.Empty;

    /// <summary>The address of the farm.</summary>
    [ObservableProperty]
    private string? _farmAddress;

    /// <summary>The telephone number of the farm.</summary>
    [ObservableProperty]
    private string? _farmPhone;

    /// <summary>The amount issued to the farm.</summary>
    [ObservableProperty]
    private decimal _issuedAmount;

    /// <summary>The amount received from the farm.</summary>
    [ObservableProperty]
    private decimal _receivedAmount;

    /// <summary>Reloads the cultures.</summary>
    [RelayCommand]
    private async Task LoadCulturesAsync()
    {
        await RunAsync(async () =>
        {
            var cultures = await _executor.SendAsync(new GetCultureResultsQuery(PatientId));

            Cultures.Clear();

            foreach (var culture in cultures)
            {
                Cultures.Add(culture);
            }

            StatusMessage = $"{cultures.Count} culture(s).";
        }).ConfigureAwait(true);
    }

    /// <summary>Records a culture result.</summary>
    [RelayCommand]
    private async Task RecordCultureAsync()
    {
        if (PatientId is null)
        {
            ErrorMessage = "A patient is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CultureId) || string.IsNullOrWhiteSpace(Organism))
        {
            ErrorMessage = "A culture identifier and an organism are required.";
            return;
        }

        await RunAsync(async () =>
        {
            var recorded = await _executor.SendAsync(new RecordCultureResultCommand(
                PatientId.Value,
                CultureId,
                Organism,
                Susceptibilities.ToArray()));

            Cultures.Add(recorded);

            Susceptibilities.Clear();

            StatusMessage = "The culture was recorded.";
        }).ConfigureAwait(true);
    }

    /// <summary>Adds a susceptibility to the culture being entered.</summary>
    [RelayCommand]
    private void AddSusceptibility()
    {
        if (string.IsNullOrWhiteSpace(AntibioticSymbol))
        {
            ErrorMessage = "An antibiotic symbol is required.";
            return;
        }

        if (!Susceptibilities.Any(item => item.AntibioticSymbol == AntibioticSymbol))
        {
            Susceptibilities.Add(new AntibioticSensitivityDto(AntibioticSymbol, Reading, ReadingUnit));
        }

        AntibioticSymbol = string.Empty;
        Reading = string.Empty;
        ReadingUnit = null;
    }

    /// <summary>Maintains the antibiotic catalogue.</summary>
    [RelayCommand]
    private async Task SaveAntibioticAsync()
    {
        if (string.IsNullOrWhiteSpace(AntibioticSymbol))
        {
            ErrorMessage = "An antibiotic symbol is required.";
            return;
        }

        await RunAsync(async () =>
        {
            var saved = await _executor.SendAsync(new ManageAntibioticCatalogueCommand(
                AntibioticId,
                AntibioticSymbol,
                AntibioticSymbol,
                Array.Empty<string>()));

            Antibiotics.Add(saved);

            AntibioticSymbol = string.Empty;

            StatusMessage = "The antibiotic was saved.";
        }).ConfigureAwait(true);
    }

    /// <summary>Records a farm result.</summary>
    [RelayCommand]
    private async Task SaveFarmResultAsync()
    {
        if (string.IsNullOrWhiteSpace(FarmName))
        {
            ErrorMessage = "A farm name is required.";
            return;
        }

        await RunAsync(async () =>
        {
            var saved = await _executor.SendAsync(new ManageFarmResultsCommand(
                0,
                FarmId,
                FarmName,
                DateTimeOffset.Now,
                IssuedAmount,
                ReceivedAmount,
                null,
                FarmAddress,
                FarmPhone));

            FarmTransactions.Add(saved);

            StatusMessage = "The farm result was recorded.";
        }).ConfigureAwait(true);
    }
}
