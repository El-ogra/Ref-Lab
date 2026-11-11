using RefLab.Application.Features.Results;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Patients;

namespace RefLab.Application.Tests.Features.Results;

/// <summary>
/// The reference system prints a standard comment under the result of a
/// specimen that was collected away from the laboratory. The fact is carried
/// on the requested test, so entering a result is where it is recorded.
/// These tests prove that the flag reaches the order row and that it is
/// never set by an entry which does not carry it.
/// </summary>
public sealed class ResultOutsideLabTests
{
    [Fact]
    public async Task A_result_entered_for_a_specimen_taken_outside_the_laboratory_records_the_flag()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);

        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new EnterResultCommand(orderTest.Id, "13.4", SampleTakenOutsideLab: true),
            CancellationToken.None);

        Assert.True(result.SampleTakenOutsideLab);
        Assert.True(orderTest.SampleTakenOutsideLab);
        Assert.True(host.Db.Items<OrderTest>().Single().SampleTakenOutsideLab);
    }

    [Fact]
    public async Task A_result_entered_without_the_flag_leaves_the_flag_unchanged()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);

        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new EnterResultCommand(orderTest.Id, "13.4"),
            CancellationToken.None);

        Assert.False(result.SampleTakenOutsideLab);
        Assert.False(orderTest.SampleTakenOutsideLab);
        Assert.False(host.Db.Items<OrderTest>().Single().SampleTakenOutsideLab);
    }
}
