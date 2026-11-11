using RefLab.Domain.Enums;
using RefLab.Domain.Patients;

namespace RefLab.Application.Features.Patients;

/// <summary>
/// A patient record as presented to the application layer.
/// </summary>
public sealed record PatientDto(
    Guid Id,
    string Name,
    Sex Sex,
    decimal? Age,
    AgeUnit? AgeUnit,
    DateOnly? BirthDate,
    string? Address,
    string? Phone,
    string? Mobile,
    string? Fax,
    string? Email,
    string? ReferringPhysicianCode,
    string? ReceptionNote)
{
    /// <summary>Projects a patient onto its DTO.</summary>
    public static PatientDto FromDomain(Patient patient) =>
        new(
            patient.Id.Value,
            patient.Name,
            patient.Sex,
            patient.Age,
            patient.AgeUnit,
            patient.BirthDate,
            patient.Address,
            patient.Phone,
            patient.Mobile,
            patient.Fax,
            patient.Email,
            patient.ReferringPhysicianCode,
            patient.ReceptionNote);
}

/// <summary>
/// A patient order as presented to the application layer.
/// </summary>
public sealed record PatientOrderDto(
    Guid Id,
    Guid PatientId,
    string? LegacyIdentifier,
    string? PersonKey,
    DateTimeOffset RequestDate,
    DateTimeOffset? OutDate,
    string? PriceMethod,
    string CurrencyCode,
    decimal? TotalPrice,
    decimal? TotalPriceAfterDiscount,
    decimal? CommissionBaseAmount,
    string? Barcode,
    int? ReferringPartyId,
    string? TreatingPhysicianCode,
    bool IsComplete,
    RecordStatus Status,
    IReadOnlyList<OrderTestDto> Tests)
{
    /// <summary>Projects an order onto its DTO.</summary>
    public static PatientOrderDto FromDomain(PatientOrder order) =>
        new(
            order.Id,
            order.PatientId.Value,
            order.LegacyIdentifier?.Value,
            order.LegacyIdentifier?.PersonKey,
            order.RequestDate,
            order.OutDate,
            order.PriceMethod,
            order.CurrencyCode,
            order.TotalPrice?.Amount,
            order.TotalPriceAfterDiscount?.Amount,
            order.CommissionBaseAmount?.Amount,
            order.Barcode,
            order.ReferringPartyId,
            order.TreatingPhysicianCode,
            order.IsComplete,
            order.Status,
            order.Tests.Select(OrderTestDto.FromDomain).ToArray());
}

/// <summary>
/// A requested test as presented to the application layer.
/// </summary>
/// <remarks>
/// The reference range, the normal range and the high/low flag are carried
/// separately because the result sheet shows them separately: a column for the
/// flag that reads high or low, a column for the range that reads as the
/// sentence the laboratory published ("Male Up To 40 U/L"), and a column for
/// the critical range. All three are read from the reference snapshot frozen
/// onto the order row, never from the live range table, so that a later change
/// to the catalogue cannot alter a result that has already been reported.
/// </remarks>
public sealed record OrderTestDto(
    Guid Id,
    Guid OrderId,
    int TestCode,
    string TestSimpleName,
    string? TestName,
    string? TestUnit,
    int SubGroupCode,
    int? MainTestCode,
    int ArrangementNumber,
    DateTimeOffset RequestDate,
    string? ResultText,
    decimal? ResultValue,
    IReadOnlyList<string?> AdditionalResults,
    string? Classification,
    string? ClassificationText,
    string? LowHighFlag,
    string? NormalRangeText,
    string? CriticalRangeText,
    ResultState State,
    bool IsReviewed,
    bool IsPrinted,
    bool IsGiven,
    long? PrintNumber,
    string? PrintedBy,
    DateTimeOffset? PrintedAt,
    string? Comment,
    decimal? Price)
{
    /// <summary>Projects a requested test onto its DTO.</summary>
    public static OrderTestDto FromDomain(OrderTest test) =>
        new(
            test.Id,
            test.OrderId,
            test.TestCode,
            test.TestSimpleName,
            test.TestName,
            test.TestUnit,
            test.SubGroupCode,
            test.MainTestCode,
            test.ArrangementNumber,
            test.RequestDate,
            test.Result?.Text,
            test.Result?.NumericValue,
            test.AdditionalResults,
            test.Classification?.ToString(),
            test.Classification is null
                ? null
                : test.ReferenceSnapshot?.Interval.ExceptionTextFor(test.Classification.Value),

            // The flag carries the two abnormal directions only. A normal or a
            // critical result is shown through the classification and the
            // exception text, not through a high/low flag.
            test.Classification is ResultClassification.High or ResultClassification.Low
                ? test.Classification.Value.ToString()
                : null,

            // The normal range is the human-readable sentence the laboratory
            // published for the analyte, frozen onto the order row.
            test.ReferenceSnapshot?.Interval.RangeText,
            test.ReferenceSnapshot?.Interval.CriticalInterval.CriticalRangeText,
            test.State,
            test.IsResultReviewed,
            test.IsResultPrinted,
            test.IsGiven,
            test.PrintNumber,
            test.PrintedBy,
            test.PrintedAt,
            test.Comment,
            test.Price?.Amount);
}

