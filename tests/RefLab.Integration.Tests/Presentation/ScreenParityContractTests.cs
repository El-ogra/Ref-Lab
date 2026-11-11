namespace RefLab.Integration.Tests.Presentation;

/// <summary>
/// The parity between the screens of the reference product and the screens of
/// Ref Lab.
/// </summary>
/// <remarks>
/// <para>
/// The presentation test project executes on Windows only, because it loads
/// the WPF runtime. The checks below read the views and the navigation table
/// as source instead, so that they run on the Linux build agent and still
/// fail the build when a screen is dropped, unregistered or stripped of the
/// features the reference product carries.
/// </para>
/// <para>
/// They are deliberately narrow: each one asserts that a specific screen is
/// registered and reachable, and that a specific control is present in a
/// specific view.
/// </para>
/// </remarks>
public sealed class ScreenParityContractTests
{
    [Fact]
    public void The_reception_screen_is_registered_and_reachable()
    {
        var registry = ReadSourceFile(Path.Combine("src", "RefLab.Presentation", "Navigation", "ScreenRegistry.cs"));
        var keys = ReadSourceFile(Path.Combine("src", "RefLab.Presentation", "Navigation", "ScreenKeys.cs"));
        var registration = ReadSourceFile(
            Path.Combine("src", "RefLab.Presentation", "Bootstrap", "PresentationServiceCollectionExtensions.cs"));
        var window = ReadSourceFile(Path.Combine("src", "RefLab.Presentation", "MainWindow.xaml"));

        Assert.Contains("Reception", registry, StringComparison.Ordinal);
        Assert.Contains("ReceptionView", registry, StringComparison.Ordinal);
        Assert.Contains("Reception", keys, StringComparison.Ordinal);
        Assert.Contains("ReceptionViewModel", registration, StringComparison.Ordinal);
        Assert.Contains("ReceptionView", registration, StringComparison.Ordinal);
        Assert.Contains("CommandParameter=\"Reception\"", window, StringComparison.Ordinal);
    }

    [Fact]
    public void The_patient_panel_carries_the_sex_radio_group()
    {
        var view = ReadSourceFile(Path.Combine(
            "src", "RefLab.Presentation", "Views", "Controls", "PatientPanelControl.xaml"));

        Assert.Equal(3, CountOccurrences(view, "<RadioButton"));
        Assert.Contains("ConverterParameter=Male", view, StringComparison.Ordinal);
        Assert.Contains("ConverterParameter=Female", view, StringComparison.Ordinal);
        Assert.Contains("ConverterParameter=Unknown", view, StringComparison.Ordinal);
    }

    [Fact]
    public void The_results_grid_carries_the_low_or_high_column_and_the_normal_range_column()
    {
        var view = ReadSourceFile(Path.Combine(
            "src", "RefLab.Presentation", "Views", "Controls", "ResultsGridControl.xaml"));

        Assert.Contains("LowHighFlag", view, StringComparison.Ordinal);
        Assert.Contains("NormalRangeText", view, StringComparison.Ordinal);
    }

    [Fact]
    public void The_report_setup_screen_carries_the_branding_block()
    {
        var view = ReadSourceFile(Path.Combine(
            "src", "RefLab.Presentation", "Views", "ReportSetupView.xaml"));

        foreach (var field in new[]
        {
            "DoctorSignature",
            "DoctorName",
            "ThanksText",
            "LabLogoPath",
            "ReportLogoPath",
            "LogoWidthPx",
            "LogoHeightPx",
            "HeadingMode",
        })
        {
            Assert.Contains(field, view, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_statistics_screen_carries_the_four_reference_indicator_tiles()
    {
        var view = ReadSourceFile(Path.Combine(
            "src", "RefLab.Presentation", "Views", "StatisticsView.xaml"));

        foreach (var indicator in new[]
        {
            "ByYear",
            "ByMonthAndSex",
            "RequestRates",
            "SamplesByYear",
        })
        {
            Assert.Contains(indicator, view, StringComparison.Ordinal);
        }
    }

    private static int CountOccurrences(string content, string value)
    {
        var count = 0;
        var index = content.IndexOf(value, StringComparison.Ordinal);

        while (index >= 0)
        {
            count++;
            index = content.IndexOf(value, index + value.Length, StringComparison.Ordinal);
        }

        return count;
    }

    private static string ReadSourceFile(string relativePath)
    {
        var root = RepositoryRoot();

        var candidate = Path.Combine(root, relativePath);

        if (File.Exists(candidate))
        {
            return File.ReadAllText(candidate);
        }

        // A file that has moved inside the presentation layer is still found by
        // its name, so that the check follows the code rather than its path.
        var presentation = Path.Combine(root, "src", "RefLab.Presentation");

        var found = Directory
            .EnumerateFiles(presentation, Path.GetFileName(relativePath), SearchOption.AllDirectories)
            .FirstOrDefault();

        if (found is not null)
        {
            return File.ReadAllText(found);
        }

        throw new InvalidOperationException(
            "The repository file was not found: " + relativePath + " above " + AppContext.BaseDirectory + ".");
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "RefLab.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "The repository root was not found: no RefLab.sln above " + AppContext.BaseDirectory + ".");
    }
}
