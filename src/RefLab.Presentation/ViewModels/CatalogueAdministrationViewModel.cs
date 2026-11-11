using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Domain.Enums;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the catalogue administration workspace.
/// </summary>
/// <remarks>
/// The workspace carries the five editors of the clinical knowledge that the
/// reference product spread over five separate windows: the test definitions,
/// the reference ranges, the units and the conversion factors, the
/// abbreviations and the clinical sub-groups.
/// </remarks>
public sealed partial class CatalogueAdministrationViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public CatalogueAdministrationViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Catalogue administration";

    /// <summary>The test definitions.</summary>
    public ObservableCollection<TestDefinitionDto> TestDefinitions { get; } = new();

    /// <summary>The reference ranges.</summary>
    public ObservableCollection<ReferenceRangeDto> ReferenceRanges { get; } = new();

    /// <summary>The clinical sub-groups.</summary>
    public ObservableCollection<ClinicalSubGroupDto> SubGroups { get; } = new();

    /// <summary>The abbreviations.</summary>
    public ObservableCollection<AbbreviationDto> Abbreviations { get; } = new();

    /// <summary>The units and the conversion factors.</summary>
    public ObservableCollection<TestUnitDto> Units { get; } = new();

    /// <summary>The conversion factors.</summary>
    public ObservableCollection<ConversionFactorDto> Conversions { get; } = new();

    /// <summary>The test definition being edited.</summary>
    [ObservableProperty]
    private TestDefinitionDto? _selectedTest;

    /// <summary>The sub-group being edited.</summary>
    [ObservableProperty]
    private ClinicalSubGroupDto? _selectedSubGroup;

    /// <summary>The abbreviation being edited.</summary>
    [ObservableProperty]
    private AbbreviationDto? _selectedAbbreviation;

    /// <summary>The code of a new test definition.</summary>
    [ObservableProperty]
    private int _newTestCode;

    /// <summary>The name of a new test definition.</summary>
    [ObservableProperty]
    private string _newTestName = string.Empty;

    /// <summary>The simple name of a new test definition.</summary>
    [ObservableProperty]
    private string _newTestSimpleName = string.Empty;

    /// <summary>The sub-group code of a new test definition.</summary>
    [ObservableProperty]
    private int _newTestSubGroupCode;

    /// <summary>The patient price of a new test definition.</summary>
    [ObservableProperty]
    private decimal _newTestPatientPrice;

    /// <summary>The laboratory price of a new test definition.</summary>
    [ObservableProperty]
    private decimal _newTestLabPrice;

    /// <summary>The external laboratory price of a new test definition.</summary>
    [ObservableProperty]
    private decimal _newTestOtherLabPrice;

    /// <summary>The Arabic name of a new test definition.</summary>
    [ObservableProperty]
    private string? _newTestArabicName;

    /// <summary>The abbreviation of a new test definition.</summary>
    [ObservableProperty]
    private string? _newTestAbbreviation;

    /// <summary>The sample type of a new test definition.</summary>
    [ObservableProperty]
    private string? _newTestSampleType;

    /// <summary>The code of a new reference range.</summary>
    [ObservableProperty]
    private int _newRangeTestCode;

    /// <summary>The sex scope of a new reference range.</summary>
    [ObservableProperty]
    private SexScope _newRangeSexScope = SexScope.All;

    /// <summary>The age unit of a new reference range.</summary>
    [ObservableProperty]
    private AgeUnit _newRangeAgeUnit = AgeUnit.Years;

    /// <summary>The lower limit of a new reference range.</summary>
    [ObservableProperty]
    private decimal? _newRangeLowLimit;

    /// <summary>The upper limit of a new reference range.</summary>
    [ObservableProperty]
    private decimal? _newRangeHighLimit;

    /// <summary>The code of a new clinical sub-group.</summary>
    [ObservableProperty]
    private int _newSubGroupCode;

    /// <summary>The name of a new clinical sub-group.</summary>
    [ObservableProperty]
    private string _newSubGroupName = string.Empty;

    /// <summary>The Arabic name of a new clinical sub-group.</summary>
    [ObservableProperty]
    private string? _newSubGroupArabicName;

    /// <summary>The identifier of the abbreviation being maintained.</summary>
    [ObservableProperty]
    private int _abbreviationId = 1;

    /// <summary>The text of the abbreviation being maintained.</summary>
    [ObservableProperty]
    private string _newAbbreviationText = string.Empty;

    /// <summary>The meaning of a new abbreviation.</summary>
    [ObservableProperty]
    private string _newAbbreviationMeaning = string.Empty;

    /// <summary>The name of a new unit.</summary>
    [ObservableProperty]
    private string? _newUnitName;

    /// <summary>The test the conversion factor belongs to.</summary>
    [ObservableProperty]
    private string? _newConversionTest;

    /// <summary>The unit the conversion starts from.</summary>
    [ObservableProperty]
    private string? _newConversionFromUnit;

    /// <summary>The unit the conversion ends at.</summary>
    [ObservableProperty]
    private string? _newConversionToUnit;

    /// <summary>The name of the test group being composed.</summary>
    [ObservableProperty]
    private string _newTestGroupName = string.Empty;

    /// <summary>The Arabic name of the test group being composed.</summary>
    [ObservableProperty]
    private string? _newTestGroupArabicName;

    /// <summary>The group whose tests the operator is copying.</summary>
    [ObservableProperty]
    private ClinicalSubGroupDto? _selectedSourceGroup;

    /// <summary>The test highlighted in the group being composed.</summary>
    [ObservableProperty]
    private TestDefinitionDto? _selectedGroupTest;

    /// <summary>The tests of the group being composed.</summary>
    public ObservableCollection<TestDefinitionDto> TestGroupTests { get; } = new();

    /// <summary>The factor of the conversion.</summary>
    [ObservableProperty]
    private decimal? _newConversionFactor;

    /// <summary>The number of decimals of the unit the conversion starts from.</summary>
    [ObservableProperty]
    private int _newConversionFromDecimals;

    /// <summary>The number of decimals of the unit the conversion ends at.</summary>
    [ObservableProperty]
    private int _newConversionToDecimals;

    /// <summary>Reloads every listing of the workspace.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var catalogue = await _executor.SendAsync(new GetTestCatalogueQuery(null, null, false, PageRequest.First(200)));

            TestDefinitions.Clear();

            foreach (var test in catalogue.Items)
            {
                TestDefinitions.Add(test);
            }

        }).ConfigureAwait(true);
    }

    /// <summary>Creates a test definition.</summary>
    [RelayCommand]
    private async Task CreateTestAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTestName) || string.IsNullOrWhiteSpace(NewTestSimpleName))
        {
            ErrorMessage = "A test name and a simple name are required.";
            return;
        }

        await RunAsync(async () =>
        {
            var created = await _executor.SendAsync(new CreateTestDefinitionCommand(
                NewTestCode,
                NewTestName,
                NewTestSimpleName,
                NewTestSubGroupCode,
                NewTestPatientPrice,
                NewTestLabPrice,
                NewTestOtherLabPrice,
                NewTestSampleType,
                NewTestArabicName,
                NewTestAbbreviation));

            TestDefinitions.Add(created);

            StatusMessage = $"Test {created.Code} was created.";
        }).ConfigureAwait(true);
    }

    /// <summary>Updates the selected test definition.</summary>
    [RelayCommand]
    private async Task UpdateTestAsync()
    {
        if (SelectedTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        await RunAsync(async () =>
        {
            var updated = await _executor.SendAsync(new UpdateTestDefinitionCommand(
                SelectedTest.Code,
                NewTestName.Length == 0 ? null : NewTestName,
                NewTestSimpleName.Length == 0 ? null : NewTestSimpleName,
                NewTestAbbreviation,
                NewTestArabicName,
                NewTestSampleType,
                null,
                NewTestPatientPrice,
                NewTestLabPrice,
                NewTestOtherLabPrice));

            Replace(TestDefinitions, updated, test => test.Code == updated.Code);

            StatusMessage = $"Test {updated.Code} was updated.";
        }).ConfigureAwait(true);
    }

    /// <summary>Creates a reference range.</summary>
    [RelayCommand]
    private async Task CreateReferenceRangeAsync()
    {
        await RunAsync(async () =>
        {
            var created = await _executor.SendAsync(new CreateReferenceRangeCommand(
                NewRangeTestCode,
                string.Empty,
                0,
                NewRangeSexScope,
                NewRangeAgeUnit,
                null,
                null,
                NewRangeLowLimit,
                NewRangeHighLimit));

            ReferenceRanges.Add(created);

            StatusMessage = "The reference range was created.";
        }).ConfigureAwait(true);
    }

    /// <summary>Updates the selected reference range.</summary>
    [RelayCommand]
    private async Task UpdateReferenceRangeAsync()
    {
        var selected = ReferenceRanges.FirstOrDefault();

        if (selected is null)
        {
            ErrorMessage = "Select a reference range first.";
            return;
        }

        await RunAsync(async () =>
        {
            var updated = await _executor.SendAsync(new UpdateReferenceRangeCommand(
                selected.Id,
                NewRangeLowLimit,
                NewRangeHighLimit));

            Replace(ReferenceRanges, updated, range => range.Id == updated.Id);

            StatusMessage = "The reference range was updated.";
        }).ConfigureAwait(true);
    }

    /// <summary>Creates a clinical sub-group.</summary>
    [RelayCommand]
    private async Task CreateSubGroupAsync()
    {
        if (string.IsNullOrWhiteSpace(NewSubGroupName))
        {
            ErrorMessage = "A sub-group name is required.";
            return;
        }

        await RunAsync(async () =>
        {
            var created = await _executor.SendAsync(new ManageClinicalSubGroupsCommand(
                NewSubGroupCode,
                NewSubGroupName,
                NewSubGroupArabicName));

            SubGroups.Add(created);

            StatusMessage = $"Sub-group {created.Code} was created.";
        }).ConfigureAwait(true);
    }

    /// <summary>
    /// Copies the tests of the selected group into the group being composed.
    /// </summary>
    /// <remarks>
    /// The reference system's custom test-group editor offers
    /// <c>Add all tests in group</c>, which copies the membership of the
    /// selected group into the group that is being composed.
    /// </remarks>
    [RelayCommand]
    private void AddAllTestsInGroup()
    {
        if (SelectedSourceGroup is null)
        {
            ErrorMessage = "Select a group first.";
            return;
        }

        foreach (var test in TestDefinitions.Where(test => test.SubGroupCode == SelectedSourceGroup.Code))
        {
            if (!TestGroupTests.Any(member => member.Code == test.Code))
            {
                TestGroupTests.Add(test);
            }
        }

        StatusMessage = $"{TestGroupTests.Count} test(s) selected.";
    }

    /// <summary>
    /// Removes the highlighted test from the group being composed.
    /// </summary>
    [RelayCommand]
    private void RemoveGroupTest()
    {
        if (SelectedGroupTest is null)
        {
            ErrorMessage = "Select a test first.";
            return;
        }

        TestGroupTests.Remove(SelectedGroupTest);
        SelectedGroupTest = null;
    }

    /// <summary>
    /// Adds every test of the catalogue to the group being composed.
    /// </summary>
    [RelayCommand]
    private void AddEveryTestToGroup()
    {
        foreach (var test in TestDefinitions)
        {
            if (!TestGroupTests.Any(member => member.Code == test.Code))
            {
                TestGroupTests.Add(test);
            }
        }

        StatusMessage = $"{TestGroupTests.Count} test(s) selected.";
    }

    /// <summary>
    /// Creates the test group from the tests that have been selected.
    /// </summary>
    [RelayCommand]
    private async Task CreateTestGroupAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTestGroupName))
        {
            ErrorMessage = "A test group name is required.";
            return;
        }

        if (TestGroupTests.Count == 0)
        {
            ErrorMessage = "Select at least one test for the group.";
            return;
        }

        await RunAsync(async () =>
        {
            var created = await _executor.SendAsync(new CreateTestGroupCommand(
                NewTestGroupName,
                TestGroupTests.Select(test => test.Code).ToArray(),
                NewTestGroupArabicName));

            SubGroups.Add(created);

            TestGroupTests.Clear();
            NewTestGroupName = string.Empty;

            StatusMessage = $"Test group {created.Code} was created.";
        }).ConfigureAwait(true);
    }

    /// <summary>Creates an abbreviation.</summary>
    [RelayCommand]
    private async Task CreateAbbreviationAsync()
    {
        if (string.IsNullOrWhiteSpace(NewAbbreviationText) || string.IsNullOrWhiteSpace(NewAbbreviationMeaning))
        {
            ErrorMessage = "An abbreviation and its meaning are required.";
            return;
        }

        await RunAsync(async () =>
        {
            var created = await _executor.SendAsync(new ManageAbbreviationsCommand(
                AbbreviationId,
                NewAbbreviationText,
                NewAbbreviationMeaning));

            Abbreviations.Add(created);

            StatusMessage = "The abbreviation was created.";
        }).ConfigureAwait(true);
    }

    /// <summary>Maintains the units and the conversion factors.</summary>
    /// <remarks>
    /// The Application layer exposes a single use case for the units and the
    /// conversion factors, and it refuses a request that supplies neither a
    /// unit name nor a conversion factor. The listing is therefore refreshed
    /// from the response of that use case rather than by a separate query.
    /// </remarks>
    [RelayCommand]
    private async Task SaveUnitsAndConversionsAsync()
    {
        var suppliesUnit = !string.IsNullOrWhiteSpace(NewUnitName);
        var suppliesConversion = !string.IsNullOrWhiteSpace(NewConversionTest) &&
                                  !string.IsNullOrWhiteSpace(NewConversionFromUnit) &&
                                  !string.IsNullOrWhiteSpace(NewConversionToUnit) &&
                                  NewConversionFactor.HasValue;

        if (!suppliesUnit && !suppliesConversion)
        {
            ErrorMessage = "A unit name or a conversion factor must be supplied.";
            return;
        }

        await RunAsync(async () =>
        {
            var units = await _executor.SendAsync(new ManageUnitsAndConversionsCommand(
                NewUnitName,
                NewConversionTest,
                NewConversionFromUnit,
                NewConversionToUnit,
                NewConversionFactor,
                NewConversionFromDecimals,
                NewConversionToDecimals));

            Units.Clear();

            foreach (var unit in units.Units)
            {
                Units.Add(unit);
            }

            Conversions.Clear();

            foreach (var conversion in units.Conversions)
            {
                Conversions.Add(conversion);
            }

            StatusMessage = "The units were saved.";
        }).ConfigureAwait(true);
    }

    private static void Replace<T>(ObservableCollection<T> collection, T value, Func<T, bool> predicate)
    {
        for (var index = 0; index < collection.Count; index++)
        {
            if (predicate(collection[index]))
            {
                collection[index] = value;
                return;
            }
        }
    }
}
