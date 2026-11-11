using RefLab.Presentation.Common;

namespace RefLab.Presentation.Tests.Fakes;

/// <summary>
/// An error presenter that records what the view models asked it to show.
/// </summary>
public sealed class RecordingErrorPresenter : IErrorPresenter
{
    /// <summary>The messages that were shown to the operator.</summary>
    public List<string> Errors { get; } = new();

    /// <summary>The information messages that were shown to the operator.</summary>
    public List<string> Information { get; } = new();

    /// <summary>Whether the presenter confirms an action.</summary>
    public bool Confirms { get; set; } = true;

    /// <inheritdoc />
    public void ShowError(string message) => Errors.Add(message);

    /// <inheritdoc />
    public void ShowInformation(string message) => Information.Add(message);

    /// <inheritdoc />
    public bool Confirm(string message) => Confirms;
}
