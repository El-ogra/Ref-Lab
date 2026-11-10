using RefLab.Application.Common;
using RefLab.Application.Features.Statistics;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Patients;

namespace RefLab.Application.Tests.Features.Statistics;

public class StatisticsTests
{
    [Fact]
    public async Task The_laboratory_statistics_count_the_orders_and_the_requests()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new GetLaboratoryStatisticsQuery(TestData.Now.AddDays(-1), TestData.Now.AddDays(1)),
            CancellationToken.None);

        Assert.Equal(1, result.OrderCount);
        Assert.Equal(1, result.RequestedTestCount);
        Assert.Single(result.ByMonth);
        Assert.Single(result.BySex);
        Assert.Single(result.BySubGroup);
    }

    [Fact]
    public async Task A_period_that_ends_before_it_starts_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new GetLaboratoryStatisticsQuery(TestData.Now.AddDays(1), TestData.Now),
                CancellationToken.None));
    }

    [Fact]
    public async Task The_performance_statistics_count_the_reviewed_and_the_printed_results()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateReviewedOrderTest(order);
        orderTest.MarkPrinted("lab.tech", TestData.Now.AddMinutes(30), 1984);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new GetPerformanceStatisticsQuery(TestData.Now.AddDays(-1), TestData.Now.AddDays(1)),
            CancellationToken.None);

        Assert.Equal(1, result.ReviewedResultCount);
        Assert.Equal(1, result.PrintedResultCount);
        Assert.Equal(0, result.HandedOverResultCount);
        Assert.Equal(30m, result.AverageTurnaroundMinutes);
    }

    [Fact]
    public async Task A_critical_result_is_counted_in_the_performance_statistics()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);

        var snapshot = RefLab.Domain.Patients.ReferenceSnapshot.Capture(
            RefLab.Domain.ClinicalKnowledge.ReferenceRange.Create(
                1,
                56,
                "Hgb",
                11,
                RefLab.Domain.Enums.SexScope.All,
                RefLab.Domain.ValueObjects.AgeBand.Create(0m, 120m, RefLab.Domain.Enums.AgeUnit.Years),
                RefLab.Domain.ValueObjects.ReferenceInterval.Create(
                    lowLimit: 12m,
                    highLimit: 16m,
                    criticalInterval: RefLab.Domain.ValueObjects.CriticalInterval.Create(highCriticalLimit: 20m))),
            TestData.Now);

        orderTest.AttachReferenceSnapshot(snapshot);
        orderTest.EnterResult(Domain.ValueObjects.ResultValue.FromNumber(25m), TestData.Now, "lab.tech");
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new GetPerformanceStatisticsQuery(TestData.Now.AddDays(-1), TestData.Now.AddDays(1)),
            CancellationToken.None);

        Assert.Equal(1, result.CriticalResultCount);
    }
}
