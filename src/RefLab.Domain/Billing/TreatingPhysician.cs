using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Billing;

/// <summary>
/// A treating physician — the doctor who asked for the examination.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: version 2 split the physician out of the referring party.
/// The <c>mowzaf</c> table holds the physicians and is referenced from
/// <c>patientinfo.mowzaf</c> and <c>payment.mowzaf</c>; the referring party
/// points at one through <c>Gehats.ReferredPW</c> and <c>Gehats.ReferredID</c>.
/// </para>
/// <para>
/// The field-level structure of the <c>mowzaf</c> table was never extracted, so
/// this entity asserts only the identifiers the evidence establishes and does
/// not invent further attributes.
/// </para>
/// </remarks>
public sealed class TreatingPhysician
{
    private TreatingPhysician()
    {
    }

    /// <summary>The physician identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The physician code.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>The physician name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The physician telephone number, when recorded on the order.</summary>
    public string? Phone { get; private set; }

    /// <summary>Creates a treating physician.</summary>
    /// <exception cref="InvalidPaymentException">
    /// The identifier is not positive or the code or name is empty.
    /// </exception>
    public static TreatingPhysician Create(int id, string code, string name)
    {
        if (id <= 0)
        {
            throw new InvalidPaymentException("A treating physician identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidPaymentException("A treating physician must have a code.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidPaymentException("A treating physician must have a name.");
        }

        return new TreatingPhysician
        {
            Id = id,
            Code = code.Trim(),
            Name = name.Trim(),
        };
    }

    /// <summary>Records the physician telephone number.</summary>
    public void SetPhone(string? phone) =>
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
}
