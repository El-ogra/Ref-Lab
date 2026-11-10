using RefLab.Application.Abstractions;

namespace RefLab.Infrastructure.Reports;

/// <summary>
/// Allocates the report sequence numbers.
/// </summary>
/// <remarks>
/// Legacy evidence: the sequence origin is the absolute value
/// <c>SETTING.ini [setup] reportstart=1984</c>. The counter is held in memory
/// until Phase 4 persists it alongside the laboratory settings, which is where
/// the legacy system keeps the same origin.
/// </remarks>
public sealed class PrintSequenceGenerator : IPrintSequenceGenerator
{
    private long _next = 1984;

    /// <inheritdoc />
    public long Next() => System.Threading.Interlocked.Increment(ref _next) - 1;
}
