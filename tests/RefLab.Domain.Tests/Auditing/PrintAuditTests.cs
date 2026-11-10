using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;

namespace RefLab.Domain.Tests.Auditing;

/// <summary>
/// Every print is recorded with its operator, its moment and its sequence
/// number. A missing or duplicated number reveals a missing report.
/// </summary>
public class PrintAuditTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_stores_the_audit_fields()
    {
        var orderTestId = Guid.NewGuid();

        var audit = PrintAudit.Create(orderTestId, "lab.tech", Moment, 1984, ReportCategory.Individual);

        Assert.Equal(orderTestId, audit.OrderTestId);
        Assert.Equal("lab.tech", audit.Operator);
        Assert.Equal(1984, audit.SequenceNumber);
        Assert.Equal(ReportCategory.Individual, audit.Category);
    }

    [Fact]
    public void Create_rejects_a_record_that_references_no_order_test()
    {
        Assert.Throws<Exceptions.InvalidPatientOrderException>(() =>
            PrintAudit.Create(Guid.Empty, "lab.tech", Moment, 1984));
    }

    [Fact]
    public void Create_rejects_a_record_without_an_operator()
    {
        Assert.Throws<Exceptions.InvalidPatientOrderException>(() =>
            PrintAudit.Create(Guid.NewGuid(), "  ", Moment, 1984));
    }

    [Fact]
    public void The_report_category_carries_the_four_legacy_output_directories()
    {
        var categories = Enum.GetValues<ReportCategory>()
            .Select(category => category.ToString())
            .ToArray();

        Assert.Equal(new[] { "Contracts", "Free", "Individual", "LabToLab" }, categories);
    }
}
