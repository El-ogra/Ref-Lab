using RefLab.Application.Common;
using RefLab.Application.Features.Identity;
using RefLab.Application.Features.Patients;
using RefLab.Application.Features.Results;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Integration.Tests.Integration;

namespace RefLab.Integration.Tests.Workflow;

/// <summary>
/// The mandated clinical workflow, end to end, through the real application.
/// </summary>
/// <remarks>
/// <para>
/// The workflow is
/// <c>SignIn → RegisterPatient → OrderTests → EnterResult → ReviewResult →
/// PrintResultReport</c>. Every step goes through the real MediatR pipeline,
/// the real handlers, the real domain rules and the real persistence layer.
/// Nothing is stubbed, and no use case is bypassed.
/// </para>
/// <para>
/// The distinction the phase directive draws is respected throughout: this
/// test proves that the application can <em>produce</em> a laboratory report.
/// It does not prove that a Windows printer has printed anything, and it says
/// so.
/// </para>
/// </remarks>
public sealed class ClinicalWorkflowEndToEndTests : IClassFixture<IntegrationTestHostFixture>
{
    private readonly IntegrationTestHostFixture _fixture;

    /// <summary>Initialises a new test class.</summary>
    /// <param name="fixture">The fixture that stands up the application.</param>
    public ClinicalWorkflowEndToEndTests(IntegrationTestHostFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>The host under test.</summary>
    private IntegrationTestHost Host => _fixture.Host;

    [Fact]
    public async Task The_clinical_workflow_runs_from_sign_in_to_the_printed_report()
    {
        var suffix = SyntheticData.Unique();
        var patientName = SyntheticData.PatientName(suffix);
        var destination = Path.Combine(Path.GetTempPath(), "RefLab-Phase6", suffix);

        try
        {
            // ---------------------------------------------------------
            // 1. Sign in
            // ---------------------------------------------------------
            var signIn = await Host.Mediator.Send(
                new SignInCommand(_fixture.OperatorName, SyntheticData.OperatorPassword));

            Assert.Equal(_fixture.OperatorName, signIn.Operator.Name);
            Assert.True(signIn.Operator.IsActive);
            Assert.Contains("ReviewResults", signIn.Operator.Permissions);

            Host.EstablishSession(_fixture.OperatorId);

            // ---------------------------------------------------------
            // 2. Register a synthetic patient
            // ---------------------------------------------------------
            var order = await Host.Mediator.Send(new RegisterPatientCommand(
                patientName,
                Sex.Male,
                30,
                AgeUnit.Years,
                null,
                "1 Synthetic Street",
                null,
                null,
                null,
                null,
                null,
                null,
                "EGP",
                CreateOrder: true));

            Assert.Equal(patientName, ReadPatientNameAsync(order.PatientId));
            Assert.Equal("EGP", order.CurrencyCode);

            // ---------------------------------------------------------
            // 3. Verify that the patient and the order were persisted
            // ---------------------------------------------------------
            var storedOrder = Host.ReadOne<PatientOrder>(candidate => candidate.Id == order.Id);

            Assert.NotNull(storedOrder);
            Assert.Equal(order.PatientId, storedOrder!.PatientId.Value);

            // ---------------------------------------------------------
            // 4. Order a test, and freeze the applicable reference range
            // ---------------------------------------------------------
            var ordered = await Host.Mediator.Send(new OrderTestsCommand(
                order.Id,
                new[] { IntegrationTestHostFixture.SharedTestCode }));

            var orderTest = Assert.Single(ordered);
            Assert.Equal(IntegrationTestHostFixture.SharedTestCode, orderTest.TestCode);

            var storedTest = Host.ReadOne<OrderTest>(candidate => candidate.Id == orderTest.Id);

            Assert.NotNull(storedTest);
            Assert.Equal(order.Id, storedTest!.OrderId);
            Assert.NotNull(storedTest.ReferenceSnapshot);
            Assert.Equal(3.5m, storedTest.ReferenceSnapshot!.Interval.LowLimit);
            Assert.Equal(7.5m, storedTest.ReferenceSnapshot.Interval.HighLimit);

            // ---------------------------------------------------------
            // 5. Enter a valid synthetic result
            // ---------------------------------------------------------
            var entered = await Host.Mediator.Send(new EnterResultCommand(
                orderTest.Id,
                "5.0",
                "u"));

            Assert.Equal("5.0", entered.ResultText);
            Assert.Equal(5.0m, entered.ResultValue);
            Assert.Equal("Normal", entered.Classification);
            Assert.Equal(ResultState.Entered, entered.State);

            var storedResult = Host.ReadOne<OrderTest>(candidate => candidate.Id == orderTest.Id);

            Assert.NotNull(storedResult);
            Assert.True(storedResult!.IsResultFinished);
            Assert.False(storedResult.IsResultReviewed);

            // ---------------------------------------------------------
            // 6. Review the result
            // ---------------------------------------------------------
            var reviewed = await Host.Mediator.Send(new ReviewResultCommand(orderTest.Id));

            Assert.True(reviewed.IsReviewed);
            Assert.Equal(ResultState.Reviewed, reviewed.State);

            var storedReview = Host.ReadOne<OrderTest>(candidate => candidate.Id == orderTest.Id);

            Assert.NotNull(storedReview);
            Assert.True(storedReview!.IsResultReviewed);
            Assert.Equal(_fixture.OperatorName, storedReview.EditedBy);

            // ---------------------------------------------------------
            // 7. Produce the laboratory report
            // ---------------------------------------------------------
            var printed = await Host.Mediator.Send(new PrintResultReportCommand(
                orderTest.Id,
                ReportCategory.Individual,
                destination));

            Assert.True(File.Exists(printed.FullPath));
            Assert.Equal(ReportCategory.Individual.ToString(), printed.Category);
            Assert.True(printed.SequenceNumber >= 1984);

            // ---------------------------------------------------------
            // 8. The report carries the patient and the result
            // ---------------------------------------------------------
            var text = PdfTextReader.ExtractText(printed.FullPath);

            Assert.Contains("LABORATORY REPORT", text, StringComparison.Ordinal);
            Assert.Contains(patientName, text, StringComparison.Ordinal);
            Assert.Contains("5.0", text, StringComparison.Ordinal);
            Assert.Contains("EGP", text, StringComparison.Ordinal);

            // ---------------------------------------------------------
            // 9. The print audit records the report
            // ---------------------------------------------------------
            var storedPrinted = Host.ReadOne<OrderTest>(candidate => candidate.Id == orderTest.Id);

            Assert.NotNull(storedPrinted);
            Assert.True(storedPrinted!.IsResultPrinted);
            Assert.Equal(printed.SequenceNumber, storedPrinted.PrintNumber);
            Assert.Equal(_fixture.OperatorName, storedPrinted.PrintedBy);

            var audit = Assert.Single(
                Host.ReadAll<Domain.Auditing.PrintAudit>(),
                entry => entry.OrderTestId == orderTest.Id);

            Assert.Equal(printed.SequenceNumber, audit.SequenceNumber);
            Assert.Equal(_fixture.OperatorName, audit.Operator);
            Assert.Equal(ReportCategory.Individual, audit.Category);
        }
        finally
        {
            TryDeleteDirectory(destination);
        }
    }

    [Fact]
    public async Task An_unknown_operator_is_refused_by_the_sign_in_use_case()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => Host.Mediator.Send(new SignInCommand(
                $"p6.absent.{SyntheticData.Unique()}",
                SyntheticData.OperatorPassword)));

