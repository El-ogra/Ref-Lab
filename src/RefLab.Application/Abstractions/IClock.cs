namespace RefLab.Application.Abstractions;

/// <summary>
/// The source of the current time.
/// </summary>
/// <remarks>
/// Injecting the clock keeps every recorded timestamp under the control of the
/// test suite instead of depending on the wall clock.
/// </remarks>
public interface IClock
{
    /// <summary>The current instant in UTC.</summary>
    DateTimeOffset UtcNow { get; }
}
