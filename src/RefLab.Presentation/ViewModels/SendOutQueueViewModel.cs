using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.SendOut;
using RefLab.Domain.Enums;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the send-out and sample disposition screen.
/// </summary>
/// <remarks>
/// The screen is reached with the F7 shortcut, in the way the reference
/// product reached the send-out screen from the accounting menu.
/// </remarks>
public sealed partial class SendOutQueueViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public SendOutQueueViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Send-out queue";

    /// <summary>The send-out requests.</summary>
    public ObservableCollection<SendOutRequestDto> Requests { get; } = new();

    /// <summary>The sample dispositions.</summary>
    public ObservableCollection<SampleDispositionDto> Dispositions { get; } = new();

    /// <summary>The selected request.</summary>
    [ObservableProperty]
    private SendOutRequestDto? _selectedRequest;

    /// <summary>Whether the queue lists the pending requests only.</summary>
    [ObservableProperty]
    private bool _pendingOnly = true;

    /// <summary>The destination laboratory of a new request.</summary>
    [ObservableProperty]
    private string _destinationLabName = string.Empty;

    /// <summary>The price charged to the destination laboratory.</summary>
    [ObservableProperty]
    private decimal? _outPrice;

    /// <summary>The price before the discount.</summary>
    [ObservableProperty]
    private decimal? _outPriceBeforeDiscount;

    /// <summary>The test the sample belongs to.</summary>
    [ObservableProperty]
    private Guid? _orderTestId;

    /// <summary>The disposition to record.</summary>
    [ObservableProperty]
    private SampleDispositionAction _dispositionAction = SampleDispositionAction.Split;

    /// <summary>Reloads the queue.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var requests = await _executor.SendAsync(new GetSendOutQueueQuery(PendingOnly));

            Requests.Clear();

            foreach (var request in requests)
            {
                Requests.Add(request);
            }

            StatusMessage = $"{requests.Count} send-out request(s).";
        }).ConfigureAwait(true);
    }

    /// <summary>Creates a send-out request.</summary>
    [RelayCommand]
    private async Task CreateAsync()
    {
        if (OrderTestId is null)
        {
            ErrorMessage = "A test is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(DestinationLabName))
        {
            ErrorMessage = "A destination laboratory is required.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new CreateSendOutRequestCommand(
                OrderTestId.Value,
                DestinationLabName,
                OutPrice,
                OutPriceBeforeDiscount));

            await LoadAsync().ConfigureAwait(true);

            StatusMessage = "The send-out request was created.";
        }).ConfigureAwait(true);
    }

    /// <summary>Dispatches the selected sample.</summary>
    [RelayCommand]
    private async Task DispatchAsync()
    {
        if (SelectedRequest is null)
        {
            ErrorMessage = "Select a request first.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new DispatchSendOutSampleCommand(SelectedRequest.Id));

            await LoadAsync().ConfigureAwait(true);

            StatusMessage = "The sample was dispatched.";
        }).ConfigureAwait(true);
    }

    /// <summary>Settles the selected request.</summary>
    [RelayCommand]
    private async Task SettleAsync()
    {
        if (SelectedRequest is null)
        {
            ErrorMessage = "Select a request first.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new SettleSendOutSampleCommand(SelectedRequest.Id));

            await LoadAsync().ConfigureAwait(true);

            StatusMessage = "The request was settled.";
        }).ConfigureAwait(true);
    }

    /// <summary>Records a split or a withdrawal.</summary>
    [RelayCommand]
    private async Task RecordDispositionAsync()
    {
        if (OrderTestId is null)
        {
            ErrorMessage = "A test is required.";
            return;
        }

        await RunAsync(async () =>
        {
            var disposition = await _executor.SendAsync(new SplitOrWithdrawSampleCommand(
                OrderTestId.Value,
                DispositionAction,
                string.IsNullOrWhiteSpace(DestinationLabName) ? null : DestinationLabName));

            Dispositions.Add(disposition);

            StatusMessage = "The disposition was recorded.";
        }).ConfigureAwait(true);
    }
}
