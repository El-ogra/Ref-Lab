using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Common;
using RefLab.Application.Features.Auditing;
using RefLab.Domain.Enums;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the audit trail and attendance screen.
/// </summary>
public sealed partial class AuditViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public AuditViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Audit trail and attendance";

    /// <summary>The print audit trail.</summary>
    public ObservableCollection<PrintAuditDto> PrintAudits { get; } = new();

    /// <summary>The attendance entries.</summary>
    public ObservableCollection<AttendanceEntryDto> Attendance { get; } = new();

    /// <summary>The page of the audit trail that is shown.</summary>
    [ObservableProperty]
    private int _pageNumber = 1;

    /// <summary>The number of records on a page.</summary>
    [ObservableProperty]
    private int _pageSize = 25;

    /// <summary>The total number of audit records.</summary>
    [ObservableProperty]
    private int _totalCount;

    /// <summary>Reloads the audit trail.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var page = new PageRequest(PageNumber, PageSize);

            var result = await _executor.SendAsync(new GetPrintAuditTrailQuery(null, page));

            PrintAudits.Clear();

            foreach (var audit in result.Items)
            {
                PrintAudits.Add(audit);
            }

            TotalCount = result.TotalCount;

            StatusMessage = $"{result.TotalCount} audit record(s).";
        }).ConfigureAwait(true);
    }

    /// <summary>Records an attendance event for the signed-in operator.</summary>
    [RelayCommand]
    private async Task RecordAttendanceAsync()
    {
        await RunAsync(async () =>
        {
            var entry = await _executor.SendAsync(new RecordAttendanceCommand(AttendanceEventType.StartedWork));

            Attendance.Add(entry);

            StatusMessage = "The attendance event was recorded.";
        }).ConfigureAwait(true);
    }
}
