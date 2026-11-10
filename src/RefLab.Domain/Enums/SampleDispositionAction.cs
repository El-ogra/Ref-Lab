namespace RefLab.Domain.Enums;

/// <summary>
/// The action recorded against a sample when it leaves its primary container.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability
/// <c>&#x0641;&#x0635;&#x0644; &#x0648;&#x0633;&#x062d;&#x0628; &#x0627;&#x0644;&#x0639;&#x064a;&#x0646;&#x0627;&#x062a;</c>
/// (split and withdraw samples) against the microbiology tables
/// <c>cultures</c>, <c>cultureanti</c> and <c>sample</c>. The structure of the
/// <c>sample</c> table was never extracted, so no further disposition values
/// are asserted here.
/// </remarks>
public enum SampleDispositionAction
{
    /// <summary>The sample was divided into a second portion.</summary>
    Split = 1,

    /// <summary>The sample was withdrawn from the laboratory.</summary>
    Withdrawn = 2,
}
