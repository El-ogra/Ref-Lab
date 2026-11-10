using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Patients;

/// <summary>
/// A laboratory episode — one visit by one patient, carrying the tests that
/// were requested on it.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>patientinfo</c> carries 39 columns and is the
/// order/visit header — the demographics, the referral, the order-level
/// pricing and the totals. Its <c>patientid</c> is <em>not</em> a patient key
/// but a composite order identifier, reproduced here by
/// <see cref="LegacyIdentifier"/>.
/// </para>
/// <para>
/// The legacy <c>pricemethod</c> column holds the account type. Its value
/// domain is <em>not established</em>, so this entity carries it as free text
/// rather than as an invented enumeration.
/// </para>
/// </remarks>
public sealed class PatientOrder
{
    private readonly List<OrderTest> _tests = new();

    private PatientOrder()
    {
    }

    /// <summary>The identity of the order.</summary>
    public Guid Id { get; private set; }

    /// <summary>The patient the order belongs to.</summary>
    public PatientId PatientId { get; private set; }

    /// <summary>The legacy composite order identifier.</summary>
    public OrderIdentifier? LegacyIdentifier { get; private set; }

    /// <summary>The date the order was raised.</summary>
    public DateTimeOffset RequestDate { get; private set; }

    /// <summary>The date the order was closed, when known.</summary>
    public DateTimeOffset? OutDate { get; private set; }

    /// <summary>Legacy <c>timein</c> — the free-text arrival time.</summary>
    public string? TimeIn { get; private set; }

    /// <summary>The account type applied to the order.</summary>
    public string? PriceMethod { get; private set; }

    /// <summary>The currency of every amount on this order.</summary>
    public string CurrencyCode { get; private set; } = string.Empty;

    /// <summary>Legacy <c>totalprice</c> — the order total before discount.</summary>
    public Money? TotalPrice { get; private set; }

    /// <summary>Legacy <c>totalpricedisc</c> — the order total after discount.</summary>
    public Money? TotalPriceAfterDiscount { get; private set; }

    /// <summary>
    /// Legacy <c>elmadfoo3</c> — the base amount used by the referring party's
    /// commission formula.
    /// </summary>
    /// <remarks>
    /// The business meaning of this legacy column is not established. Only its
    /// role in the commission formula <c>SUM(omola * elmadfoo3 / 100)</c> is
    /// known, so the column is carried under a name that reflects that role
    /// rather than an assumed meaning.
    /// </remarks>
    public Money? CommissionBaseAmount { get; private set; }

    /// <summary>Legacy <c>discount</c> — the discount percentage applied.</summary>
    public Percentage? Discount { get; private set; }

    /// <summary>Legacy <c>discount100</c> — the companion discount value.</summary>
    public decimal? Discount100 { get; private set; }

    /// <summary>The referring party the order was received from.</summary>
    public int? ReferringPartyId { get; private set; }

    /// <summary>The treating physician recorded on the order.</summary>
    public string? TreatingPhysicianCode { get; private set; }

    /// <summary>The barcode allocated to the order.</summary>
    public string? Barcode { get; private set; }

    /// <summary>Legacy <c>completes</c> — the stored completion flag.</summary>
    public bool IsComplete { get; private set; }

    /// <summary>Legacy <c>recepnote</c> — the reception note.</summary>
    public string? ReceptionNote { get; private set; }

    /// <summary>Legacy <c>moreinfo</c> — the first additional-information field.</summary>
    public string? AdditionalInfo { get; private set; }

    /// <summary>Legacy <c>moreinfo2</c> — the second additional-information field.</summary>
    public string? AdditionalInfo2 { get; private set; }

    /// <summary>The operator who registered the order.</summary>
    public string? CreatedBy { get; private set; }

    /// <summary>The tests requested on this order.</summary>
    public IReadOnlyCollection<OrderTest> Tests => _tests;

    /// <summary>
    /// The lifecycle status of the order, derived from the requested tests.
    /// </summary>
    public RecordStatus Status => DeriveStatus();

