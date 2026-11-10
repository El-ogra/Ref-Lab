using System.Windows;

namespace RefLab.Presentation.Common;

/// <summary>
/// Presents messages through WPF message boxes.
/// </summary>
public sealed class ErrorPresenter : IErrorPresenter
{
    /// <inheritdoc />
    public void ShowError(string message) =>
        MessageBox.Show(message, "Ref Lab", MessageBoxButton.OK, MessageBoxImage.Error);

    /// <inheritdoc />
    public void ShowInformation(string message) =>
        MessageBox.Show(message, "Ref Lab", MessageBoxButton.OK, MessageBoxImage.Information);

    /// <inheritdoc />
    public bool Confirm(string message)
    {
        var result = MessageBox.Show(message, "Ref Lab", MessageBoxButton.YesNo, MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }
}
