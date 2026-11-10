using RefLab.Application.Common;
using RefLab.Application.Features.Auditing;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;

namespace RefLab.Application.Tests.Features.Auditing;

public class RecordAttendanceHandlerTests
{
    [Fact]
    public async Task A_work_start_is_recorded_on_the_operator_and_in_the_log()
    {
        using var host = new ApplicationTestHost();
        var @operator = TestData.CreateOperator();
        host.Db.Seed(@operator);

        var entry = await host.Sender.Send(
            new RecordAttendanceCommand(AttendanceEventType.StartedWork),
            CancellationToken.None);

        Assert.Equal(AttendanceEventType.StartedWork, entry.EventType);
        Assert.NotNull(@operator.AttendanceStart);
        Assert.Single(host.Db.Items<AttendanceEntry>());
    }

    [Fact]
    public async Task An_unsigned_operator_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.User.OperatorId = null;

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(new RecordAttendanceCommand(AttendanceEventType.StartedWork), CancellationToken.None));
    }
}

public class GetPrintAuditTrailHandlerTests
{
    [Fact]
    public async Task The_audit_trail_is_returned_newest_first()
    {
        using var host = new ApplicationTestHost();
        var orderTestId = Guid.NewGuid();

        host.Db.Seed(
            PrintAudit.Create(orderTestId, "lab.tech", new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero), 1984),
            PrintAudit.Create(orderTestId, "lab.tech", new DateTimeOffset(2026, 3, 2, 9, 0, 0, TimeSpan.Zero), 1985));

        var result = await host.Sender.Send(
            new GetPrintAuditTrailQuery(orderTestId, PageRequest.First()),
            CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1985, result.Items[0].SequenceNumber);
    }

    [Fact]
    public async Task The_trail_is_filtered_by_requested_test()
    {
        using var host = new ApplicationTestHost();
        var wanted = Guid.NewGuid();

        host.Db.Seed(
            PrintAudit.Create(wanted, "lab.tech", DateTimeOffset.UtcNow, 1984),
            PrintAudit.Create(Guid.NewGuid(), "lab.tech", DateTimeOffset.UtcNow, 1985));

        var result = await host.Sender.Send(
            new GetPrintAuditTrailQuery(wanted, PageRequest.First()),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(wanted, result.Items[0].OrderTestId);
        Assert.Equal(1984, result.Items[0].SequenceNumber);
    }
}
