using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.Patients;
using RefLab.Domain.Rules;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Patients;

/// <summary>
/// The requested test is the transactional centre of the legacy system. Its
/// lifecycle flags, its frozen reference range and its release gate are
/// preserved exactly.
/// </summary>
public class OrderTestTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    private static OrderTest CreateOrderTest()
    {
        var orderId = Guid.NewGuid();
        return OrderTest.Create(orderId, 56, "Hgb", 11, Moment, "lab.tech");
    }

    private static ReferenceSnapshot CreateSnapshot(
        decimal? low = 12m,
        decimal? high = 16m,
        decimal? lowCritical = null,
        decimal? highCritical = null)
    {
        return ReferenceSnapshot.Capture(
            56,
            "Hgb",
            SexScope.Male,
            AgeBand.Create(0m, 120m, AgeUnit.Years),
            ReferenceInterval.Create(
                lowLimit: low,
                highLimit: high,
                criticalInterval: CriticalInterval.Create(lowCritical, highCritical)),
            Moment);
    }

    [Fact]
    public void Create_stores_the_requested_test()
    {
        var test = CreateOrderTest();

        Assert.Equal(56, test.TestCode);
        Assert.Equal("Hgb", test.TestSimpleName);
        Assert.Equal(11, test.SubGroupCode);
        Assert.Equal("lab.tech", test.CreatedBy);
        Assert.Equal(ResultState.NotEntered, test.State);
    }

    [Fact]
    public void Create_rejects_a_missing_order()
    {
        Assert.Throws<InvalidPatientOrderException>(() =>
            OrderTest.Create(Guid.Empty, 56, "Hgb", 11, Moment, "lab.tech"));
    }

    [Fact]
    public void Create_rejects_a_missing_operator()
    {
        Assert.Throws<InvalidPatientOrderException>(() =>
            OrderTest.Create(Guid.NewGuid(), 56, "Hgb", 11, Moment, "  "));
    }

    [Fact]
    public void EnterResult_stores_the_value_and_marks_the_entry_complete()
    {
        var test = CreateOrderTest();

        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");

        Assert.Equal(13.4m, test.Result!.NumericValue);
        Assert.True(test.IsResultFinished);
        Assert.Equal(ResultState.Entered, test.State);
    }

    [Fact]
    public void ConfirmReviewed_marks_the_result_as_reviewed()
    {
        var test = CreateOrderTest();
        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");

        test.ConfirmReviewed(Moment.AddMinutes(1), "lab.tech");

        Assert.True(test.IsResultReviewed);
        Assert.Equal(ResultState.Reviewed, test.State);
    }

    [Fact]
    public void ConfirmReviewed_of_a_result_that_was_never_entered_is_rejected()
    {
        var test = CreateOrderTest();

        Assert.Throws<InvalidPatientOrderException>(() =>
            test.ConfirmReviewed(Moment, "lab.tech"));
    }

    [Fact]
    public void EnterResult_on_a_reviewed_result_is_rejected()
    {
        var test = CreateOrderTest();
        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");
        test.ConfirmReviewed(Moment, "lab.tech");

        Assert.Throws<InvalidPatientOrderException>(() =>
            test.EnterResult(ResultValue.FromNumber(9m), Moment, "lab.tech"));
    }

    [Fact]
    public void Printing_an_unreviewed_result_is_rejected()
    {
        var test = CreateOrderTest();
        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");

        var exception = Assert.Throws<ResultNotReviewedException>(() =>
            test.MarkPrinted("lab.tech", Moment, 1984));

        Assert.Equal(56, exception.TestCode);
        Assert.False(test.IsResultPrinted);
    }

    [Fact]
    public void Handing_over_an_unreviewed_result_is_rejected()
    {
        var test = CreateOrderTest();

        Assert.Throws<ResultNotReviewedException>(() => test.MarkGiven("reception", Moment));
    }

    [Fact]
    public void Printing_a_reviewed_result_records_the_audit()
    {
        var test = CreateOrderTest();
        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");
        test.ConfirmReviewed(Moment, "lab.tech");

        test.MarkPrinted("lab.tech", Moment.AddMinutes(2), 1984);

        Assert.True(test.IsResultPrinted);
        Assert.Equal("lab.tech", test.PrintedBy);
        Assert.Equal(1984, test.PrintNumber);
        Assert.Equal(ResultState.Printed, test.State);
    }

    [Fact]
    public void Handing_over_a_reviewed_result_records_the_audit()
    {
        var test = CreateOrderTest();
        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");
        test.ConfirmReviewed(Moment, "lab.tech");

        test.MarkGiven("reception", Moment.AddMinutes(3));

        Assert.True(test.IsGiven);
        Assert.Equal("reception", test.HandedOverBy);
        Assert.Equal(ResultState.Delivered, test.State);
    }

    [Fact]
    public void The_classification_uses_the_frozen_reference_range()
    {
        var test = CreateOrderTest();
        test.AttachReferenceSnapshot(CreateSnapshot(low: 12m, high: 16m));
        test.EnterResult(ResultValue.FromNumber(18m), Moment, "lab.tech");

        Assert.Equal(ResultClassification.High, test.Classification);
    }

    [Fact]
    public void A_critical_value_overrides_the_abnormal_classification()
    {
        var test = CreateOrderTest();
        test.AttachReferenceSnapshot(CreateSnapshot(low: 12m, high: 16m, highCritical: 20m));
        test.EnterResult(ResultValue.FromNumber(18m), Moment, "lab.tech");

        Assert.Equal(ResultClassification.High, test.Classification);

        test.EnterResult(ResultValue.FromNumber(25m), Moment, "lab.tech");

        Assert.Equal(ResultClassification.Critical, test.Classification);
    }

    [Fact]
    public void A_result_without_a_frozen_range_has_no_classification()
    {
        var test = CreateOrderTest();
        test.EnterResult(ResultValue.FromNumber(18m), Moment, "lab.tech");

        Assert.Null(test.Classification);
    }

    [Fact]
    public void The_send_out_state_follows_the_two_legacy_flags()
    {
        var test = CreateOrderTest();

        Assert.Equal(SendOutState.Pending, test.SendOutState);

        test.MarkSent();
        Assert.Equal(SendOutState.Dispatched, test.SendOutState);

        test.MarkSentOut();
        Assert.Equal(SendOutState.Settled, test.SendOutState);
    }

    [Fact]
    public void An_amendment_records_the_previous_operator()
    {
        var test = CreateOrderTest();

        test.EnterResult(ResultValue.FromNumber(13.4m), Moment, "lab.tech");

        Assert.Equal("lab.tech", test.EditedBy);
        Assert.Null(test.PreviousUser);

        test.EnterResult(ResultValue.FromNumber(13.5m), Moment.AddMinutes(1), "second.tech");

        Assert.Equal("second.tech", test.EditedBy);
        Assert.Equal("lab.tech", test.PreviousUser);
    }

    [Fact]
    public void The_additional_result_slots_hold_four_values()
    {
        var test = CreateOrderTest();

        test.SetAdditionalResult(0, "value one", Moment, "lab.tech");
        test.SetAdditionalResult(3, "value four", Moment, "lab.tech");

        Assert.Equal("value one", test.AdditionalResults[0]);
        Assert.Equal("value four", test.AdditionalResults[3]);
        Assert.Null(test.AdditionalResults[1]);
    }

    [Fact]
    public void An_additional_result_slot_outside_the_range_is_rejected()
    {
        var test = CreateOrderTest();

        Assert.Throws<InvalidPatientOrderException>(() =>
            test.SetAdditionalResult(4, "value", Moment, "lab.tech"));
    }
}
