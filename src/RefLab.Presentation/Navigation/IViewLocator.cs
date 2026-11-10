namespace RefLab.Presentation.Navigation;

/// <summary>
/// Resolves the view of a screen.
/// </summary>
/// <remarks>
/// This is the only place that maps a screen key onto a view, which keeps the
/// shell free of a table of screen types.
/// </remarks>
public interface IViewLocator
{
    /// <summary>Resolves the view of a screen.</summary>
    /// <param name="screenKey">The key of the screen.</param>
    /// <returns>The view, or <see langword="null"/> when the key is unknown.</returns>
    object? Locate(string screenKey);
}
