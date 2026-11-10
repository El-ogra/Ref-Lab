using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Results;

public class EnterAdditionalResultTests
{
    [Fact]
    public async Task The_additional_slot_is_written()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        var orderTest = TestData.CreateOrderTest(order);
        host.Db.Seed(patient, order, orderTest);

        var result = await host.Sender.Send(
            new EnterAdditionalResultCommand(orderTest.Id, 2, "second value"),
            CancellationToken.None);

        Assert.Equal("second value", result.AdditionalResults[2]);
    }

    [Fact]
    public async Task A_slot_outside_the_available_range_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new EnterAdditionalResultCommand(Guid.NewGuid(), 9, "x"), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_requested_test_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new EnterAdditionalResultCommand(Guid.NewGuid(), 0, "x"), CancellationToken.None));
    }
}

public class MethodSheetTests
{
    [Fact]
    public async Task The_method_sheet_carries_the_sample_type_and_the_critical_values()
    {
        using var host = new ApplicationTestHost();
        var definition = TestData.CreateTestDefinition();
        definition.UpdateDetails(sampleType: "EDTA Blood", description: "Haemoglobin");
        definition.SetCriticalInterval(CriticalInterval.Create(lowCriticalLimit: 5m, highCriticalLimit: 20m));
        host.Db.Seed(definition);

        var result = await host.Sender.Send(new GetMethodSheetQuery(56), CancellationToken.None);

        Assert.Equal("EDTA Blood", result.SampleType);
        Assert.Equal("Haemoglobin", result.Description);
        Assert.Equal(5m, result.CriticalValues[0].LowCriticalLimit);
        Assert.Equal(20m, result.CriticalValues[0].HighCriticalLimit);
    }

    [Fact]
    public async Task An_unknown_test_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new GetMethodSheetQuery(999), CancellationToken.None));
    }

    [Fact]
    public async Task The_critical_values_are_returned_on_their_own()
    {
        using var host = new ApplicationTestHost();
        var definition = TestData.CreateTestDefinition();
        definition.SetCriticalInterval(CriticalInterval.Create(highCriticalLimit: 20m, criticalExceptionText: "حرجة"));
        host.Db.Seed(definition);

        var result = await host.Sender.Send(new GetCriticalValuesQuery(56), CancellationToken.None);

        Assert.Equal(20m, result.HighCriticalLimit);
        Assert.Equal("حرجة", result.ExceptionText);
    }
}
