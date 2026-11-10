using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.SendOut;

/// <summary>
/// The record of a sample that the laboratory refers to another laboratory.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: a test flagged <c>testtable.inotherlab</c> enters the
/// send-out list automatically. Each carries two independent states —
/// <c>sentout</c> (settled) and <c>send</c> (dispatched) — and a destination in
/// <c>otherlabname</c>, which may be made the standing default with the
/// legacy label <c>&#x062f;&#x0627;&#x0626;&#x0645;&#x0627; &#x0625;&#x0644;&#x064a;&#x0647;</c>.
/// </para>
/// <para>
/// Dispatch and payment are tracked separately so that an unsettled referral
/// stays visible.
/// </para>
/// </remarks>
public sealed class SendOutRequest
{
    private SendOutRequest()
    {
    }

    /// <summary>The identity of the request.</summary>
    public Guid Id { get; private set; }

    /// <summary>The order test being referred.</summary>
    public Guid OrderTestId { get; private set; }

    /// <summary>The destination laboratory.</summary>
    public string DestinationLabName { get; private set; } = string.Empty;

    /// <summary>Legacy <c>send</c> — the sample has been dispatched.</summary>
    public bool IsDispatched { get; private set; }

    /// <summary>Legacy <c>sentout</c> — the referral has been settled.</summary>
    public bool IsSettled { get; private set; }

    /// <summary>The moment the sample was dispatched.</summary>
    public DateTimeOffset? DispatchedAt { get; private set; }

    /// <summary>The moment the referral was settled.</summary>
    public DateTimeOffset? SettledAt { get; private set; }

    /// <summary>Legacy <c>testpriceout</c> — the price charged by the external laboratory.</summary>
    public decimal? OutPrice { get; private set; }

    /// <summary>Legacy <c>testpriceoutmadfo</c> — the same price before discount.</summary>
    public decimal? OutPriceBeforeDiscount { get; private set; }

    /// <summary>The combined settlement state.</summary>
    public SendOutState State => IsSettled
        ? SendOutState.Settled
        : IsDispatched
            ? SendOutState.Dispatched
            : SendOutState.Pending;

    /// <summary>Creates a send-out request.</summary>
    /// <exception cref="InvalidSendOutStateException">
    /// The order test identity is empty or the destination laboratory is not named.
    /// </exception>
    public static SendOutRequest Create(
        Guid orderTestId,
        string destinationLabName,
        decimal? outPrice = null,
        decimal? outPriceBeforeDiscount = null)
    {
        if (orderTestId == Guid.Empty)
        {
            throw new InvalidSendOutStateException("A send-out request must reference an order test.");
        }

        if (string.IsNullOrWhiteSpace(destinationLabName))
        {
            throw new InvalidSendOutStateException("A send-out request must name the destination laboratory.");
        }

        return new SendOutRequest
        {
            Id = Guid.NewGuid(),
            OrderTestId = orderTestId,
            DestinationLabName = destinationLabName.Trim(),
            OutPrice = outPrice,
            OutPriceBeforeDiscount = outPriceBeforeDiscount,
        };
    }

    /// <summary>Records that the sample has been dispatched.</summary>
    public void MarkDispatched(DateTimeOffset at)
    {
        if (IsSettled)
        {
            throw new InvalidSendOutStateException(
                "A settled referral cannot be dispatched again.");
        }

        IsDispatched = true;
        DispatchedAt = at;
    }

    /// <summary>Records that the referral has been settled.</summary>
    public void MarkSettled(DateTimeOffset at)
    {
        if (!IsDispatched)
        {
            throw new InvalidSendOutStateException(
                "A referral must be dispatched before it can be settled.");
        }

        IsSettled = true;
        SettledAt = at;
    }

    /// <summary>Changes the destination laboratory before the sample is dispatched.</summary>
    /// <exception cref="InvalidSendOutStateException">
    /// The sample has already been dispatched, or the name is empty.
    /// </exception>
    public void SetDestinationLab(string destinationLabName)
    {
        if (IsDispatched)
        {
            throw new InvalidSendOutStateException(
                "The destination of a dispatched sample can no longer be changed.");
        }

        if (string.IsNullOrWhiteSpace(destinationLabName))
        {
            throw new InvalidSendOutStateException("A send-out request must name the destination laboratory.");
        }

        DestinationLabName = destinationLabName.Trim();
    }

    /// <summary>Changes the send-out prices.</summary>
    public void SetPrices(decimal? outPrice, decimal? outPriceBeforeDiscount)
    {
        OutPrice = outPrice;
        OutPriceBeforeDiscount = outPriceBeforeDiscount;
    }
}
