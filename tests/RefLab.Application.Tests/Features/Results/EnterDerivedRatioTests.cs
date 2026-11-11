using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Results;

/// <summary>
/// A ratio row is an ordinary requested test: the value on it is computed from
/// the two results already on the order, and it is classified against its own
/// frozen reference interval.
/// </summary>
public class EnterDerivedRatioTests
{
    private const int NumeratorTestCode = 201;
    private const int DenominatorTestCode = 202;
    private const int RatioTestCode = 203;

    private static (PatientOrder Order, OrderTest Numerator, OrderTest Denominator, OrderTest Ratio) SeedOrder(
        ApplicationTestHost host,
        bool enterNumerator = true,
        bool enterDenominator = true)
    {
        var @operator = TestData.CreateOperator();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);

        var numerator = TestData.CreateOrderTest(order, NumeratorTestCode, "SGOT (AST)");
        var denominator = TestData.CreateOrderTest(order, DenominatorTestCode, "SGPT (ALT)");
        var ratio = TestData.CreateOrderTest(order, RatioTestCode, "AST/ALT ratio");

        if (enterNumerator)
        {
            numerator.EnterResult(ResultValue.FromNumber(41m), TestData.Now, "lab.tech");
        }

        if (enterDenominator)
        {
            denominator.EnterResult(ResultValue.FromNumber(12m), TestData.Now, "lab.tech");
        }

        host.Db.Seed(@operator, patient, order, numerator, denominator, ratio);

        return (order, numerator, denominator, ratio);
    }

    [Fact]
    public async Task The_derived_ratio_is_entered_on_the_ratio_row()
    {
        using var host = new ApplicationTestHost();
        var (order, _, _, _) = SeedOrder(host);

        var result = await host.Sender.Send(
            new EnterDerivedRatioCommand(order.Id, NumeratorTestCode, DenominatorTestCode, RatioTestCode),
            CancellationToken.None);

        Assert.Equal(RatioTestCode, result.TestCode);
        Assert.Equal("3.42", result.ResultText);
        Assert.Equal(3.42m, result.ResultValue);
    }

    [Fact]
    public async Task A_ratio_row_without_a_frozen_range_is_not_classified()
    {
        using var host = new ApplicationTestHost();
        var (order, _, _, _) = SeedOrder(host);

        var result = await host.Sender.Send(
            new EnterDerivedRatioCommand(order.Id, NumeratorTestCode, DenominatorTestCode, RatioTestCode),
            CancellationToken.None);

        Assert.Null(result.Classification);
        Assert.Null(result.LowHighFlag);
    }

    [Fact]
    public async Task The_derived_ratio_is_classified_against_the_frozen_range_of_the_ratio_row()
    {
        using var host = new ApplicationTestHost();
        var (order, _, _, ratio) = SeedOrder(host);

        ratio.AttachReferenceSnapshot(ReferenceSnapshot.Capture(
            ReferenceRange.Create(
                id: 1,
                testCode: RatioTestCode,
                testSimpleName: "AST/ALT ratio",
                subGroupCode: 11,
                sexScope: SexScope.All,
                ageBand: AgeBand.Create(0m, 120m, AgeUnit.Years),
                interval: ReferenceInterval.Create(
                    lowLimit: 0.5m,
                    highLimit: 2m,
                    rangeText: "Up To 2")),
            TestData.Now));

        var result = await host.Sender.Send(
            new EnterDerivedRatioCommand(order.Id, NumeratorTestCode, DenominatorTestCode, RatioTestCode),
            CancellationToken.None);

        Assert.Equal("High", result.Classification);
        Assert.Equal("High", result.LowHighFlag);
        Assert.Equal("Up To 2", result.NormalRangeText);
    }

    [Fact]
    public async Task A_ratio_cannot_be_derived_when_the_numerator_has_no_result()
    {
        using var host = new ApplicationTestHost();
        var (order, _, _, _) = SeedOrder(host, enterNumerator: false);

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new EnterDerivedRatioCommand(order.Id, NumeratorTestCode, DenominatorTestCode, RatioTestCode),
                CancellationToken.None));
    }

    [Fact]
    public async Task A_ratio_cannot_be_derived_when_the_denominator_has_no_result()
    {
        using var host = new ApplicationTestHost();
        var (order, _, _, _) = SeedOrder(host, enterDenominator: false);

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new EnterDerivedRatioCommand(order.Id, NumeratorTestCode, DenominatorTestCode, RatioTestCode),
                CancellationToken.None));
    }

    [Fact]
    public async Task A_test_that_is_not_on_the_order_is_rejected()
    {
        using var host = new ApplicationTestHost();
        var (order, _, _, _) = SeedOrder(host);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(
                new EnterDerivedRatioCommand(order.Id, 999, DenominatorTestCode, RatioTestCode),
                CancellationToken.None));
    }
}
