namespace RefLab.Application.Abstractions;

/// <summary>
/// Allocates the report sequence numbers.
/// </summary>
/// <remarks>
/// Legacy evidence: the sequence origin is the absolute value
/// <c>SETTING.ini [setup] reportstart=1984</c>. The number makes every issued
/// report attributable and reveals a missing report when a number is absent.
/// </remarks>
public interface IPrintSequenceGenerator
{
    /// <summary>Returns the next sequence number.</summary>
    long Next();
}
