using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RefLab.Application.Features.Catalogue;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The display state of the English test-catalogue panel.
/// </summary>
/// <remarks>
/// The catalogue panel is the English surface of the patient work screen. It
/// holds the catalogue listing and the tests the operator has selected for the
/// current order; the commands that act on the selection belong to the parent
/// view model.
/// </remarks>
public sealed partial class CataloguePanelViewModel : ObservableObject
{
    /// <summary>The catalogue listing.</summary>
    public ObservableCollection<TestDefinitionDto> Tests { get; } = new();

    /// <summary>The tests selected for the current order.</summary>
    public ObservableCollection<TestDefinitionDto> SelectedTests { get; } = new();

    /// <summary>The test the operator is working with.</summary>
    [ObservableProperty]
    private TestDefinitionDto? _selectedTest;

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
        SelectedTest = null;
    }
}
