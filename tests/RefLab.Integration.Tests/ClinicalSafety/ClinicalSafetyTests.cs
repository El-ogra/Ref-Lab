using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Application.Features.Identity;
using RefLab.Application.Features.Patients;
using RefLab.Application.Features.Results;
using RefLab.Domain.Auditing;
using RefLab.Domain.Billing;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;
using RefLab.Integration.Tests.Integration;

namespace RefLab.Integration.Tests.ClinicalSafety;

/// <summary>
/// The clinical-safety rules of the laboratory, exercised against the real
/// application and the real database.
/// </summary>
/// <remarks>
/// <para>
/// Section 8.2 of the Phase 0 discovery report lists the tests that are
/// mandatory for clinical safety. This class covers the classification
/// matrix, the precedence of the critical limits, the freezing of the
/// reference range and the review gate.
/// </para>
/// <para>
/// Every value used here is synthetic, and every reference range is a
/// synthetic range seeded by the fixture. No clinical limit is taken from a
/// real analyte, and none is invented as though it were one.
/// </para>
/// </remarks>
public sealed class ClinicalSafetyTests : IClassFixture<IntegrationTestHostFixture>
{
    private readonly IntegrationTestHostFixture _fixture;

    /// <summary>Initialises a new test class.</summary>
    /// <param name="fixture">The fixture that stands up the application.</param>
    public ClinicalSafetyTests(IntegrationTestHostFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>The host under test.</summary>
    private IntegrationTestHost Host => _fixture.Host;

    /// <summary>
    /// The classification is decided by the sex and by the age band of the
    /// patient, because the reference range that is frozen onto the order is
    /// selected on both.
    /// </summary>
    /// <remarks>
    /// The four ranges are seeded by the fixture: male 0–12 (2.5–6.5),
    /// female 0–12 (2.0–5.5), male 13–120 (3.5–7.5) and female 13–120
    /// (3.0–7.0).
    /// </remarks>
    /// <param name="sex">The sex of the patient.</param>
    /// <param name="age">The age of the patient, in years.</param>
    /// <param name="value">The value entered.</param>
    /// <param name="expected">The expected classification.</param>
    [Theory]
    [InlineData(Sex.Male, 8, "3.0", "Normal")]
    [InlineData(Sex.Male, 8, "7.0", "High")]
    [InlineData(Sex.Male, 8, "2.0", "Low")]
    [InlineData(Sex.Female, 8, "3.0", "Normal")]
    [InlineData(Sex.Female, 8, "7.0", "High")]
    [InlineData(Sex.Female, 8, "1.5", "Low")]
    [InlineData(Sex.Male, 30, "3.0", "Low")]
    [InlineData(Sex.Male, 30, "8.0", "High")]
    [InlineData(Sex.Female, 30, "3.0", "Normal")]
    [InlineData(Sex.Female, 30, "8.0", "High")]
    public async Task A_result_is_classified_against_the_frozen_range(
        Sex sex,
        int age,
        string value,
        string expected)
    {
        Host.EstablishSession(_fixture.OperatorId);

        var (_, orderTestId) = await _fixture
            .OrderOneTestAsync(SyntheticData.Unique(), sex, age, IntegrationTestHostFixture.SharedTestCode)
            ;

        var entered = await Host.Mediator.Send(new EnterResultCommand(orderTestId, value, "u"));

        Assert.Equal(expected, entered.Classification);
    }

    /// <summary>
    /// The same value classifies differently for a man and for a woman,
    /// because the reference range is scoped by sex.
    /// </summary>
    [Fact]
    public async Task The_same_value_classifies_differently_for_a_man_and_for_a_woman()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var (_, maleOrderTestId) = await _fixture
            .OrderOneTestAsync(suffix + "m", Sex.Male, 30, IntegrationTestHostFixture.SharedTestCode)
            ;

        var (_, femaleOrderTestId) = await _fixture
            .OrderOneTestAsync(suffix + "f", Sex.Female, 30, IntegrationTestHostFixture.SharedTestCode)
            ;

        var male = await Host.Mediator.Send(new EnterResultCommand(maleOrderTestId, "3.0", "u"));
        var female = await Host.Mediator.Send(new EnterResultCommand(femaleOrderTestId, "3.0", "u"));

        Assert.Equal("Low", male.Classification);
        Assert.Equal("Normal", female.Classification);
    }

