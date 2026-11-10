using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using RefLab.Application.Features.Derivation;
using RefLab.Application.Features.Results;
using RefLab.Domain.Enums;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels.Panels;

/// <summary>
/// The derivation panel.
/// </summary>
/// <remarks>
/// <para>
/// The panel asks the Application layer for the derived clinical values and
/// displays what it returns. It never derives a value itself: the derivation
/// rules belong to the domain, and the values shown are the values the domain
/// computed.
/// </para>
/// <para>
/// The panel is docked inside the patient work screen rather than being a
/// screen of its own.
/// </para>
/// </remarks>
public sealed partial class DerivationPanelViewModel : ViewModelBase
{
    /// <summary>Initialises a new panel.</summary>
    public DerivationPanelViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <summary>The derived values.</summary>
    public ObservableCollection<DerivationResultDto> Results { get; } = new();

    /// <summary>The haemoglobin value the derivation starts from.</summary>
    [ObservableProperty]
    private decimal _hemoglobin;

    /// <summary>The sex used by the haemoglobin percentage derivation.</summary>
    [ObservableProperty]
    private Sex _sex = Sex.Male;

    /// <summary>The age, in years, used by the haemoglobin percentage derivation.</summary>
    [ObservableProperty]
    private decimal _ageYears;

    /// <summary>The patient prothrombin time.</summary>
    [ObservableProperty]
    private decimal _patientProthrombinTime;

    /// <summary>The control prothrombin time.</summary>
    [ObservableProperty]
    private decimal _controlProthrombinTime;

    /// <summary>The normal prothrombin time.</summary>
    [ObservableProperty]
    private decimal _normalProthrombinTime;

    /// <summary>The control index.</summary>
    [ObservableProperty]
    private decimal _controlIndex;

    /// <summary>The control time the prothrombin interpolation starts from.</summary>
    [ObservableProperty]
    private decimal _controlTime;

    /// <summary>Derives the haematocrit.</summary>
    [RelayCommand]
    private async Task DeriveHematocritAsync() =>
        await DeriveAsync(() => new DeriveHematocritQuery(Hemoglobin)).ConfigureAwait(true);

    /// <summary>Derives the haemoglobin percentage.</summary>
    [RelayCommand]
    private async Task DeriveHemoglobinPercentageAsync() =>
        await DeriveAsync(() => new DeriveHemoglobinPercentageQuery(Hemoglobin, Sex, AgeYears)).ConfigureAwait(true);

    /// <summary>Derives the international sensitivity index.</summary>
    [RelayCommand]
    private async Task DeriveInternationalSensitivityIndexAsync() =>
        await DeriveAsync(() => new DeriveInternationalSensitivityIndexQuery(
            PatientProthrombinTime,
            ControlProthrombinTime,
            NormalProthrombinTime,
            ControlIndex)).ConfigureAwait(true);

    /// <summary>Interpolates the prothrombin control.</summary>
    [RelayCommand]
    private async Task InterpolateProthrombinControlAsync() =>
        await DeriveAsync(() => new InterpolateProthrombinControlQuery(ControlTime)).ConfigureAwait(true);

    /// <summary>Removes every derived value.</summary>
    [RelayCommand]
    private void Clear() => Results.Clear();

    private async Task DeriveAsync(Func<IRequest<DerivationResultDto>> factory)
    {
        await RunAsync(async () =>
        {
            var result = await _executor.SendAsync(factory());

            Results.Clear();
            Results.Add(result);
        }).ConfigureAwait(true);
    }
}
