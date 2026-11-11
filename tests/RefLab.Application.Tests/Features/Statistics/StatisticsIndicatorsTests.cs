using RefLab.Application.Features.Statistics;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Enums;

namespace RefLab.Application.Tests.Features.Statistics;

/// <summary>
/// The vendor guide documents the statistics module of the reference system:
/// the patient count by year sorted by month and by sex, the request rate of
/// a test group and the sample count for the year. These tests prove that the
/// laboratory statistics carry those indicators, and that the statistics of
/// a single clinical sub-group can be asked for on their own.
/// </summary>
public sealed class StatisticsIndicatorsTests
{
    private static readonly DateTimeOffset From = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset To = new(2025, 12, 31, 23, 59, 59, TimeSpan.Zero);

    private static DateTimeOffset In2024(int month, int day) =>
        new(2024, month, day, 9, 0, 0, TimeSpan.Zero);

    private static DateTimeOffset In2025(int month, int day) =>
        new(2025, month, day, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task The_statistics_carry_the_patient_count_of_every_year()
    {
        using var host = new ApplicationTestHost();

        var first = TestData.CreatePatient(name: "Ahmed");
        var second = TestData.CreatePatient(name: "Mona");
        var third = TestData.CreatePatient(name: "Sara");

        var orderOf2024 = TestData.CreateOrder(first, requestDate: In2024(5, 10));
        var firstOrderOf2025 = TestData.CreateOrder(second, requestDate: In2025(3, 4));
        var secondOrderOf2025 = TestData.CreateOrder(third, requestDate: In2025(3, 9));

        host.Db.Seed(first, second, third, orderOf2024, firstOrderOf2025, secondOrderOf2025);

        var result = await host.Sender.Send(
            new GetLaboratoryStatisticsQuery(From, To),
            CancellationToken.None);

        Assert.Contains(new PatientCountDto("2024", 1), result.ByYear);
        Assert.Contains(new PatientCountDto("2025", 2), result.ByYear);
    }

    [Fact]
    public async Task The_statistics_carry_the_patient_count_of_every_month_and_sex()
    {
        using var host = new ApplicationTestHost();

        var male = TestData.CreatePatient(name: "Ahmed", sex: Sex.Male);
        var female = TestData.CreatePatient(name: "Mona", sex: Sex.Female);

        var orderOfTheMale = TestData.CreateOrder(male, requestDate: In2025(3, 4));
        var orderOfTheFemale = TestData.CreateOrder(female, requestDate: In2025(3, 9));

        host.Db.Seed(male, female, orderOfTheMale, orderOfTheFemale);

        var result = await host.Sender.Send(
            new GetLaboratoryStatisticsQuery(From, To),
            CancellationToken.None);

        Assert.Contains(new PatientCountDto("2025-03 Male", 1), result.ByMonthAndSex);
        Assert.Contains(new PatientCountDto("2025-03 Female", 1), result.ByMonthAndSex);
    }

    [Fact]
    public async Task The_statistics_carry_the_request_rate_of_every_clinical_sub_group()
    {
        using var host = new ApplicationTestHost();

        var first = TestData.CreatePatient(name: "Ahmed");
        var second = TestData.CreatePatient(name: "Mona");

        var orderOf2024 = TestData.CreateOrder(first, requestDate: In2024(5, 10));
        var orderOf2025 = TestData.CreateOrder(second, requestDate: In2025(3, 4));

        var haemoglobin = TestData.CreateOrderTest(orderOf2024, testCode: 56, simpleName: "Hgb", subGroupCode: 11);
        var whiteCells = TestData.CreateOrderTest(orderOf2024, testCode: 57, simpleName: "WBC", subGroupCode: 11);
        var glucose = TestData.CreateOrderTest(orderOf2025, testCode: 60, simpleName: "Glucose", subGroupCode: 11);
        var urea = TestData.CreateOrderTest(orderOf2025, testCode: 61, simpleName: "Urea", subGroupCode: 22);

        host.Db.Seed(first, second, orderOf2024, orderOf2025, haemoglobin, whiteCells, glucose, urea);

        var result = await host.Sender.Send(
            new GetLaboratoryStatisticsQuery(From, To),
            CancellationToken.None);

        // Three requests of the blood-count group were made by two patients,
        // so the group receives one and a half requests per patient.
        Assert.Contains(new TestGroupRateDto("11", 3, 2, 1.5m), result.RequestRates);
        Assert.Contains(new TestGroupRateDto("22", 1, 1, 1m), result.RequestRates);
    }

    [Fact]
    public async Task The_statistics_carry_the_sample_count_of_every_year()
    {
        using var host = new ApplicationTestHost();

        var first = TestData.CreatePatient(name: "Ahmed");
        var second = TestData.CreatePatient(name: "Mona");

        var orderOf2024 = TestData.CreateOrder(first, requestDate: In2024(5, 10));
        var orderOf2025 = TestData.CreateOrder(second, requestDate: In2025(3, 4));

        var haemoglobin = TestData.CreateOrderTest(orderOf2024, testCode: 56, simpleName: "Hgb", subGroupCode: 11);
        var whiteCells = TestData.CreateOrderTest(orderOf2024, testCode: 57, simpleName: "WBC", subGroupCode: 11);
        var glucose = TestData.CreateOrderTest(orderOf2025, testCode: 60, simpleName: "Glucose", subGroupCode: 11);

        host.Db.Seed(first, second, orderOf2024, orderOf2025, haemoglobin, whiteCells, glucose);

        var result = await host.Sender.Send(
            new GetLaboratoryStatisticsQuery(From, To),
            CancellationToken.None);

        Assert.Contains(new SampleCountDto("2024", 2), result.SamplesByYear);
        Assert.Contains(new SampleCountDto("2025", 1), result.SamplesByYear);
    }

    [Fact]
    public async Task A_clinical_sub_group_restricts_the_request_rates_to_that_sub_group()
    {
        using var host = new ApplicationTestHost();

        var first = TestData.CreatePatient(name: "Ahmed");
        var second = TestData.CreatePatient(name: "Mona");

        var orderOf2024 = TestData.CreateOrder(first, requestDate: In2024(5, 10));
        var orderOf2025 = TestData.CreateOrder(second, requestDate: In2025(3, 4));

        var haemoglobin = TestData.CreateOrderTest(orderOf2024, testCode: 56, simpleName: "Hgb", subGroupCode: 11);
        var whiteCells = TestData.CreateOrderTest(orderOf2024, testCode: 57, simpleName: "WBC", subGroupCode: 11);
        var glucose = TestData.CreateOrderTest(orderOf2025, testCode: 60, simpleName: "Glucose", subGroupCode: 11);
        var urea = TestData.CreateOrderTest(orderOf2025, testCode: 61, simpleName: "Urea", subGroupCode: 22);

        host.Db.Seed(first, second, orderOf2024, orderOf2025, haemoglobin, whiteCells, glucose, urea);

        var result = await host.Sender.Send(
            new GetLaboratoryStatisticsQuery(From, To, SubGroupCode: 11),
            CancellationToken.None);

        var rate = Assert.Single(result.RequestRates);

        Assert.Equal("11", rate.SubGroupCode);
        Assert.Equal(3, rate.RequestCount);
        Assert.Equal(2, rate.PatientCount);
        Assert.Equal(1.5m, rate.RequestsPerPatient);
    }
}
