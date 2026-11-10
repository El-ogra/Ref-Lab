using RefLab.Application.Common;
using RefLab.Application.Features.Worksheets;
using RefLab.Application.Tests.Fakes;

namespace RefLab.Application.Tests.Features.Worksheets;

public class WorksheetTests
{
    [Fact]
    public async Task The_patient_group_work_sheet_lists_every_case_in_the_period()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new BuildPatientGroupWorkSheetCommand(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                Array.Empty<Guid>(),
                @"D:\out"),
            CancellationToken.None);

        Assert.Equal(1, result.RowCount);
        Assert.Equal("محمد أحمد", result.Rows[0].Label);
        Assert.Equal(1, host.Pdf.CallCount);
    }

    [Fact]
    public async Task The_work_sheet_is_narrowed_to_the_selected_patients()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new BuildPatientGroupWorkSheetCommand(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                new[] { Guid.NewGuid() },
                @"D:\out"),
            CancellationToken.None);

        Assert.Equal(0, result.RowCount);
    }

    [Fact]
    public async Task The_test_group_work_sheet_lists_the_results_of_a_test()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateReviewedOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new BuildTestGroupWorkSheetCommand(
                TestData.Now.AddDays(-1),
                TestData.Now.AddDays(1),
                @"D:\out",
                TestCode: 56),
            CancellationToken.None);

        Assert.Equal(1, result.RowCount);
        Assert.Contains("13.4", result.Rows[0].Cells[2], StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_work_sheet_without_a_test_or_a_group_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new BuildTestGroupWorkSheetCommand(
                    TestData.Now.AddDays(-1),
                    TestData.Now.AddDays(1),
                    @"D:\out"),
                CancellationToken.None));
    }
}
