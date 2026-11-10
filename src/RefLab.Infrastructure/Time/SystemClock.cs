using RefLab.Application.Abstractions;

namespace RefLab.Infrastructure.Time;

/// <summary>
/// The system clock.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
