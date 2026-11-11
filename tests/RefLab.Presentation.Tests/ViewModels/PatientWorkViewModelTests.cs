using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Application.Common;
using RefLab.Application.Features.Results;
using RefLab.Application.Features.Patients;
using RefLab.Domain.Enums;
using RefLab.Presentation.Common;
using RefLab.Presentation.Tests.Fakes;
using RefLab.Presentation.ViewModels;
using RefLab.Presentation.ViewModels.Panels;

namespace RefLab.Presentation.Tests.ViewModels;

/// <summary>
/// The patient work screen — the screen that carries the F8, F9, F11 and F12
/// shortcuts.
/// </summary>
/// <remarks>
/// <para>
/// The keyboard shortcuts of the legacy system are bound to the commands of
/// this screen: <c>F8</c> reviews, <c>F9</c> saves, <c>F11</c> prepares the
/// print and <c>F12</c> produces the report. The bindings themselves are
/// declared in <c>PatientWorkView.xaml</c> and can only be exercised on
/// Windows; the behaviour of the commands behind them is exercised here.
/// </para>
/// <para>
/// The tests cover the guards: the commands refuse to act when the screen is
/// not in a state to act, and they report why.
/// </para>
/// </remarks>
public sealed class PatientWorkViewModelTests
{
    [Fact]
    public void The_prepare_print_command_refuses_when_no_test_is_selected()
    {
        using var host = new PresentationTestHost(StubMediator.Immediate(_ => null));

        var viewModel = CreateViewModel(host);

        viewModel.PreparePrintCommand.Execute(null);

        Assert.Equal("Select a test first.", viewModel.ErrorMessage);
        Assert.Null(viewModel.StatusMessage);
    }

    [Fact]
    public void The_prepare_print_command_refuses_when_no_destination_is_chosen()
    {
        using var host = new PresentationTestHost(StubMediator.Immediate(_ => null));

        var viewModel = CreateViewModel(host);

        viewModel.SelectedTest = CreateOrderTest();
        viewModel.DestinationDirectory = "   ";

        viewModel.PreparePrintCommand.Execute(null);

        Assert.Equal("Choose a destination directory first.", viewModel.ErrorMessage);
    }

    [Fact]
    public void The_prepare_print_command_reports_the_chosen_destination()
    {
        using var host = new PresentationTestHost(StubMediator.Immediate(_ => null));

        var viewModel = CreateViewModel(host);

        viewModel.SelectedTest = CreateOrderTest();
        viewModel.DestinationDirectory = @"C:\Synthetic\Reports";

        viewModel.PreparePrintCommand.Execute(null);

        Assert.Null(viewModel.ErrorMessage);
        Assert.Contains(@"C:\Synthetic\Reports", viewModel.StatusMessage!, StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_print_command_refuses_when_no_test_is_selected()
    {
        var mediator = StubMediator.Immediate(_ => null);

        using var host = new PresentationTestHost(mediator);

        var viewModel = CreateViewModel(host);

        await viewModel.PrintCommand.ExecuteAsync(null);

        Assert.Equal("Select a test first.", viewModel.ErrorMessage);
        Assert.Empty(mediator.Sent);
    }

    [Fact]
    public async Task The_print_command_refuses_when_no_destination_is_chosen()
    {
        var mediator = StubMediator.Immediate(_ => null);

        using var host = new PresentationTestHost(mediator);

        var viewModel = CreateViewModel(host);

        viewModel.SelectedTest = CreateOrderTest();
        viewModel.DestinationDirectory = string.Empty;

        await viewModel.PrintCommand.ExecuteAsync(null);

        Assert.Equal("Choose a destination directory first.", viewModel.ErrorMessage);
        Assert.Empty(mediator.Sent);
    }

    [Fact]
    public async Task The_print_command_sends_the_print_request_and_reports_the_written_file()
    {
        var produced = new PrintResultReportDto(
            Guid.Empty,
            1984,
            "LABORATORY_REPORT_20261110_120000.pdf",
            @"C:\Synthetic\Reports\LABORATORY_REPORT_20261110_120000.pdf",
            ReportCategory.Contracts.ToString());

        var mediator = StubMediator.Immediate(_ => produced);

        using var host = new PresentationTestHost(mediator);

        var viewModel = CreateViewModel(host);

        viewModel.SelectedTest = CreateOrderTest();
        viewModel.DestinationDirectory = @"C:\Synthetic\Reports";
        viewModel.ReportCategory = ReportCategory.Contracts;

        await viewModel.PrintCommand.ExecuteAsync(null);

        var request = Assert.IsType<Application.Features.Results.PrintResultReportCommand>(
            Assert.Single(mediator.Sent, item => item is Application.Features.Results.PrintResultReportCommand));

        Assert.Equal(viewModel.SelectedTest!.Id, request.OrderTestId);
        Assert.Equal(ReportCategory.Contracts, request.Category);
        Assert.Equal(@"C:\Synthetic\Reports", request.DestinationDirectory);

        // The command reports what it wrote and raises no error.
        Assert.Null(viewModel.ErrorMessage);
        Assert.Equal(produced.FullPath, viewModel.LastPrintedPath);
        Assert.Contains(produced.FullPath, viewModel.StatusMessage!, StringComparison.Ordinal);
    }

    private static PatientWorkViewModel CreateViewModel(PresentationTestHost host)
    {
        var executor = host.Services.GetRequiredService<RequestExecutor>();

        return new PatientWorkViewModel(
            executor,
            new RecordingErrorPresenter(),
            new MethodSheetPanelViewModel(executor),
            new DerivationPanelViewModel(executor));
    }

    private static OrderTestDto CreateOrderTest() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            5001,
            "SYNALB",
            "SYNTHETIC ALBUMIN",
            "u",
            1,
            null,
            0,
            DateTimeOffset.UtcNow,
            "5.0",
            5.0m,
            new string?[4],
            "Normal",
            null,
            LowHighFlag: null,
            NormalRangeText: "Adult 3.5 - 5.2 g/dL",
            CriticalRangeText: null,
            ResultState.Reviewed,
            IsReviewed: true,
            IsPrinted: false,
            IsGiven: false,
            PrintNumber: null,
            PrintedBy: null,
            PrintedAt: null,
            Comment: null,
            Price: 150m);
}
