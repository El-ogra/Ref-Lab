using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Billing;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the accounting and cash desk screen.
/// </summary>
/// <remarks>
/// The screen settles the account of a patient order, shows the payments and
/// the referring-party commission, and runs the stock-take for a period.
/// </remarks>
public sealed partial class AccountingViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public AccountingViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Accounting";

    /// <summary>The payments.</summary>
    public ObservableCollection<PaymentDto> Payments { get; } = new();

    /// <summary>The referring parties.</summary>
    public ObservableCollection<ReferringPartyDto> ReferringParties { get; } = new();

    /// <summary>The last stock-take.</summary>
    [ObservableProperty]
    private StockTakeDto? _stockTake;

    /// <summary>The last commission.</summary>
    [ObservableProperty]
    private CommissionDto? _commission;

    /// <summary>The order whose account is settled.</summary>
    [ObservableProperty]
    private Guid? _orderId;

    /// <summary>The amount settled.</summary>
    [ObservableProperty]
    private decimal _amount;

    /// <summary>The payment type.</summary>
    [ObservableProperty]
    private int _paymentType;

    /// <summary>The code of the referring party of the payment.</summary>
    [ObservableProperty]
    private string? _referringPartyCode;

    /// <summary>The code of the treating physician of the payment.</summary>
    [ObservableProperty]
    private string? _treatingPhysicianCode;

    /// <summary>The simple name of the test the payment refers to.</summary>
    [ObservableProperty]
    private string? _testSimpleName;

    /// <summary>The comment of the payment.</summary>
    [ObservableProperty]
    private string? _comment;

    /// <summary>The start of the stock-take period.</summary>
    [ObservableProperty]
    private DateTime _stockTakeFrom = DateTime.Today.AddMonths(-1);

    /// <summary>The end of the stock-take period.</summary>
    [ObservableProperty]
    private DateTime _stockTakeTo = DateTime.Today;

    /// <summary>The referring party the stock-take is scoped to.</summary>
    [ObservableProperty]
    private string? _stockTakeReferringPartyCode;

    /// <summary>The treating physician the stock-take is scoped to.</summary>
    [ObservableProperty]
    private string? _stockTakeTreatingPhysicianCode;

    /// <summary>The referring party the commission is computed for.</summary>
    [ObservableProperty]
    private int _commissionReferringPartyId;

    /// <summary>The orders the commission is computed over.</summary>
    public ObservableCollection<Guid> CommissionOrderIds { get; } = new();

    /// <summary>Reloads the payments and the referring parties.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var payments = await _executor.SendAsync(new GetPaymentsQuery());

            Payments.Clear();

            foreach (var payment in payments)
            {
                Payments.Add(payment);
            }

            var parties = await _executor.SendAsync(new GetReferringPartiesQuery());

            ReferringParties.Clear();

            foreach (var party in parties)
            {
                ReferringParties.Add(party);
            }
        }).ConfigureAwait(true);
    }

    /// <summary>Records a payment.</summary>
    [RelayCommand]
    private async Task RecordPaymentAsync()
    {
        if (OrderId is null)
        {
            ErrorMessage = "An order is required.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new RecordPaymentCommand(
                OrderId.Value,
                Amount,
                PaymentType,
                ReferringPartyCode,
                TestSimpleName,
                TreatingPhysicianCode,
                Comment));

            await LoadAsync().ConfigureAwait(true);

            StatusMessage = "The payment was recorded.";
        }).ConfigureAwait(true);
    }

    /// <summary>Computes the commission of a referring party.</summary>
    [RelayCommand]
    private async Task ComputeCommissionAsync()
    {
        if (CommissionOrderIds.Count == 0)
        {
            ErrorMessage = "Select at least one order.";
            return;
        }

        await RunAsync(async () =>
        {
            Commission = await _executor.SendAsync(new ComputeCommissionQuery(
                CommissionReferringPartyId,
                CommissionOrderIds.ToArray()));

            StatusMessage = $"Commission: {Commission.CommissionAmount} {Commission.CurrencyCode}.";
        }).ConfigureAwait(true);
    }

    /// <summary>Runs the stock-take for a period.</summary>
    [RelayCommand]
    private async Task RunStockTakeAsync()
    {
        await RunAsync(async () =>
        {
            var from = new DateTimeOffset(DateOnly.FromDateTime(StockTakeFrom.Date), TimeOnly.MinValue, TimeSpan.Zero);
            var to = new DateTimeOffset(DateOnly.FromDateTime(StockTakeTo.Date), TimeOnly.MaxValue, TimeSpan.Zero);

            StockTake = await _executor.SendAsync(new RunPeriodStockTakeQuery(from, to, LabCurrency.Code));

            StatusMessage = $"{StockTake.OrderCount} order(s) in the period.";
        }).ConfigureAwait(true);
    }

    /// <summary>Runs the stock-take for a physician or a referring party.</summary>
    [RelayCommand]
    private async Task RunScopedStockTakeAsync()
    {
        await RunAsync(async () =>
        {
            var from = new DateTimeOffset(DateOnly.FromDateTime(StockTakeFrom.Date), TimeOnly.MinValue, TimeSpan.Zero);
            var to = new DateTimeOffset(DateOnly.FromDateTime(StockTakeTo.Date), TimeOnly.MaxValue, TimeSpan.Zero);

            StockTake = await _executor.SendAsync(new RunStockTakeByPhysicianOrPartyQuery(
                from,
                to,
                LabCurrency.Code,
                StockTakeReferringPartyCode,
                StockTakeTreatingPhysicianCode));

            StatusMessage = $"{StockTake.OrderCount} order(s) in the scope.";
        }).ConfigureAwait(true);
    }

    /// <summary>Sets the default account type of the laboratory.</summary>
    [RelayCommand]
    private async Task SetDefaultAccountTypeAsync()
    {
        await RunAsync(async () =>
        {
            await _executor.SendAsync(new SetDefaultAccountTypeCommand(Comment));

            StatusMessage = "The default account type was saved.";
        }).ConfigureAwait(true);
    }
}
