using System.Windows.Controls;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Views;

/// <summary>
/// PartiesPricingView.xaml
/// </summary>
public partial class PartiesPricingView : UserControl
{
    /// <summary>Initialises a new instance.</summary>
    /// <param name="viewModel">The view model.</param>
    public PartiesPricingView(PartiesPricingViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();

        DataContext = viewModel;
    }
}
