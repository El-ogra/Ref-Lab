using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Results;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels.Panels;

/// <summary>
/// The method sheet and critical values panel.
/// </summary>
/// <remarks>
/// The panel is shown beside the result entry field of the patient work
/// screen, in the way the reference product showed the method sheet of the
/// analyte under entry. It is a panel and not a screen of its own.
/// </remarks>
public sealed partial class MethodSheetPanelViewModel : ViewModelBase
{
    /// <summary>Initialises a new panel.</summary>
    public MethodSheetPanelViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <summary>The method sheet of the analyte under entry.</summary>
    [ObservableProperty]
    private MethodSheetDto? _methodSheet;

    /// <summary>The critical values of the analyte under entry.</summary>
    public ObservableCollection<CriticalValueDto> CriticalValues { get; } = new();

    /// <summary>Whether the panel has content to show.</summary>
    public bool HasContent => MethodSheet is not null;

    /// <summary>Loads the method sheet and the critical values of a test.</summary>
    /// <param name="testCode">The code of the test.</param>
    [RelayCommand]
    public async Task LoadAsync(int testCode)
    {
        await RunAsync(async () =>
        {
            var sheet = await _executor.SendAsync(new GetMethodSheetQuery(testCode));

            MethodSheet = sheet;

            CriticalValues.Clear();

            foreach (var value in sheet.CriticalValues)
            {
                CriticalValues.Add(value);
            }

            if (CriticalValues.Count == 0)
            {
                var critical = await _executor.SendAsync(new GetCriticalValuesQuery(testCode));

                if (critical is not null)
                {
                    CriticalValues.Add(critical);
                }
            }

            OnPropertyChanged(nameof(HasContent));
        }).ConfigureAwait(true);
    }
}
