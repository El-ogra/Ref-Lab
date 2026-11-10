namespace RefLab.Presentation.Common;

/// <summary>
/// Presents an operator-facing error message.
/// </summary>
/// <remarks>
/// The presentation layer keeps the presentation of a failure behind an
/// abstraction so that a view model never has to reference a WPF dialog, and
/// so that the presenter can be replaced in a test.
/// </remarks>
public interface IErrorPresenter
{
    /// <summary>Presents an error message to the operator.</summary>
    /// <param name="message">The message to present.</param>
    void ShowError(string message);

    /// <summary>Presents an informational message to the operator.</summary>
    /// <param name="message">The message to present.</param>
    void ShowInformation(string message);

    /// <summary>Asks the operator to confirm an action.</summary>
    /// <param name="message">The confirmation question.</param>
    /// <returns><see langword="true"/> when the operator confirmed.</returns>
    bool Confirm(string message);
}
