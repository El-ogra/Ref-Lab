using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Billing;

/// <summary>
/// A payment received against a patient order.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>payment</c> carries 10 columns — <c>patientid</c>,
/// <c>paiddate</c>, <c>type</c>, <c>gehah</c>, <c>testsimp</c>, <c>money</c>,
/// <c>mowzaf</c>, <c>editdate</c>, <c>comment</c> and <c>paymentnum</c>.
/// The value domain of <c>type</c> is not established, so it is carried as the
/// legacy integer code.
/// </remarks>
public sealed class Payment
{
    private Payment()
    {
    }

    /// <summary>The identity of the payment.</summary>
    public Guid Id { get; private set; }

    /// <summary>The order the payment settles.</summary>
    public Guid OrderId { get; private set; }

    /// <summary>The legacy composite identifier of the order being settled.</summary>
    public string? LegacyOrderIdentifier { get; private set; }

    /// <summary>The date the payment was received.</summary>
    public DateTimeOffset PaidDate { get; private set; }

    /// <summary>Legacy <c>type</c> — the payment type code.</summary>
    public int Type { get; private set; }

    /// <summary>Legacy <c>gehah</c> — the referring party code.</summary>
    public string? ReferringPartyCode { get; private set; }

    /// <summary>Legacy <c>testsimp</c> — the analyte the payment relates to.</summary>
    public string? TestSimpleName { get; private set; }

    /// <summary>The amount received.</summary>
    public Money Amount { get; private set; } = null!;

    /// <summary>Legacy <c>mowzaf</c> — the treating physician code.</summary>
    public string? TreatingPhysicianCode { get; private set; }

    /// <summary>Legacy <c>editdate</c> — the moment the payment was recorded.</summary>
    public DateTimeOffset? RecordedAt { get; private set; }

    /// <summary>Legacy <c>comment</c>.</summary>
    public string? Comment { get; private set; }

    /// <summary>Legacy <c>paymentnum</c> — the receipt sequence number.</summary>
    public long Number { get; private set; }

    /// <summary>Creates a payment.</summary>
    /// <exception cref="InvalidPaymentException">
    /// The order identity is empty or the amount is missing.
    /// </exception>
    public static Payment Create(
        Guid orderId,
        DateTimeOffset paidDate,
        Money amount,
        long number,
        int type = 0,
        string? referringPartyCode = null,
        string? testSimpleName = null,
        string? treatingPhysicianCode = null,
        string? legacyOrderIdentifier = null)
    {
        if (orderId == Guid.Empty)
        {
            throw new InvalidPaymentException("A payment must settle an order.");
        }

        ArgumentNullException.ThrowIfNull(amount);

        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            LegacyOrderIdentifier = legacyOrderIdentifier,
            PaidDate = paidDate,
            Type = type,
            ReferringPartyCode = Normalize(referringPartyCode),
            TestSimpleName = Normalize(testSimpleName),
            Amount = amount,
            TreatingPhysicianCode = Normalize(treatingPhysicianCode),
            RecordedAt = paidDate,
            Number = number,
        };
    }

    /// <summary>Records the receipt comment.</summary>
    public void SetComment(string? comment) =>
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
