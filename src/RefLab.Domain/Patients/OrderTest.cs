using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Rules;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Patients;

/// <summary>
/// A single test requested on a patient order, together with its result and
/// its whole audit trail.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>patienttest</c> carries 46 columns and is the
/// transactional centre of the legacy system. It holds the requested test, the
/// frozen reference range, up to five result slots, the lifecycle flags
/// <c>resultfinsh</c>, <c>resultreview</c>, <c>resultprint</c>, <c>give</c>,
/// <c>sentout</c> and <c>send</c>, the pricing fields, the operational fields
/// and a complete audit set (<c>edituser</c>/<c>editdate</c>,
/// <c>prevuser</c>/<c>prevdate</c>, <c>printuser</c>/<c>printdate</c>,
/// <c>printnum</c>, <c>taslimuser</c>/<c>taslimdate</c>).
/// </para>
/// <para>
/// Every one of those flags is preserved verbatim. The additional lifecycle
/// state exposed by <see cref="State"/> is a projection, never a replacement.
/// </para>
/// </remarks>
public sealed class OrderTest
{
    private OrderTest()
    {
    }

    /// <summary>The identity of the requested test.</summary>
    public Guid Id { get; private set; }

    /// <summary>The order the test belongs to.</summary>
    public Guid OrderId { get; private set; }

    /// <summary>The requested test code.</summary>
    public int TestCode { get; private set; }

    /// <summary>The simplified analyte name, denormalised for reporting.</summary>
    public string TestSimpleName { get; private set; } = string.Empty;

    /// <summary>The abbreviation, denormalised for reporting.</summary>
    public string? TestAbbreviation { get; private set; }

    /// <summary>The test name, denormalised for reporting.</summary>
    public string? TestName { get; private set; }

    /// <summary>The history name, denormalised for reporting.</summary>
    public string? HistoryName { get; private set; }

    /// <summary>The measurement unit, denormalised for reporting.</summary>
    public string? TestUnit { get; private set; }

    /// <summary>The clinical sub-group the test belongs to.</summary>
    public int SubGroupCode { get; private set; }

    /// <summary>
    /// The parent test code when this test is a member of a test group.
    /// </summary>
    /// <remarks>
    /// Legacy evidence: <c>patienttestgroup</c> mirrors <c>patienttest</c> but
    /// is keyed by two codes — <c>maintestcode</c> and <c>testcode</c>. It is
    /// the only many-to-many structure in the legacy schema.
    /// </remarks>
    public int? MainTestCode { get; private set; }

    /// <summary>The ordering position of the test within the order.</summary>
    public int ArrangementNumber { get; private set; }

    /// <summary>The date the test was requested.</summary>
    public DateTimeOffset RequestDate { get; private set; }

    /// <summary>The entered result.</summary>
    public ResultValue? Result { get; private set; }

    /// <summary>The additional result slots <c>resultt1</c> to <c>resultt4</c>.</summary>
    public IReadOnlyList<string?> AdditionalResults { get; private set; } = Array.Empty<string?>();

    /// <summary>The reference range that was in force when the test was ordered.</summary>
    public ReferenceSnapshot? ReferenceSnapshot { get; private set; }

    /// <summary>Legacy <c>resultfinsh</c> — the result entry is complete.</summary>
    public bool IsResultFinished { get; private set; }

    /// <summary>Legacy <c>resultreview</c> — the result has been confirmed.</summary>
    public bool IsResultReviewed { get; private set; }

    /// <summary>Legacy <c>resultprint</c> — the result has been printed.</summary>
    public bool IsResultPrinted { get; private set; }

    /// <summary>Legacy <c>give</c> — the result has been handed over.</summary>
    public bool IsGiven { get; private set; }

    /// <summary>Legacy <c>sentout</c> — the send-out sample has been settled.</summary>
    public bool IsSentOut { get; private set; }

    /// <summary>Legacy <c>send</c> — the sample has been dispatched.</summary>
    public bool IsSent { get; private set; }

    /// <summary>Legacy <c>printwith</c> — the test is printed with its group.</summary>
    public bool PrintWithGroup { get; private set; }

    /// <summary>Legacy <c>RESULTCASEPRINT</c> — the printed case style.</summary>
    public string? ResultCasePrint { get; private set; }

    /// <summary>
    /// Whether the specimen was taken outside the laboratory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The reference system carries this as the standard comment of a result:
    /// the printed report shows <c>Comment : Sample taken outside lab.</c>
    /// when the specimen was collected away from the laboratory.
    /// </para>
    /// <para>
    /// The flag is recorded here rather than only as free text, so that the
    /// report can print the laboratory's own wording of it, and so that a
    /// laboratory can tell the two cases apart when it reads its own data.
    /// It is orthogonal to the result state machine: it survives every
    /// transition and gates none of them.
    /// </para>
    /// </remarks>
    public bool SampleTakenOutsideLab { get; private set; }

    /// <summary>The price charged for the test.</summary>
    public Money? Price { get; private set; }

