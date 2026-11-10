namespace RefLab.Domain.Enums;

/// <summary>
/// The clinical classification of an entered result value.
/// </summary>
/// <remarks>
/// The classification is three-valued and ordered: normal, then abnormal
/// (high or low), then <em>critical</em>. A value that breaches a critical
/// limit is always classified as critical, whatever its abnormal classification.
/// Legacy evidence: the shipped manual describes the flag as
/// <c>&#x0645;&#x0639;&#x0627;&#x0644;&#x064a; &#x0623;&#x0648; &#x0645;&#x0646;&#x062e;&#x0641;&#x0636;&#x0629; &#x0623;&#x0648; &#x062d;&#x0631;&#x062c;&#x0629;</c>
/// (high, low or critical). The printed report renders the flag in English as
/// <c>High</c>, <c>Low</c> or <c>Normal</c>; the display strings themselves
/// belong to the presentation layer and are sourced from
/// <c>ui-labels-verified.md</c>.
/// </remarks>
public enum ResultClassification
{
    /// <summary>The value lies inside the applicable reference interval.</summary>
    Normal = 0,

    /// <summary>The value is above the applicable upper reference limit.</summary>
    High = 1,

    /// <summary>The value is below the applicable lower reference limit.</summary>
    Low = 2,

    /// <summary>The value breaches a critical limit. This overrides any abnormal classification.</summary>
    Critical = 3,
}
