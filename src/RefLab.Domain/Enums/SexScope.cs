namespace RefLab.Domain.Enums;

/// <summary>
/// The sex a reference range applies to.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>normalrange.sex</c> is <c>Text (10)</c>. Across the 852
/// shipped rows the distribution was <c>Male</c> 422, <c>Female</c> 421,
/// <c>All</c> 5 and empty 4. The empty value is not established and is not
/// reproduced here; <see cref="All"/> covers the sex-agnostic rows.
/// </remarks>
public enum SexScope
{
    Male = 1,
    Female = 2,

    /// <summary>The range applies to every sex.</summary>
    All = 3,
}
