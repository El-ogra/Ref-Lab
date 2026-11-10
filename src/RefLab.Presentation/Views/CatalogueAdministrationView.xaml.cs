using System.Windows.Controls;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Views;

/// <summary>
/// CatalogueAdministrationView.xaml
/// </summary>
public partial class CatalogueAdministrationView : UserControl
{
    /// <summary>Initialises a new instance.</summary>
    /// <param name="viewModel">The view model.</param>
    public CatalogueAdministrationView(CatalogueAdministrationViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();

        DataContext = viewModel;
    }
}
