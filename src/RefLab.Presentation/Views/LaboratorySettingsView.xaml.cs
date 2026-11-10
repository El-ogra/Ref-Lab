using System.Windows.Controls;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Views;

/// <summary>
/// LaboratorySettingsView.xaml
/// </summary>
public partial class LaboratorySettingsView : UserControl
{
    /// <summary>Initialises a new instance.</summary>
    /// <param name="viewModel">The view model.</param>
    public LaboratorySettingsView(LaboratorySettingsViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();

        DataContext = viewModel;
    }
}
