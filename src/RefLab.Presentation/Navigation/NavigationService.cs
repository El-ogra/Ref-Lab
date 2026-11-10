using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace RefLab.Presentation.Navigation;

/// <summary>
/// The default navigation service.
/// </summary>
/// <remarks>
/// The service keeps a small history so that the shell can return to the
/// previous screen. The current view is published as a notifying property so
/// that the shell's navigation region binds to it directly.
/// </remarks>
public sealed class NavigationService : INavigationService
{
    private readonly IViewLocator _viewLocator;
    private readonly List<(string ScreenKey, object? Parameter)> _history = new();
    private UserControl? _currentView;
    private string? _currentScreenKey;

    /// <summary>Initialises a new navigation service.</summary>
    public NavigationService(IViewLocator viewLocator)
    {
        _viewLocator = viewLocator ?? throw new ArgumentNullException(nameof(viewLocator));
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public object? CurrentView => _currentView;

    /// <inheritdoc />
    public string? CurrentScreenKey => _currentScreenKey;

    /// <inheritdoc />
    public void NavigateTo(string screenKey, object? parameter = null)
    {
        if (string.IsNullOrWhiteSpace(screenKey))
        {
            return;
        }

        if (_viewLocator.Locate(screenKey) is not UserControl view)
        {
            return;
        }

        if (_currentScreenKey is not null &&
            !string.Equals(_currentScreenKey, screenKey, StringComparison.Ordinal))
        {
            _history.Add((_currentScreenKey, null));
        }

        if (_currentView?.DataContext is INavigationAware leaving)
        {
            leaving.OnNavigatedFrom();
        }

        _currentScreenKey = screenKey;
        _currentView = view;

        if (view.DataContext is INavigationAware arriving)
        {
            arriving.OnNavigatedTo(parameter);
        }

        OnPropertyChanged(nameof(CurrentView));
        OnPropertyChanged(nameof(CurrentScreenKey));
    }

    /// <inheritdoc />
    public bool GoBack()
    {
        if (_history.Count == 0)
        {
            return false;
        }

        var previous = _history[^1];
        _history.RemoveAt(_history.Count - 1);

        NavigateTo(previous.ScreenKey, previous.Parameter);

        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