    /// <summary>
    /// The same value classifies differently for a child and for an adult,
    /// because the reference range is scoped by age band.
    /// </summary>
    [Fact]
    public async Task The_same_value_classifies_differently_for_a_child_and_for_an_adult()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var (_, childOrderTestId) = await _fixture
            .OrderOneTestAsync(suffix + "c", Sex.Male, 8, IntegrationTestHostFixture.SharedTestCode)
            ;

        var (_, adultOrderTestId) = await _fixture
            .OrderOneTestAsync(suffix + "a", Sex.Male, 30, IntegrationTestHostFixture.SharedTestCode)
            ;

        var child = await Host.Mediator.Send(new EnterResultCommand(childOrderTestId, "3.0", "u"));
        var adult = await Host.Mediator.Send(new EnterResultCommand(adultOrderTestId, "3.0", "u"));

        Assert.Equal("Normal", child.Classification);
        Assert.Equal("Low", adult.Classification);
    }

    /// <summary>
    /// A value that breaches a critical limit is presented as critical, even
    /// though its abnormal classification would have been different.
    /// </summary>
    [Fact]
    public async Task A_critical_limit_overrides_the_abnormal_classification()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var (_, orderTestId) = await _fixture
            .OrderOneTestAsync(
                SyntheticData.Unique(),
                Sex.Male,
                40,
                IntegrationTestHostFixture.CriticalTestCode)
            ;

        // 6.8 is above the normal interval (2.5–6.5) but inside the
        // critical range (2.0–7.0).
        var high = await Host.Mediator.Send(new EnterResultCommand(orderTestId, "6.8", "u"));

        Assert.Equal("High", high.Classification);

        // 7.2 breaches the critical limit and is therefore critical.
        var critical = await Host.Mediator.Send(new EnterResultCommand(orderTestId, "7.2", "u"));

        Assert.Equal("Critical", critical.Classification);
    }

    /// <summary>
    /// The reference range is frozen onto the order row when the test is
    /// ordered, so that a later change of the catalogue does not touch a
    /// result that has already been recorded.
    /// </summary>
    /// <remarks>
    /// This is the rule the legacy system carried literally: the limits are
    /// copied onto the test row at the time of the request, and a later change
    /// to the range table leaves the recorded result alone.
    /// </remarks>
    [Fact]
    public async Task The_frozen_range_survives_a_later_change_of_the_catalogue_range()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var (_, orderTestId) = await _fixture
            .OrderOneTestAsync(suffix, Sex.Male, 30, IntegrationTestHostFixture.AmendableTestCode)
            ;

        var entered = await Host.Mediator.Send(new EnterResultCommand(orderTestId, "3.0", "u"));

        Assert.Equal("Low", entered.Classification);

        // The catalogue range for a man of 30 years is widened. The test
        // amends an analyte of its own, so that the change cannot reach the
        // other tests.
        var range = Host.ReadAll<ReferenceRange>()
            .Single(candidate =>
                candidate.TestCode == IntegrationTestHostFixture.AmendableTestCode &&
                candidate.SexScope == SexScope.Male &&
                candidate.AgeBand.From == 13m);

        await Host.Mediator.Send(new UpdateReferenceRangeCommand(
            range.Id,
            LowLimit: 1m,
            HighLimit: 20m));

        // The recorded test still carries the range that was in force.
        var stored = Host.ReadOne<OrderTest>(candidate => candidate.Id == orderTestId);

        Assert.NotNull(stored);
        Assert.NotNull(stored!.ReferenceSnapshot);
        Assert.Equal(3.5m, stored.ReferenceSnapshot!.Interval.LowLimit);
        Assert.Equal(7.5m, stored.ReferenceSnapshot.Interval.HighLimit);

        // And its classification is unchanged.
        var reread = Host.ReadOne<OrderTest>(candidate => candidate.Id == orderTestId);

        Assert.Equal("Low", reread!.Classification?.ToString());

        // A test ordered after the change receives the new range.
        var (_, laterOrderTestId) = await _fixture
            .OrderOneTestAsync(suffix + "later", Sex.Male, 30, IntegrationTestHostFixture.AmendableTestCode)
            ;

        var laterEntered = await Host.Mediator.Send(new EnterResultCommand(laterOrderTestId, "3.0", "u"));

        Assert.Equal("Normal", laterEntered.Classification);
    }

    /// <summary>
    /// A result for which the catalogue carries no range cannot be
    /// classified, and the application says so rather than guessing.
    /// </summary>
    [Fact]
    public async Task A_result_without_a_frozen_range_carries_no_classification()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var (_, orderTestId) = await _fixture
            .OrderOneTestAsync(
                SyntheticData.Unique(),
                Sex.Male,
                30,
                IntegrationTestHostFixture.UnclassifiedTestCode)
            ;

        var entered = await Host.Mediator.Send(new EnterResultCommand(orderTestId, "5.0", "u"));

        Assert.Null(entered.Classification);
        Assert.Null(entered.ClassificationText);
        Assert.Equal("5.0", entered.ResultText);
    }

    /// <summary>
    /// A result that has been confirmed as reviewed cannot be amended.
    /// </summary>
    [Fact]
    public async Task A_reviewed_result_cannot_be_amended()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var (_, orderTestId) = await _fixture
            .OrderOneTestAsync(SyntheticData.Unique(), Sex.Male, 30, IntegrationTestHostFixture.SharedTestCode)
            ;

        await Host.Mediator.Send(new EnterResultCommand(orderTestId, "5.0", "u"));
        await Host.Mediator.Send(new ReviewResultCommand(orderTestId));

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => Host.Mediator.Send(new EnterResultCommand(orderTestId, "6.0", "u")));

        Assert.Contains("cannot be amended", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A result cannot be reviewed before it has been entered.
    /// </summary>
    [Fact]
    public async Task A_result_cannot_be_reviewed_before_it_has_been_entered()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var (_, orderTestId) = await _fixture
            .OrderOneTestAsync(SyntheticData.Unique(), Sex.Male, 30, IntegrationTestHostFixture.SharedTestCode)
            ;

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => Host.Mediator.Send(new ReviewResultCommand(orderTestId)));

        Assert.Contains("must be entered before it can be reviewed", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The tests of one order are numbered consecutively, starting at zero.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The arrangement number is the position of the test inside the order.
    /// It is computed from the tests that the order already carries, and the
    /// test therefore pins down that the count is taken from the persisted
    /// state of the order rather than from an in-memory collection that the
    /// database has not populated.
    /// </para>
    /// <para>
    /// The order is read back through a context that has not seen it before,
    /// which is the situation the desktop application meets whenever the
    /// order was loaded after the session began.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_tests_of_one_order_are_numbered_consecutively()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var order = await Host.Mediator.Send(new RegisterPatientCommand(
            SyntheticData.PatientName(suffix),
            Sex.Male,
            30,
            AgeUnit.Years,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "EGP",
            CreateOrder: true));

        var first = Assert.Single(await Host.Mediator.Send(new OrderTestsCommand(
            order.Id,
            new[] { IntegrationTestHostFixture.SharedTestCode })));

        Assert.Equal(0, first.ArrangementNumber);

        var second = Assert.Single(await Host.Mediator.Send(new OrderTestsCommand(
            order.Id,
            new[] { IntegrationTestHostFixture.CriticalTestCode })));

        Assert.Equal(1, second.ArrangementNumber);
    }

    /// <summary>
    /// The reference range that is frozen onto one order does not corrupt the
    /// range of another order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The snapshot is a value object that belongs to the order row. Two
    /// orders may capture the same catalogue range, and each of them must own
    /// its own copy; a shared instance would leave one of the two orders with
    /// an interval that the database has already written under the other.
    /// </para>
    /// <para>
    /// The test orders the same analyte on two different patients and then
    /// reads both order rows back, checking that each one carries its own
    /// limits.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task Two_orders_that_capture_the_same_catalogue_range_keep_their_own_limits()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var (_, maleOrderTestId) = await _fixture
            .OrderOneTestAsync(suffix + "m", Sex.Male, 30, IntegrationTestHostFixture.SharedTestCode)
            ;

        var (_, femaleOrderTestId) = await _fixture
            .OrderOneTestAsync(suffix + "f", Sex.Female, 30, IntegrationTestHostFixture.SharedTestCode)
            ;

        var male = Host.ReadOne<OrderTest>(candidate => candidate.Id == maleOrderTestId);
        var female = Host.ReadOne<OrderTest>(candidate => candidate.Id == femaleOrderTestId);

        Assert.NotNull(male);
        Assert.NotNull(female);
        Assert.NotNull(male!.ReferenceSnapshot);
        Assert.NotNull(female!.ReferenceSnapshot);

        // The man receives the male range and the woman the female range,
        // and neither row is overwritten by the other.
        Assert.Equal(3.5m, male.ReferenceSnapshot!.Interval.LowLimit);
        Assert.Equal(7.5m, male.ReferenceSnapshot.Interval.HighLimit);
        Assert.Equal(3.0m, female.ReferenceSnapshot!.Interval.LowLimit);
        Assert.Equal(7.0m, female.ReferenceSnapshot.Interval.HighLimit);

        // The two snapshots do not share one instance.
        Assert.NotSame(male.ReferenceSnapshot!.Interval, female.ReferenceSnapshot!.Interval);
        Assert.NotSame(male.ReferenceSnapshot!.AgeBand, female.ReferenceSnapshot!.AgeBand);
    }

    /// <summary>
    /// The referring party's commission cannot be computed, because nothing
    /// in the Application layer ever writes the commission base of an order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ⚠ <b>This test documents an application gap. It does not certify the
    /// feature.</b> Section 8.2 of the discovery report lists the referral
    /// commission, <c>Σ(omola × base / 100)</c>, as a mandatory clinical
    /// test. The domain and the query exist, and the domain computes the
    /// total correctly, but <c>PatientOrder.SetAmounts</c> is never called by
    /// any handler, so <c>CommissionBaseAmount</c> is always null and the
    /// query always refuses.
    /// </para>
    /// <para>
    /// The test therefore asserts the behaviour that exists, so that the day
    /// the gap is closed the change is noticed, and the report records the
    /// feature as <b>NOT IMPLEMENTED</b> rather than as passing.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_referral_commission_cannot_be_computed_because_the_base_is_never_written()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var partyId = SyntheticData.NextCatalogueId();

        var party = ReferringParty.Create(partyId, $"SYN-RP-{suffix}", $"Synthetic referring party {suffix}");
        party.SetCommissionRate(Percentage.Create(10m));

        var unitOfWork = (Application.Abstractions.IUnitOfWork)Host.Context;
        unitOfWork.Add(party);

        var order = await Host.Mediator.Send(new RegisterPatientCommand(
            SyntheticData.PatientName(suffix),
            Sex.Male,
            30,
            AgeUnit.Years,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "EGP",
            CreateOrder: true));

        await unitOfWork.SaveChangesAsync();

        var stored = Host.ReadOne<PatientOrder>(candidate => candidate.Id == order.Id);

        Assert.NotNull(stored);
        Assert.Null(stored!.CommissionBaseAmount);
        Assert.Null(stored.TotalPrice);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => Host.Mediator.Send(new Application.Features.Billing.ComputeCommissionQuery(
                partyId,
                new[] { order.Id })));

        Assert.Contains("commission base amount", exception.Message, StringComparison.Ordinal);
    }
}
