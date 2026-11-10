namespace RefLab.Presentation.Navigation;

/// <summary>
/// The contract a view model implements to take part in navigation.
/// </summary>
/// <remarks>
/// A screen that receives a parameter — a patient, an order, a date range —
/// implements this contract so that the navigation service can hand the
/// parameter over without the view knowing how it arrived.
/// </remarks>
public interface INavigationAware
{
    /// <summary>Called after the screen becomes the current screen.</summary>
    /// <param name="parameter">The navigation parameter, when there is one.</param>
    void OnNavigatedTo(object? parameter);

    /// <summary>Called before the screen stops being the current screen.</summary>
    void OnNavigatedFrom();
}