/// <summary>
/// The medical history sheet of a patient.
/// </summary>
public sealed record PatientHistoryDto(
    Guid PatientId,
    int Selector,
    DateTimeOffset RequestDate,
    bool PrintFlag,
    int UsedSlotCount,
    IReadOnlyDictionary<int, string> Slots)
{
    /// <summary>Projects a history sheet onto its DTO.</summary>
    public static PatientHistoryDto FromDomain(PatientHistory history)
    {
        var slots = new Dictionary<int, string>();

        for (var slot = 1; slot <= PatientHistory.SlotCount; slot++)
        {
            var value = history.GetSlot(slot);

            if (value is not null)
            {
                slots[slot] = value;
            }
        }

        return new PatientHistoryDto(
            history.PatientId.Value,
            history.Selector,
            history.RequestDate,
            history.PrintFlag,
            history.UsedSlotCount,
            slots);
    }
}

/// <summary>
/// A picture attached to a patient.
/// </summary>
public sealed record PatientPictureDto(
    Guid Id,
    Guid PatientId,
    Guid? OrderId,
    int? TestCode,
    int Slot,
    string ImagePath,
    string? Caption)
{
    /// <summary>Projects a patient picture onto its DTO.</summary>
    public static PatientPictureDto FromDomain(PatientPicture picture) =>
        new(
            picture.Id,
            picture.PatientId.Value,
            picture.OrderId,
            picture.TestCode,
            picture.Slot,
            picture.ImagePath,
            picture.Caption);
}

/// <summary>
/// The barcode allocated to a patient order.
/// </summary>
public sealed record BarcodeDto(Guid OrderId, string Barcode, DateTimeOffset GeneratedAt);

/// <summary>
/// One tracked case in the follow-up register.
/// </summary>
/// <remarks>
/// Legacy evidence: the vendor guide documents the capability
/// <c>&#x0645;&#x062a;&#x0627;&#x0628;&#x0639;&#x0629; &#x062a;&#x0633;&#x062c;&#x064a;&#x0644; &#x0627;&#x0644;&#x062d;&#x0627;&#x0644;&#x0627;&#x062a; &#x0648;&#x0627;&#x0644;&#x062a;&#x0639;&#x062f;&#x064a;&#x0644; &#x0639;&#x0644;&#x064a;&#x0647;&#x0627; &#x0648;&#x0625;&#x062f;&#x062e;&#x0627;&#x0644; &#x0627;&#x0644;&#x0646;&#x062a;&#x0627;&#x0626;&#x062c; &#x0648;&#x0637;&#x0628;&#x0627;&#x0639;&#x0629; &#x0627;&#x0644;&#x062a;&#x0642;&#x0627;&#x0629;&#x0631;</c>
/// — follow up the recorded cases, amend them, enter the results and print the reports.
/// </remarks>
public sealed record CaseFollowUpDto(
    Guid OrderId,
    Guid PatientId,
    string PatientName,
    string? LegacyIdentifier,
    DateTimeOffset RequestDate,
    RecordStatus Status,
    int RequestedTestCount,
    int ReviewedTestCount,
    int PrintedTestCount,
    int HandedOverTestCount,
    bool IsComplete,
    string NextAction);

/// <summary>
/// The legacy composite order identifier as presented to the application layer.
/// </summary>
public sealed record OrderIdentifierDto(
    string Value,
    string PersonKey,
    string DisplayCode,
    string TrailingCharacter);
