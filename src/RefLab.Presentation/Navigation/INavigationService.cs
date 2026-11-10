using System.ComponentModel;

namespace RefLab.Presentation.Navigation;

/// <summary>
/// Moves between the screens of the shell.
/// </summary>
/// <remarks>
/// The main window is the shell, not a screen: it owns a navigation region
/// and shows one screen at a time. A view model navigates through this
/// abstraction rather than reaching into the window, which keeps the view
/// models unit-testable against a fake.
/// </remarks>
public interface INavigationService : INotifyPropertyChanged
{
    /// <summary>The view of the current screen.</summary>
    object? CurrentView { get; }

    /// <summary>The key of the current screen.</summary>
    string? CurrentScreenKey { get; }

    /// <summary>Navigates to a screen.</summary>
    /// <param name="screenKey">The key of the target screen.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    void NavigateTo(string screenKey, object? parameter = null);

    /// <summary>Returns to the previously visited screen, when there is one.</summary>
    /// <returns><see langword="true"/> when a screen was returned to.</returns>
    bool GoBack();
}
