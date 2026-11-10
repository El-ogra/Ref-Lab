namespace RefLab.Domain.Enums;

/// <summary>
/// The settlement state of a sample referred to an external laboratory.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>patienttest</c> carries two independent booleans —
/// <c>send</c> (dispatched) and <c>sentout</c> (settled). The legacy manual
/// describes the two states as <c>&#x062e;&#x0627;&#x0644;&#x0635;</c> and
/// <c>&#x062f;&#x0627;&#x0626;&#x0645;&#x0627; &#x0625;&#x0644;&#x064a;&#x0647;</c>.
/// This enum is a projection in which settlement wins over dispatch; both raw
/// booleans remain available on the entity so no legacy information is lost.
/// </remarks>
public enum SendOutState
{
    /// <summary>The sample has neither been dispatched nor settled.</summary>
    Pending = 1,

    /// <summary>The sample has been dispatched to the external laboratory but is not yet settled.</summary>
    Dispatched = 2,

    /// <summary>The sample has been settled with the external laboratory.</summary>
    Settled = 3,
}
