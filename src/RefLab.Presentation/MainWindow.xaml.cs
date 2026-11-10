using System.Windows;
using RefLab.Presentation.ViewModels;

namespace RefLab.Presentation;

/// <summary>
/// The shell window of the application.
/// </summary>
/// <remarks>
/// The main window is the shell, not a screen. It owns the module toolbar, the
/// operator identity strip and the navigation region, and it shows one screen
/// at a time. The screens themselves are resolved from the dependency-injection
/// container by the navigation service.
/// </remarks>
public partial class MainWindow : Window
{
    /// <summary>Initialises a new instance.</summary>
    /// <param name="shellViewModel">The shell view model.</param>
    public MainWindow(ShellViewModel shellViewModel)
    {
        ArgumentNullException.ThrowIfNull(shellViewModel);

        InitializeComponent();

        DataContext = shellViewModel;
    }
}
