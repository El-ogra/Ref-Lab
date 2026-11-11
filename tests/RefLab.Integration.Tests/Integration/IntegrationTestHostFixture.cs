using RefLab.Application.Abstractions;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.ValueObjects;
using RefLab.Infrastructure.Security;

namespace RefLab.Integration.Tests.Integration;

/// <summary>
/// The fixture that stands up the application under test and seeds the
/// synthetic clinical knowledge the suite shares.
/// </summary>
/// <remarks>
/// <para>
/// The fixture is used as a class fixture, so every test class gets its own
/// host and its own database. The tests inside one class therefore share the
/// long-lived context — exactly as the desktop application does — while the
/// classes never interfere with one another.
/// </para>
/// <para>
/// All seeded data is synthetic. The catalogue rows carry the code range
/// 5001 upwards and the names carry the <c>SYNTHETIC</c> prefix, so that a row
/// that ever escaped into a laboratory database would be recognisable.
/// </para>
/// </remarks>
public sealed class IntegrationTestHostFixture : IAsyncLifetime
{
    /// <summary>
    /// The code of the synthetic analyte whose reference ranges are scoped by
    /// sex and by age band.
    /// </summary>
    public const int SharedTestCode = 5001;

    /// <summary>
    /// The code of the synthetic analyte that carries critical limits.
    /// </summary>
    public const int CriticalTestCode = 5002;

    /// <summary>
    /// The code of the synthetic analyte that carries no reference range at
    /// all, so that the behaviour of an unclassified result can be verified.
    /// </summary>
    public const int UnclassifiedTestCode = 5003;

    /// <summary>
    /// The code of the synthetic analyte that the freezing test amends.
    /// </summary>
    /// <remarks>
    /// The freezing test changes the limits of the range of this analyte. It
    /// therefore has an analyte of its own, so that the change cannot reach
    /// the tests that share <see cref="SharedTestCode"/>.
    /// </remarks>
    public const int AmendableTestCode = 5004;

    private IntegrationTestHost? _host;

    /// <summary>The host under test.</summary>
    public IntegrationTestHost Host =>
        _host ?? throw new InvalidOperationException("The fixture has not been initialised.");

    /// <summary>The identifier of the seeded operator.</summary>
    public int OperatorId { get; private set; }

    /// <summary>The name of the seeded operator.</summary>
    public string OperatorName { get; private set; } = string.Empty;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _host = await IntegrationTestHost.CreateAsync();

        // The run states which provider it used and which database it
        // created. The console is captured by the test host and appears in
        // the output, so that the result of a run can be read without
        // guessing from the environment.
        Console.WriteLine($"The integration tests run against {_host.ProviderName}.");

        if (_host.DatabaseName is not null)
        {
            Console.WriteLine(
                $"The harness created the database {_host.DatabaseName} and will drop it when the run ends.");
        }

