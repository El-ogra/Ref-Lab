using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Billing;

/// <summary>
/// A referring party — a clinic or a doctor that sends work to the laboratory.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>Gehats</c> carries 21 columns and 11 shipped rows. It
/// holds the party's identity, its contact details, its special-limit flag and
/// its commission rate in <c>omola</c>.
/// </para>
/// <para>
/// Version 2 split the physician out of the referring party: the
/// <c>ReferredPW</c> code and the <c>ReferredID</c> identifier now point at a
/// treating physician, and <c>patientinfo.PatRPW</c> carries the referring
/// physician on the order.
/// </para>
/// </remarks>
public sealed class ReferringParty
{
    private ReferringParty()
    {
    }

    /// <summary>The referring party identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The party code.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>The party name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The code of the responsible person at the party.</summary>
    public string? ContactPersonCode { get; private set; }

    /// <summary>The name of the responsible person at the party.</summary>
    public string? ContactPersonName { get; private set; }

    /// <summary>The city the party is located in.</summary>
    public string? City { get; private set; }

    /// <summary>The party's address.</summary>
    public string? Address { get; private set; }

    /// <summary>The party's telephone number.</summary>
    public string? Phone { get; private set; }

    /// <summary>The party's mobile number.</summary>
    public string? Mobile { get; private set; }

    /// <summary>The party's fax number.</summary>
    public string? Fax { get; private set; }

    /// <summary>The party's email address.</summary>
    public string? Email { get; private set; }

    /// <summary>The party's website.</summary>
    public string? Website { get; private set; }

    /// <summary>Whether the party is active.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Whether the party receives special commercial treatment.</summary>
    public bool IsSpecial { get; private set; }

    /// <summary>The special limit granted to the party.</summary>
    public decimal SpecialLimit { get; private set; }

    /// <summary>The commission rate earned by the party.</summary>
    public Percentage? CommissionRate { get; private set; }

    /// <summary>Legacy <c>morsel</c> — the party classification code.</summary>
    public int? Morsel { get; private set; }

    /// <summary>The account type applied to the party.</summary>
    public string? PriceMethod { get; private set; }

    /// <summary>The telephone number of the responsible person.</summary>
    public string? ContactPhone { get; private set; }

    /// <summary>Whether an order number is printed for this party.</summary>
    public bool PrintOrderNumber { get; private set; }

    /// <summary>The code of the treating physician the party refers to.</summary>
    public string? TreatingPhysicianCode { get; private set; }

    /// <summary>The identifier of the treating physician the party refers to.</summary>
    public int? TreatingPhysicianId { get; private set; }

    /// <summary>Creates a referring party.</summary>
    /// <exception cref="InvalidPaymentException">
    /// The identifier is not positive or the code or name is empty.
    /// </exception>
    public static ReferringParty Create(int id, string code, string name)
    {
        if (id <= 0)
        {
            throw new InvalidPaymentException("A referring party identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidPaymentException("A referring party must have a code.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidPaymentException("A referring party must have a name.");
        }

        return new ReferringParty
        {
            Id = id,
            Code = code.Trim(),
            Name = name.Trim(),
        };
    }

    /// <summary>Sets the commission rate earned by the party.</summary>
    public void SetCommissionRate(Percentage? commissionRate) => CommissionRate = commissionRate;

    /// <summary>Records the party's contact details.</summary>
    public void UpdateContactDetails(
        string? address = null,
        string? phone = null,
        string? mobile = null,
        string? fax = null,
        string? email = null,
        string? website = null,
        string? city = null)
    {
        Address = Normalize(address);
        Phone = Normalize(phone);
        Mobile = Normalize(mobile);
        Fax = Normalize(fax);
        Email = Normalize(email);
        Website = Normalize(website);
        City = Normalize(city);
    }

    /// <summary>Records the responsible person at the party.</summary>
    public void SetContactPerson(string? code, string? name, string? phone)
    {
        ContactPersonCode = Normalize(code);
        ContactPersonName = Normalize(name);
        ContactPhone = Normalize(phone);
    }

    /// <summary>Sets the special commercial treatment of the party.</summary>
    public void SetSpecialTreatment(bool isSpecial, decimal specialLimit)
    {
        IsSpecial = isSpecial;
        SpecialLimit = specialLimit;
    }

    /// <summary>Points the party at the treating physician it refers to.</summary>
    public void SetTreatingPhysician(string? code, int? physicianId)
    {
        TreatingPhysicianCode = Normalize(code);
        TreatingPhysicianId = physicianId;
    }

    /// <summary>Sets the account type applied to the party.</summary>
    public void SetPriceMethod(string? priceMethod) =>
        PriceMethod = Normalize(priceMethod);

    /// <summary>Sets whether an order number is printed for this party.</summary>
    public void SetPrintOrderNumber(bool printOrderNumber) => PrintOrderNumber = printOrderNumber;

    /// <summary>Deactivates the party so that it can no longer be selected.</summary>
    public void Deactivate() => IsActive = false;

    /// <summary>Reactivates the party.</summary>
    public void Activate() => IsActive = true;

    /// <summary>Sets the party classification code.</summary>
    public void SetMorsel(int? morsel) => Morsel = morsel;

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
