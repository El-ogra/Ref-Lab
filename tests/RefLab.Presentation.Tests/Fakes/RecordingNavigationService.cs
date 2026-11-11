using System.ComponentModel;
using RefLab.Presentation.Navigation;

namespace RefLab.Presentation.Tests.Fakes;

/// <summary>
/// A navigation service that records what the view models asked for.
/// </summary>
/// <remarks>
/// The presentation layer navigates through the
/// <see cref="INavigationService"/> abstraction rather than reaching into the
/// window, which is exactly what makes the view models testable. This
/// recording implementation is the fake that the abstraction was designed
/// for.
/// </remarks>
public sealed class RecordingNavigationService : INavigationService
{
    private readonly List<string> _visited = new();

    /// <inheritdoc />
    public object? CurrentView { get; private set; }

    /// <inheritdoc />
    public string? CurrentScreenKey { get; private set; }

    /// <summary>The screens the view models navigated to, in order.</summary>
    public IReadOnlyList<string> Visited => _visited;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public void NavigateTo(string screenKey, object? parameter = null)
    {
        ArgumentNullException.ThrowIfNull(screenKey);

        _visited.Add(screenKey);

        CurrentScreenKey = screenKey;
        CurrentView = parameter;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentScreenKey)));
    }

    /// <inheritdoc />
    public bool GoBack()
    {
        if (_visited.Count < 2)
        {
            return false;
        }

        _visited.RemoveAt(_visited.Count - 1);

        CurrentScreenKey = _visited[^1];

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentScreenKey)));

        return true;
    }
}
