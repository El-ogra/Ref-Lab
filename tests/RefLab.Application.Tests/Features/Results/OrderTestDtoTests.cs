using RefLab.Application.Features.Patients;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Results;

/// <summary>
/// The result sheet shows the flag, the normal range and the critical range in
/// separate columns. All three are read from the reference snapshot frozen
/// onto the order row, never from the live range table.
/// </summary>
public class OrderTestDtoTests
{
    private static OrderTest CreateOrderTest(decimal value, ReferenceInterval? interval)
    {
        var orderTest = OrderTest.Create(Guid.NewGuid(), 56, "Hgb", 11, TestData.Now, "lab.tech");
        orderTest.EnterResult(ResultValue.FromNumber(value), TestData.Now, "lab.tech");

        if (interval is not null)
        {
            orderTest.AttachReferenceSnapshot(ReferenceSnapshot.Capture(
                ReferenceRange.Create(
                    id: 1,
                    testCode: 56,
                    testSimpleName: "Hgb",
                    subGroupCode: 11,
                    sexScope: SexScope.All,
                    ageBand: AgeBand.Create(0m, 120m, AgeUnit.Years),
                    interval: interval),
                TestData.Now));
        }

        return orderTest;
    }

    [Fact]
    public void The_flag_is_high_for_a_high_result()
    {
        var orderTest = CreateOrderTest(
            18.5m,
            ReferenceInterval.Create(lowLimit: 12m, highLimit: 16m));

        var dto = OrderTestDto.FromDomain(orderTest);

        Assert.Equal("High", dto.Classification);
        Assert.Equal("High", dto.LowHighFlag);
    }

    [Fact]
    public void The_flag_is_low_for_a_low_result()
    {
        var orderTest = CreateOrderTest(
            5m,
            ReferenceInterval.Create(lowLimit: 12m, highLimit: 16m));

        var dto = OrderTestDto.FromDomain(orderTest);

        Assert.Equal("Low", dto.Classification);
        Assert.Equal("Low", dto.LowHighFlag);
    }

    [Fact]
    public void The_flag_is_absent_for_a_normal_result()
    {
        var orderTest = CreateOrderTest(
            13.4m,
            ReferenceInterval.Create(lowLimit: 12m, highLimit: 16m));

        var dto = OrderTestDto.FromDomain(orderTest);

        Assert.Equal("Normal", dto.Classification);
        Assert.Null(dto.LowHighFlag);
    }

    [Fact]
    public void The_flag_is_absent_for_a_critical_result()
    {
        var orderTest = CreateOrderTest(
            25m,
            ReferenceInterval.Create(
                lowLimit: 12m,
                highLimit: 16m,
                criticalInterval: CriticalInterval.Create(highCriticalLimit: 20m)));

        var dto = OrderTestDto.FromDomain(orderTest);

        Assert.Equal("Critical", dto.Classification);
        Assert.Null(dto.LowHighFlag);
    }

    [Fact]
    public void The_normal_range_is_read_from_the_frozen_snapshot()
    {
        var orderTest = CreateOrderTest(
            13.4m,
            ReferenceInterval.Create(
                lowLimit: 12m,
                highLimit: 16m,
                rangeText: "Adult 12 - 16 g/dL"));

        var dto = OrderTestDto.FromDomain(orderTest);

        Assert.Equal("Adult 12 - 16 g/dL", dto.NormalRangeText);
    }

    [Fact]
    public void The_critical_range_is_read_from_the_frozen_snapshot()
    {
        var orderTest = CreateOrderTest(
            13.4m,
            ReferenceInterval.Create(
                lowLimit: 12m,
                highLimit: 16m,
                criticalInterval: CriticalInterval.Create(
                    highCriticalLimit: 20m,
                    criticalRangeText: "Above 20 g/dL")));

        var dto = OrderTestDto.FromDomain(orderTest);

        Assert.Equal("Above 20 g/dL", dto.CriticalRangeText);
    }

    [Fact]
    public void Without_a_frozen_snapshot_there_is_no_flag_and_no_range()
    {
        var orderTest = CreateOrderTest(13.4m, interval: null);

        var dto = OrderTestDto.FromDomain(orderTest);

        Assert.Null(dto.Classification);
        Assert.Null(dto.LowHighFlag);
        Assert.Null(dto.NormalRangeText);
        Assert.Null(dto.CriticalRangeText);
    }
}
