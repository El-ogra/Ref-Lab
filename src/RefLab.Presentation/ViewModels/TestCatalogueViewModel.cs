using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the test catalogue and selection screen.
/// </summary>
/// <remarks>
/// The catalogue screen is the English surface of the clinical knowledge. It
/// browses the test catalogue, shows the profile of a test and the custom
/// groups, and maintains the price lists of the laboratory.
/// </remarks>
public sealed partial class TestCatalogueViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public TestCatalogueViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Test catalogue";

    /// <summary>The catalogue listing.</summary>
    public ObservableCollection<TestDefinitionDto> Tests { get; } = new();

    /// <summary>The profile of the selected test.</summary>
    public ObservableCollection<ReferenceRangeDto> Profile { get; } = new();

    /// <summary>The test the operator is working with.</summary>
    [ObservableProperty]
    private TestDefinitionDto? _selectedTest;

    /// <summary>The filter applied to the catalogue listing.</summary>
    [ObservableProperty]
    private string? _filter;

    /// <summary>The sub-group the catalogue is filtered by.</summary>
    [ObservableProperty]
    private int? _subGroupCode;

    /// <summary>Loads the catalogue listing.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var result = await _executor.SendAsync(new GetTestCatalogueQuery(Filter, SubGroupCode, true, PageRequest.First(200)));

            Tests.Clear();

            foreach (var test in result.Items)
            {
                Tests.Add(test);
            }

            StatusMessage = $"{result.TotalCount} test(s) in the catalogue.";
        }).ConfigureAwait(true);
    }

    /// <summary>Loads the profile — the reference ranges — of the selected test.</summary>
    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        if (SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        await RunAsync(async () =>
        {
            var profile = await _executor.SendAsync(new GetTestProfileQuery(SelectedTest.Code));

            Profile.Clear();

            foreach (var range in profile.ReferenceRanges)
            {
                Profile.Add(range);
            }

            StatusMessage = $"{profile.ReferenceRanges.Count} reference range(s).";
        }).ConfigureAwait(true);
    }
}
