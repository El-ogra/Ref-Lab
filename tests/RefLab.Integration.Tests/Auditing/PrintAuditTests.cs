using RefLab.Application.Features.Patients;
using RefLab.Application.Features.Results;
using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;
using RefLab.Integration.Tests.Integration;

namespace RefLab.Integration.Tests.Auditing;

/// <summary>
/// The print audit trail of the laboratory.
/// </summary>
/// <remarks>
/// <para>
/// The legacy system records <c>resultprint</c>, <c>printuser</c>,
/// <c>printdate</c> and <c>printnum</c> on the test row. Ref Lab keeps the
/// flags on the order row and adds a typed audit record, so that the reports
/// that were issued can be listed and attributed.
/// </para>
/// <para>
/// The sequence number is a confidentiality mechanism as much as an
/// identifier: a missing or a duplicated number reveals a missing report. The
/// tests below therefore pin down that the numbers are consecutive and that
/// they ascend.
/// </para>
/// </remarks>
public sealed class PrintAuditTests : IClassFixture<IntegrationTestHostFixture>
{
    private readonly IntegrationTestHostFixture _fixture;

    /// <summary>Initialises a new test class.</summary>
    /// <param name="fixture">The fixture that stands up the application.</param>
    public PrintAuditTests(IntegrationTestHostFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>The host under test.</summary>
    private IntegrationTestHost Host => _fixture.Host;

    [Fact]
    public async Task The_print_sequence_numbers_are_consecutive_and_ascending()
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

        var ordered = await Host.Mediator.Send(new OrderTestsCommand(
            order.Id,
            new[] { IntegrationTestHostFixture.SharedTestCode }));

        var orderTest = Assert.Single(ordered);

        await Host.Mediator.Send(new EnterResultCommand(orderTest.Id, "5.0", "u"));
        await Host.Mediator.Send(new ReviewResultCommand(orderTest.Id));

        var destination = Path.Combine(Path.GetTempPath(), "RefLab-Phase6", suffix);

        try
        {
            var first = await Host.Mediator.Send(new PrintResultReportCommand(
                orderTest.Id,
                ReportCategory.Individual,
                destination));

            var second = await Host.Mediator.Send(new PrintResultReportCommand(
                orderTest.Id,
                ReportCategory.Contracts,
                destination));

            // The legacy origin of the counter is 1984, and the numbers are
            // consecutive.
            Assert.True(first.SequenceNumber >= 1984);
            Assert.Equal(first.SequenceNumber + 1, second.SequenceNumber);

            // Both reports were produced.
            Assert.True(File.Exists(first.FullPath));
            Assert.True(File.Exists(second.FullPath));
            Assert.NotEqual(first.FileName, second.FileName);

            // Both reports were audited, in the order they were printed.
            var audit = Host.ReadAll<PrintAudit>()
                .Where(entry => entry.OrderTestId == orderTest.Id)
                .OrderBy(entry => entry.SequenceNumber)
                .ToArray();

            Assert.Equal(2, audit.Length);
            Assert.Equal(first.SequenceNumber, audit[0].SequenceNumber);
            Assert.Equal(second.SequenceNumber, audit[1].SequenceNumber);
            Assert.Equal(ReportCategory.Individual, audit[0].Category);
            Assert.Equal(ReportCategory.Contracts, audit[1].Category);
            Assert.Equal(_fixture.OperatorName, audit[0].Operator);
            Assert.Equal(_fixture.OperatorName, audit[1].Operator);

            // The order row carries the last number that was issued.
            var stored = Host.ReadOne<Domain.Patients.OrderTest>(candidate => candidate.Id == orderTest.Id);

            Assert.NotNull(stored);
            Assert.True(stored!.IsResultPrinted);
            Assert.Equal(second.SequenceNumber, stored.PrintNumber);
            Assert.Equal(_fixture.OperatorName, stored.PrintedBy);
        }
        finally
        {
            TryDeleteDirectory(destination);
        }
    }

    [Fact]
    public async Task A_print_audit_record_names_the_operator_the_moment_and_the_category()
    {
        Host.EstablishSession(_fixture.OperatorId);

        var suffix = SyntheticData.Unique();

        var order = await Host.Mediator.Send(new RegisterPatientCommand(
            SyntheticData.PatientName(suffix),
            Sex.Female,
            26,
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

        var ordered = await Host.Mediator.Send(new OrderTestsCommand(
            order.Id,
            new[] { IntegrationTestHostFixture.SharedTestCode }));

        var orderTest = Assert.Single(ordered);

        await Host.Mediator.Send(new EnterResultCommand(orderTest.Id, "4.0", "u"));
        await Host.Mediator.Send(new ReviewResultCommand(orderTest.Id));

        var destination = Path.Combine(Path.GetTempPath(), "RefLab-Phase6", suffix);

        try
        {
            var printed = await Host.Mediator.Send(new PrintResultReportCommand(
                orderTest.Id,
                ReportCategory.Individual,
                destination));

            var audit = Assert.Single(
                Host.ReadAll<PrintAudit>(),
                entry => entry.OrderTestId == orderTest.Id);

            Assert.Equal(printed.SequenceNumber, audit.SequenceNumber);
            Assert.Equal(_fixture.OperatorName, audit.Operator);
            Assert.Equal(ReportCategory.Individual, audit.Category);
            Assert.NotEqual(default, audit.PrintedAt);
        }
        finally
        {
            TryDeleteDirectory(destination);
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (IOException)
        {
            // The temporary directory is left behind; it carries no real data.
        }
        catch (UnauthorizedAccessException)
        {
            // The temporary directory is left behind; it carries no real data.
        }
    }
}
