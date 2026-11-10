using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Statistics;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the statistics and performance screen.
/// </summary>
public sealed partial class StatisticsViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public StatisticsViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Statistics and performance";

    /// <summary>The patient counts by month.</summary>
    public ObservableCollection<PatientCountDto> ByMonth { get; } = new();

    /// <summary>The patient counts by sex.</summary>
    public ObservableCollection<PatientCountDto> BySex { get; } = new();

    /// <summary>The request counts by sub-group.</summary>
    public ObservableCollection<PatientCountDto> BySubGroup { get; } = new();

    /// <summary>The last laboratory statistics.</summary>
    [ObservableProperty]
    private LaboratoryStatisticsDto? _statistics;

    /// <summary>The last performance statistics.</summary>
    [ObservableProperty]
    private PerformanceStatisticsDto? _performance;

    /// <summary>The start of the period.</summary>
    [ObservableProperty]
    private DateTime _from = DateTime.Today.AddMonths(-1);

    /// <summary>The end of the period.</summary>
    [ObservableProperty]
    private DateTime _to = DateTime.Today;

    /// <summary>Runs the laboratory statistics.</summary>
    [RelayCommand]
    private async Task RunLaboratoryStatisticsAsync()
    {
        await RunAsync(async () =>
        {
            var from = new DateTimeOffset(DateOnly.FromDateTime(From.Date), TimeOnly.MinValue, TimeSpan.Zero);
            var to = new DateTimeOffset(DateOnly.FromDateTime(To.Date), TimeOnly.MaxValue, TimeSpan.Zero);

            Statistics = await _executor.SendAsync(new GetLaboratoryStatisticsQuery(from, to));

            ByMonth.Clear();

            foreach (var item in Statistics.ByMonth)
            {
                ByMonth.Add(item);
            }

            BySex.Clear();

            foreach (var item in Statistics.BySex)
            {
                BySex.Add(item);
            }

            BySubGroup.Clear();

            foreach (var item in Statistics.BySubGroup)
            {
                BySubGroup.Add(item);
            }

            StatusMessage = $"{Statistics.PatientCount} patient(s) in the period.";
        }).ConfigureAwait(true);
    }

    /// <summary>Runs the performance statistics.</summary>
    [RelayCommand]
    private async Task RunPerformanceStatisticsAsync()
    {
        await RunAsync(async () =>
        {
            var from = new DateTimeOffset(DateOnly.FromDateTime(From.Date), TimeOnly.MinValue, TimeSpan.Zero);
            var to = new DateTimeOffset(DateOnly.FromDateTime(To.Date), TimeOnly.MaxValue, TimeSpan.Zero);

            Performance = await _executor.SendAsync(new GetPerformanceStatisticsQuery(from, to));

            StatusMessage = $"{Performance.ReviewedResultCount} reviewed result(s) in the period.";
        }).ConfigureAwait(true);
    }
}
