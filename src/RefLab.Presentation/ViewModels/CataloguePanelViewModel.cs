using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Catalogue;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The display state of the English test-catalogue panel.
/// </summary>
/// <remarks>
/// <para>
/// The catalogue panel is the English surface of the patient work screen. It
/// holds the catalogue listing and the tests the operator has selected for the
/// current order; the commands that act on the selection belong to the parent
/// view model.
/// </para>
/// <para>
/// The panel carries the three actions of the reference system's own catalogue
/// panel — <c>Add select test</c>, <c>Delete</c> and <c>All</c> — which add the
/// highlighted catalogue test to the selection, remove the highlighted selected
/// test from it, and add every listed test to it.
/// </para>
/// </remarks>
public sealed partial class CataloguePanelViewModel : ObservableObject
{
    /// <summary>The catalogue listing.</summary>
    public ObservableCollection<TestDefinitionDto> Tests { get; } = new();

    /// <summary>The tests selected for the current order.</summary>
    public ObservableCollection<TestDefinitionDto> SelectedTests { get; } = new();

    /// <summary>The test the operator is working with in the catalogue.</summary>
    [ObservableProperty]
    private TestDefinitionDto? _selectedTest;

    /// <summary>The selected test the operator is working with in the selection.</summary>
    [ObservableProperty]
    private TestDefinitionDto? _selectedOrderTest;

    /// <summary>The filter applied to the catalogue listing.</summary>
    [ObservableProperty]
    private string? _filter;

    /// <summary>Whether the catalogue lists the send-out tests only.</summary>
    [ObservableProperty]
    private bool _sendOutOnly;

    /// <summary>Removes every selection.</summary>
    public void ClearSelection()
    {
        SelectedTests.Clear();
        SelectedOrderTest = null;
    }

    /// <summary>
    /// Adds the highlighted catalogue test to the selection for the order.
    /// </summary>
    [RelayCommand]
    private void AddSelectedTest()
    {
        if (SelectedTest is null)
        {
            return;
        }

        if (!SelectedTests.Any(test => test.Code == SelectedTest.Code))
        {
            SelectedTests.Add(SelectedTest);
        }
    }

    /// <summary>
    /// Removes the highlighted selected test from the selection for the order.
    /// </summary>
    [RelayCommand]
    private void RemoveSelectedTest()
    {
        if (SelectedOrderTest is null)
        {
            return;
        }

        SelectedTests.Remove(SelectedOrderTest);
        SelectedOrderTest = null;
    }

    /// <summary>
    /// Adds every test the catalogue currently lists to the selection for the
    /// order.
    /// </summary>
    [RelayCommand]
    private void AddAllTests()
    {
        foreach (var test in Tests)
        {
            if (!SelectedTests.Any(selected => selected.Code == test.Code))
            {
                SelectedTests.Add(test);
            }
        }
    }
}
