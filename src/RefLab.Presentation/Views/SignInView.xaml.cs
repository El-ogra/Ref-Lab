using System.Windows.Controls;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation.Views;

/// <summary>
/// The start-up and sign-in screen.
/// </summary>
public partial class SignInView : UserControl
{
    /// <summary>Initialises a new instance.</summary>
    /// <param name="viewModel">The view model.</param>
    public SignInView(SignInViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();

        DataContext = viewModel;
    }
}