    /// <summary>Creates a patient order.</summary>
    /// <exception cref="InvalidPatientOrderException">
    /// A mandatory value is missing or the identifier is outside the
    /// single-digit range.
    /// </exception>
    public static PatientOrder Create(
        PatientId patientId,
        DateTimeOffset requestDate,
        string currencyCode,
        string createdBy,
        OrderIdentifier? legacyIdentifier = null)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new InvalidPatientOrderException("A patient order must state its currency.");
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new InvalidPatientOrderException("A patient order must record the registering operator.");
        }

        return new PatientOrder
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            RequestDate = requestDate,
            CurrencyCode = currencyCode.Trim().ToUpperInvariant(),
            CreatedBy = createdBy.Trim(),
            LegacyIdentifier = legacyIdentifier,
        };
    }

    /// <summary>Attaches the legacy composite identifier to the order.</summary>
    public void AttachLegacyIdentifier(OrderIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        LegacyIdentifier = identifier;
    }

    /// <summary>Adds a requested test to the order.</summary>
    public void AddTest(OrderTest orderTest)
    {
        ArgumentNullException.ThrowIfNull(orderTest);

        if (orderTest.OrderId != Id)
        {
            throw new InvalidPatientOrderException(
                "The requested test does not belong to this order.");
        }

        _tests.Add(orderTest);
    }

    /// <summary>Removes a requested test from the order.</summary>
    public void RemoveTest(OrderTest orderTest)
    {
        ArgumentNullException.ThrowIfNull(orderTest);
        _tests.Remove(orderTest);
    }

    /// <summary>Records the arrival time of the patient.</summary>
    public void SetTimeIn(string? timeIn) =>
        TimeIn = string.IsNullOrWhiteSpace(timeIn) ? null : timeIn.Trim();

    /// <summary>Records the barcode allocated to the order.</summary>
    public void SetBarcode(string? barcode) =>
        Barcode = string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();

    /// <summary>Records the account type applied to the order.</summary>
    public void SetPriceMethod(string? priceMethod) =>
        PriceMethod = string.IsNullOrWhiteSpace(priceMethod) ? null : priceMethod.Trim();

    /// <summary>Records the referring party the order was received from.</summary>
    public void SetReferringParty(int? referringPartyId) => ReferringPartyId = referringPartyId;

    /// <summary>Records the treating physician on the order.</summary>
    public void SetTreatingPhysician(string? treatingPhysicianCode) =>
        TreatingPhysicianCode = string.IsNullOrWhiteSpace(treatingPhysicianCode)
            ? null
            : treatingPhysicianCode.Trim();

    /// <summary>Records the order totals and the commission base.</summary>
    public void SetAmounts(
        Money? totalPrice,
        Money? totalPriceAfterDiscount,
        Money? commissionBaseAmount = null,
        Percentage? discount = null,
        decimal? discount100 = null)
    {
        TotalPrice = totalPrice;
        TotalPriceAfterDiscount = totalPriceAfterDiscount;
        CommissionBaseAmount = commissionBaseAmount;
        Discount = discount;
        Discount100 = discount100;
    }

    /// <summary>Records the additional information fields.</summary>
    public void SetAdditionalInfo(string? additionalInfo, string? additionalInfo2)
    {
        AdditionalInfo = string.IsNullOrWhiteSpace(additionalInfo) ? null : additionalInfo.Trim();
        AdditionalInfo2 = string.IsNullOrWhiteSpace(additionalInfo2) ? null : additionalInfo2.Trim();
    }

    /// <summary>Records the reception note.</summary>
    public void SetReceptionNote(string? receptionNote) =>
        ReceptionNote = string.IsNullOrWhiteSpace(receptionNote) ? null : receptionNote.Trim();

    /// <summary>Closes the order.</summary>
    public void Close(DateTimeOffset outDate) => OutDate = outDate;

    /// <summary>Sets the stored completion flag and refreshes the derived status.</summary>
    public void MarkComplete(bool isComplete) => IsComplete = isComplete;

    /// <summary>
    /// Determines whether every requested test has been handed over.
    /// </summary>
    public bool IsFullyDelivered() =>
        _tests.Count > 0 && _tests.All(test => test.IsGiven);

    /// <summary>
    /// Determines whether every requested test has been reviewed.
    /// </summary>
    public bool IsFullyReviewed() =>
        _tests.Count > 0 && _tests.All(test => test.IsResultReviewed);

    private RecordStatus DeriveStatus()
    {
        if (_tests.Count == 0)
        {
            return RecordStatus.Open;
        }

        if (IsFullyDelivered())
        {
            return RecordStatus.Delivered;
        }

        if (IsFullyReviewed())
        {
            return RecordStatus.Completed;
        }

        return RecordStatus.InProgress;
    }
}