    /// <summary>Legacy <c>testpriceout</c> — the send-out price.</summary>
    public decimal? OutPrice { get; private set; }

    /// <summary>Legacy <c>testpriceoutmadfo</c> — the send-out price before discount.</summary>
    public decimal? OutPriceBeforeDiscount { get; private set; }

    /// <summary>The external laboratory the sample was referred to.</summary>
    public string? OtherLabName { get; private set; }

    /// <summary>Legacy <c>testtime</c> — the turnaround time in minutes.</summary>
    public int? TestTime { get; private set; }

    /// <summary>Legacy <c>Timeout</c> — the per-test timeout.</summary>
    public string? Timeout { get; private set; }

    /// <summary>Legacy <c>Taketime</c> — the time actually taken.</summary>
    public int? TakeTime { get; private set; }

    /// <summary>Legacy <c>commentx</c> — the free-text result comment.</summary>
    public string? Comment { get; private set; }

    /// <summary>Legacy <c>loggroup</c> — the login group the test is filed under.</summary>
    public string? LogGroup { get; private set; }

    /// <summary>The operator who created the order row.</summary>
    public string? CreatedBy { get; private set; }

    /// <summary>The moment the order row was created.</summary>
    public DateTimeOffset? CreatedAt { get; private set; }

    /// <summary>The operator who last amended the order row.</summary>
    public string? EditedBy { get; private set; }

    /// <summary>The moment the order row was last amended.</summary>
    public DateTimeOffset? EditedAt { get; private set; }

    /// <summary>Legacy <c>prevuser</c> — the operator before the last amendment.</summary>
    public string? PreviousUser { get; private set; }

    /// <summary>Legacy <c>prevdate</c> — the moment before the last amendment.</summary>
    public DateTimeOffset? PreviousAt { get; private set; }

    /// <summary>Legacy <c>printuser</c> — the operator who printed the result.</summary>
    public string? PrintedBy { get; private set; }

    /// <summary>Legacy <c>printdate</c> — the moment the result was printed.</summary>
    public DateTimeOffset? PrintedAt { get; private set; }

    /// <summary>Legacy <c>printnum</c> — the report sequence number.</summary>
    public long? PrintNumber { get; private set; }

    /// <summary>Legacy <c>taslimuser</c> — the operator who handed the result over.</summary>
    public string? HandedOverBy { get; private set; }

    /// <summary>Legacy <c>taslimdate</c> — the moment the result was handed over.</summary>
    public DateTimeOffset? HandedOverAt { get; private set; }

    /// <summary>
    /// The lifecycle state of the result, derived from the legacy flags.
    /// </summary>
    public ResultState State
    {
        get
        {
            if (IsGiven)
            {
                return ResultState.Delivered;
            }

            if (IsResultPrinted)
            {
                return ResultState.Printed;
            }

            if (IsResultReviewed)
            {
                return ResultState.Reviewed;
            }

            return IsResultFinished ? ResultState.Entered : ResultState.NotEntered;
        }
    }

    /// <summary>
    /// The clinical classification of the entered result, or
    /// <see langword="null"/> when no classification is possible.
    /// </summary>
    public ResultClassification? Classification =>
        ResultClassificationPolicy.Classify(Result, ReferenceSnapshot);

    /// <summary>The send-out settlement state.</summary>
    public SendOutState SendOutState => IsSentOut
        ? SendOutState.Settled
        : IsSent
            ? SendOutState.Dispatched
            : SendOutState.Pending;

