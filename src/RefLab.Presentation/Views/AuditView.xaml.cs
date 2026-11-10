using System.Windows.Controls;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Views;

/// <summary>
/// AuditView.xaml
/// </summary>
public partial class AuditView : UserControl
{
    /// <summary>Initialises a new instance.</summary>
    /// <param name="viewModel">The view model.</param>
    public AuditView(AuditViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();

        DataContext = viewModel;
    }
}