        await SeedOperatorAsync();
        await SeedCatalogueAsync();
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        if (_host is not null)
        {
            await _host.DisposeAsync();

            _host = null;
        }
    }

    /// <summary>
    /// Orders one synthetic test onto a synthetic patient and returns the
    /// identifiers the caller needs.
    /// </summary>
    /// <param name="suffix">A unique fragment.</param>
    /// <param name="sex">The sex of the synthetic patient.</param>
    /// <param name="age">The age of the synthetic patient, in years.</param>
    /// <param name="testCode">The synthetic analyte to order.</param>
    /// <returns>The order identifier and the order-test identifier.</returns>
    public async Task<(Guid OrderId, Guid OrderTestId)> OrderOneTestAsync(
        string suffix,
        Sex sex,
        int age,
        int testCode)
    {
        var patientName = SyntheticData.PatientName(suffix);

        var order = await Host.Mediator.Send(new Application.Features.Patients.RegisterPatientCommand(
            patientName,
            sex,
            age,
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

        var ordered = await Host.Mediator.Send(
            new Application.Features.Results.OrderTestsCommand(order.Id, new[] { testCode }));

        var orderTest = Assert.Single(ordered);

        return (order.Id, orderTest.Id);
    }

    private async Task SeedOperatorAsync()
    {
        OperatorId = SyntheticData.NextOperatorId();
        OperatorName = $"p6.operator.{OperatorId}";

        var @operator = Operator.Create(
            OperatorId,
            OperatorName,
            new Pbkdf2PasswordHasher().Hash(SyntheticData.OperatorPassword),
            "Phase 6 integration operator");

        foreach (var permission in Enum.GetValues<Permission>())
        {
            @operator.Grant(permission);
        }

        var unitOfWork = (IUnitOfWork)Host.Context;

        unitOfWork.Add(@operator);

        await unitOfWork.SaveChangesAsync();
    }

    private async Task SeedCatalogueAsync()
    {
        var unitOfWork = (IUnitOfWork)Host.Context;

        var albumin = TestDefinition.Create(SharedTestCode, "SYNTHETIC ALBUMIN", "SYNALB", 1);
        albumin.UpdatePrices(150m, 60m, 0m);
        albumin.UpdateDetails(abbreviation: "SYNALB", sampleType: "Serum");

        var critical = TestDefinition.Create(CriticalTestCode, "SYNTHETIC CRITICAL ANALYTE", "SYNCRT", 1);
        critical.UpdatePrices(150m, 60m, 0m);
        critical.UpdateDetails(abbreviation: "SYNCRT", sampleType: "Serum");

        var unclassified = TestDefinition.Create(UnclassifiedTestCode, "SYNTHETIC UNCLASSIFIED", "SYNUNC", 1);
        unclassified.UpdatePrices(150m, 60m, 0m);
        unclassified.UpdateDetails(abbreviation: "SYNUNC", sampleType: "Serum");

        var amendable = TestDefinition.Create(AmendableTestCode, "SYNTHETIC AMENDABLE", "SYNAMD", 1);
        amendable.UpdatePrices(150m, 60m, 0m);
        amendable.UpdateDetails(abbreviation: "SYNAMD", sampleType: "Serum");

        unitOfWork.Add(albumin);
        unitOfWork.Add(critical);
        unitOfWork.Add(unclassified);
        unitOfWork.Add(amendable);

        // Four ranges, one per combination of sex and age band, with
        // different limits. The same value must therefore classify
        // differently for a man and for a woman, and differently for a child
        // and for an adult. A single unbounded, sex-agnostic range would make
        // the sex and the age of the test irrelevant.
        AddRange(unitOfWork, SharedTestCode, "SYNALB", SexScope.Male, 0m, 12m, 2.5m, 6.5m, null, null);
        AddRange(unitOfWork, SharedTestCode, "SYNALB", SexScope.Female, 0m, 12m, 2.0m, 5.5m, null, null);
        AddRange(unitOfWork, SharedTestCode, "SYNALB", SexScope.Male, 13m, 120m, 3.5m, 7.5m, null, null);
        AddRange(unitOfWork, SharedTestCode, "SYNALB", SexScope.Female, 13m, 120m, 3.0m, 7.0m, null, null);

        // The range that the freezing test amends.
        AddRange(unitOfWork, AmendableTestCode, "SYNAMD", SexScope.Male, 13m, 120m, 3.5m, 7.5m, null, null);

        // One range with critical limits that sit outside the normal interval,
        // so that a value can be abnormal without being critical and a
        // critical value can override an abnormal classification.
        AddRange(unitOfWork, CriticalTestCode, "SYNCRT", SexScope.All, null, null, 2.5m, 6.5m, 2.0m, 7.0m);

        await unitOfWork.SaveChangesAsync();
    }

    private static void AddRange(
        IUnitOfWork unitOfWork,
        int testCode,
        string simpleName,
        SexScope sexScope,
        decimal? ageFrom,
        decimal? ageTo,
        decimal lowLimit,
        decimal highLimit,
        decimal? lowCriticalLimit,
        decimal? highCriticalLimit)
    {
        var interval = ReferenceInterval.Create(
            lowLimit: lowLimit,
            highLimit: highLimit,
            lowExceptionText: "Below the reference interval",
            highExceptionText: "Above the reference interval",
            rangeText: $"Synthetic range {lowLimit} - {highLimit}",
            unit: "u",
            criticalInterval: lowCriticalLimit.HasValue || highCriticalLimit.HasValue
                ? CriticalInterval.Create(lowCriticalLimit, highCriticalLimit, "Synthetic critical range")
                : null);

        var range = ReferenceRange.Create(
            SyntheticData.NextCatalogueId(),
            testCode,
            simpleName,
            1,
            sexScope,
            AgeBand.Create(ageFrom, ageTo, AgeUnit.Years),
            interval);

        unitOfWork.Add(range);
    }
}