    /// <summary>Creates a requested test.</summary>
    /// <exception cref="InvalidPatientOrderException">
    /// The test code is not positive or the order identity is empty.
    /// </exception>
    public static OrderTest Create(
        Guid orderId,
        int testCode,
        string testSimpleName,
        int subGroupCode,
        DateTimeOffset requestDate,
        string createdBy,
        int arrangementNumber = 0,
        int? mainTestCode = null)
    {
        if (orderId == Guid.Empty)
        {
            throw new InvalidPatientOrderException("A requested test must belong to an order.");
        }

        if (testCode <= 0)
        {
            throw new InvalidPatientOrderException("A requested test must carry a positive test code.");
        }

        if (string.IsNullOrWhiteSpace(testSimpleName))
        {
            throw new InvalidPatientOrderException("A requested test must carry the analyte name.");
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new InvalidPatientOrderException("A requested test must record the operator who created it.");
        }

        return new OrderTest
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            TestCode = testCode,
            TestSimpleName = testSimpleName.Trim(),
            SubGroupCode = subGroupCode,
            MainTestCode = mainTestCode,
            ArrangementNumber = arrangementNumber,
            RequestDate = requestDate,
            CreatedBy = createdBy.Trim(),
            CreatedAt = requestDate,
            AdditionalResults = new string?[4],
        };
    }

    /// <summary>
    /// Enters the result value and marks the entry as complete.
    /// </summary>
    /// <exception cref="InvalidPatientOrderException">
    /// The result has already been confirmed as reviewed.
    /// </exception>
    public void EnterResult(ResultValue result, DateTimeOffset at, string editedBy)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (IsResultReviewed)
        {
            throw new InvalidPatientOrderException(
                "A result that has been confirmed as reviewed cannot be amended.");
        }

        Result = result;
        IsResultFinished = true;
        RecordAmendment(editedBy, at);
    }

    /// <summary>
    /// Writes one of the additional result slots.
    /// </summary>
    public void SetAdditionalResult(int slotIndex, string? value, DateTimeOffset at, string editedBy)
    {
        if (slotIndex is < 0 or > 3)
        {
            throw new InvalidPatientOrderException(
                "An additional result slot must be between 0 and 3.");
        }

        var slots = AdditionalResults.ToArray();
        slots[slotIndex] = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        AdditionalResults = slots;

        RecordAmendment(editedBy, at);
    }

    /// <summary>Attaches the frozen reference range to the order row.</summary>
    public void AttachReferenceSnapshot(ReferenceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ReferenceSnapshot = snapshot;
    }

    /// <summary>
    /// Confirms the result as reviewed.
    /// </summary>
    /// <exception cref="InvalidPatientOrderException">No result has been entered yet.</exception>
    public void ConfirmReviewed(DateTimeOffset at, string reviewedBy)
    {
        if (!IsResultFinished || Result is null)
        {
            throw new InvalidPatientOrderException("A result must be entered before it can be reviewed.");
        }

        IsResultReviewed = true;
        RecordAmendment(reviewedBy, at);
    }

    /// <summary>
    /// Records that the result has been printed.
    /// </summary>
    /// <exception cref="ResultNotReviewedException">
    /// The result has not been confirmed as reviewed.
    /// </exception>
    public void MarkPrinted(string printedBy, DateTimeOffset at, long printNumber)
    {
        RequireReviewed();

        IsResultPrinted = true;
        PrintedBy = printedBy.Trim();
        PrintedAt = at;
        PrintNumber = printNumber;
    }

    /// <summary>
    /// Records that the result has been handed over.
    /// </summary>
    /// <exception cref="ResultNotReviewedException">
    /// The result has not been confirmed as reviewed.
    /// </exception>
    public void MarkGiven(string handedOverBy, DateTimeOffset at)
    {
        RequireReviewed();

        IsGiven = true;
        HandedOverBy = handedOverBy.Trim();
        HandedOverAt = at;
    }

    /// <summary>Records that the sample has been dispatched to the external laboratory.</summary>
    public void MarkSent()
    {
        IsSent = true;
    }

    /// <summary>Records that the send-out sample has been settled.</summary>
    public void MarkSentOut()
    {
        IsSentOut = true;
    }

    /// <summary>Sets the send-out destination and the send-out prices.</summary>
    public void ConfigureSendOut(string? otherLabName, decimal? outPrice, decimal? outPriceBeforeDiscount)
    {
        OtherLabName = string.IsNullOrWhiteSpace(otherLabName) ? null : otherLabName.Trim();
        OutPrice = outPrice;
        OutPriceBeforeDiscount = outPriceBeforeDiscount;
    }

    /// <summary>Sets the price charged for the test.</summary>
    public void SetPrice(Money price)
    {
        ArgumentNullException.ThrowIfNull(price);
        Price = price;
    }

    /// <summary>Sets the free-text result comment.</summary>
    public void SetComment(string? comment, DateTimeOffset at, string editedBy)
    {
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        RecordAmendment(editedBy, at);
    }

    /// <summary>
    /// Records whether the specimen was taken outside the laboratory.
    /// </summary>
    public void SetSampleTakenOutsideLab(bool takenOutsideLab, DateTimeOffset at, string editedBy)
    {
        SampleTakenOutsideLab = takenOutsideLab;
        RecordAmendment(editedBy, at);
    }

    /// <summary>Sets the denormalised reporting snapshots.</summary>
    public void SetReportingSnapshots(
        string? testName = null,
        string? testAbbreviation = null,
        string? historyName = null,
        string? testUnit = null,
        string? logGroup = null)
    {
        TestName = Normalize(testName);
        TestAbbreviation = Normalize(testAbbreviation);
        HistoryName = Normalize(historyName);
        TestUnit = Normalize(testUnit);
        LogGroup = Normalize(logGroup);
    }

    /// <summary>Sets the operational timing fields.</summary>
    public void SetTiming(int? testTime, string? timeout, int? takeTime)
    {
        TestTime = testTime;
        Timeout = Normalize(timeout);
        TakeTime = takeTime;
    }

    /// <summary>
    /// Throws when the result is not eligible for release.
    /// </summary>
    /// <exception cref="ResultNotReviewedException">The result is not reviewed.</exception>
    public void RequireReviewed()
    {
        if (!IsResultReviewed)
        {
            throw new ResultNotReviewedException(TestCode);
        }
    }

    private void RecordAmendment(string editedBy, DateTimeOffset at)
    {
        PreviousUser = EditedBy;
        PreviousAt = EditedAt;
        EditedBy = editedBy.Trim();
        EditedAt = at;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
