namespace RefLab.Domain.Enums;

/// <summary>
/// The unit in which an age or an age band is expressed.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>normalrange.ageunit</c> is <c>Text (10)</c>. Across the
/// 852 shipped rows the distribution was <c>Years</c> 708, <c>Days</c> 70,
/// <c>Mons.</c> 65 and empty 9. The legacy spelling <c>Mons.</c> is normalised
/// here to <see cref="Months"/>; the empty value is not established.
/// </remarks>
public enum AgeUnit
{
    Years = 1,
    Months = 2,
    Days = 3,
}
