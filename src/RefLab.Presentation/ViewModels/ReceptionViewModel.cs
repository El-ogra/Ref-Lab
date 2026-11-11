using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Common;
using RefLab.Application.Features.Auditing;
using RefLab.Application.Features.Patients;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the reception desk.
/// </summary>
/// <remarks>
/// <para>
/// The reception desk is the working surface of the clerk who receives the
/// patients: the day's cases on one side, the tests of the selected case on the
/// other, and the record of what has already been printed.
/// </para>
/// <para>
/// The desk is a workflow over the use cases the laboratory already has — the
/// follow-up register, the order of the patient, the print audit trail and the
/// review action. It introduces no business logic of its own.
/// </para>
/// </remarks>
public sealed partial class ReceptionViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public ReceptionViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Reception";

    /// <summary>The cases of the period.</summary>
    public ObservableCollection<CaseFollowUpDto> Cases { get; } = new();

    /// <summary>The print audit trail of the selected case.</summary>
    public ObservableCollection<PrintAuditDto> AuditTrail { get; } = new();

    /// <summary>The case the clerk is working on.</summary>
    [ObservableProperty]
    private CaseFollowUpDto? _selectedCase;

    /// <summary>The start of the period.</summary>
    [ObservableProperty]
    private DateTime? _from;

    /// <summary>The end of the period.</summary>
    [ObservableProperty]
    private DateTime? _to;

    /// <summary>Whether only the cases that are not yet complete are listed.</summary>
    [ObservableProperty]
    private bool _openOnly = true;

    /// <summary>
    /// Loads the cases of the period and the audit trail of the selected one.
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var cases = await _executor.SendAsync(new FollowUpCaseQuery(
                From.HasValue ? new DateTimeOffset(DateOnly.FromDateTime(From.Value), TimeOnly.MinValue, TimeSpan.Zero) : null,
                To.HasValue ? new DateTimeOffset(DateOnly.FromDateTime(To.Value), TimeOnly.MaxValue, TimeSpan.Zero) : null,
                OpenOnly));

            Cases.Clear();

            foreach (var followUp in cases)
            {
                Cases.Add(followUp);
            }

            if (SelectedCase is not null)
            {
                await LoadAuditAsync(SelectedCase).ConfigureAwait(true);
            }

            StatusMessage = $"{Cases.Count} case(s) in the period.";
        }).ConfigureAwait(true);
    }

    /// <summary>
    /// Loads the print audit trail of the selected case.
    /// </summary>
    [RelayCommand]
    private async Task LoadAuditAsync()
    {
        if (SelectedCase is null)
        {
            return;
        }

        await LoadAuditAsync(SelectedCase).ConfigureAwait(true);
    }

    private async Task LoadAuditAsync(CaseFollowUpDto followUp)
    {
        // The print audit trail is recorded against a test, so the trail of a
        // case is the trail of the tests the case carries.
        var order = await _executor.SendAsync(new GetPatientOrderQuery(followUp.OrderId));

        var testIds = order.Tests.Select(test => test.Id).ToHashSet();

        var audit = await _executor.SendAsync(new GetPrintAuditTrailQuery(
            null,
            PageRequest.First(200)));

        AuditTrail.Clear();

        foreach (var record in audit.Items.Where(record => testIds.Contains(record.OrderTestId)))
        {
            AuditTrail.Add(record);
        }
    }
}