        Assert.Equal("Operator", exception.EntityName);
    }

    [Fact]
    public async Task A_wrong_password_is_refused_by_the_sign_in_use_case()
    {
        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => Host.Mediator.Send(new SignInCommand(
                _fixture.OperatorName,
                "definitely-not-the-password")));

        Assert.Contains("incorrect", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task An_inactive_operator_is_refused_by_the_sign_in_use_case()
    {
        var name = $"p6.inactive.{SyntheticData.Unique()}";
        var id = SyntheticData.NextOperatorId();

        var @operator = Domain.Identity.Operator.Create(
            id,
            name,
            new Infrastructure.Security.Pbkdf2PasswordHasher().Hash(SyntheticData.OperatorPassword));

        @operator.SetActive(false);

        var unitOfWork = (Application.Abstractions.IUnitOfWork)Host.Context;

        unitOfWork.Add(@operator);

        await unitOfWork.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => Host.Mediator.Send(new SignInCommand(name, SyntheticData.OperatorPassword)));

        Assert.Contains("not active", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task A_result_cannot_be_printed_before_it_has_been_reviewed()
    {
        var suffix = SyntheticData.Unique();

        Host.EstablishSession(_fixture.OperatorId);

        var order = await Host.Mediator.Send(new RegisterPatientCommand(
            SyntheticData.PatientName(suffix),
            Sex.Female,
            44,
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

        var destination = Path.Combine(Path.GetTempPath(), "RefLab-Phase6", suffix);

        try
        {
            // The domain raises ResultNotReviewedException and the
            // DomainExceptionBehavior translates it into the conflict that
            // the presentation layer knows how to show.
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => Host.Mediator.Send(new PrintResultReportCommand(
                    orderTest.Id,
                    ReportCategory.Individual,
                    destination)));

            Assert.Contains("has not been confirmed as reviewed", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            TryDeleteDirectory(destination);
        }
    }

    private string? ReadPatientNameAsync(Guid patientId)
    {
        var patient = Host.ReadOne<Patient>(candidate => candidate.Id.Value == patientId);

        return patient?.Name;
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
