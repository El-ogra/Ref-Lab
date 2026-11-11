using System.Text.RegularExpressions;

namespace RefLab.Integration.Tests.Presentation;

/// <summary>
/// The contract between the foundational phases and the views.
/// </summary>
/// <remarks>
/// <para>
/// The presentation test project executes on Windows only, because it loads
/// the WPF runtime. The checks below read the views as source instead, so
/// that they run on the Linux build agent and still fail the build when a
/// phase changes a view without changing the binding that goes with it.
/// </para>
/// <para>
/// They are deliberately narrow: each one asserts that a specific binding
/// introduced by a foundational phase is present in the view it belongs to.
/// </para>
/// </remarks>
public sealed class FoundationalPhaseContractTests
{
    [Fact]
    public void The_results_grid_shows_the_low_or_high_flag_and_the_normal_range()
    {
        var view = ReadView("Views", "Controls", "ResultsGridControl.xaml");

        Assert.Contains("Binding=\"{Binding LowHighFlag}\"", view, StringComparison.Ordinal);
        Assert.Contains("Binding=\"{Binding NormalRangeText}\"", view, StringComparison.Ordinal);
        Assert.Contains("StaticResource Column.LowOrHigh", view, StringComparison.Ordinal);
        Assert.Contains("StaticResource Column.NormalRange", view, StringComparison.Ordinal);
    }

    [Fact]
    public void The_results_grid_colours_the_flag_and_the_range()
    {
        var view = ReadView("Views", "Controls", "ResultsGridControl.xaml");

        Assert.Contains("StaticResource LabFlagCellStyle", view, StringComparison.Ordinal);
        Assert.Contains("StaticResource LabRangeCellStyle", view, StringComparison.Ordinal);
    }

    [Fact]
    public void The_patient_panel_carries_the_referring_party()
    {
        var view = ReadView("Views", "Controls", "PatientPanelControl.xaml");

        Assert.Contains("StaticResource Ar.PatientPanel.ReferringParty", view, StringComparison.Ordinal);
        Assert.Contains("Text=\"{Binding ReferringPartyCode", view, StringComparison.Ordinal);
    }

    [Fact]
    public void The_shell_offers_every_registered_screen()
    {
        var window = ReadView("MainWindow.xaml");
        var registry = ReadRepositoryFile(Path.Combine("src", "RefLab.Presentation", "Navigation", "ScreenRegistry.cs"));

        var registered = Regex.Matches(registry, @"ScreenKeys\.([A-Za-z]+)")
            .Cast<Match>()
            .Select(match => match.Groups[1].Value)
            .Where(key => key != "SignIn")
            .Distinct()
            .ToArray();

        Assert.NotEmpty(registered);

        var missing = registered
            .Where(key => !window.Contains($"CommandParameter=\"{key}\"", StringComparison.Ordinal))
            .ToArray();

        Assert.True(
            missing.Length == 0,
            "The shell toolbar cannot reach these registered screens: " + string.Join(", ", missing));
    }

    [Fact]
    public void The_shell_status_strip_shows_the_operator_and_the_workstation()
    {
        var window = ReadView("MainWindow.xaml");

        Assert.Contains("StaticResource Shell.UserName", window, StringComparison.Ordinal);
        Assert.Contains("StaticResource Shell.LastLogin", window, StringComparison.Ordinal);
        Assert.Contains("StaticResource Shell.Database", window, StringComparison.Ordinal);
        Assert.Contains("StaticResource Shell.Today", window, StringComparison.Ordinal);
        Assert.Contains("Text=\"{Binding LastSignInText}\"", window, StringComparison.Ordinal);
        Assert.Contains("Text=\"{Binding TodayText}\"", window, StringComparison.Ordinal);
    }

    [Fact]
    public void The_patient_work_screen_binds_the_patient_panel_and_the_results_grid()
    {
        var view = ReadView("Views", "PatientWorkView.xaml");

        Assert.Contains("controls:PatientPanelControl", view, StringComparison.Ordinal);
        Assert.Contains("controls:ResultsGridControl", view, StringComparison.Ordinal);
        Assert.Contains("StaticResource Panel.Results", view, StringComparison.Ordinal);
    }

    [Fact]
    public void The_design_system_styles_are_the_ones_the_views_use()
    {
        var panel = ReadView("Views", "Controls", "PatientPanelControl.xaml");
        var catalogue = ReadView("Views", "Controls", "CataloguePanelControl.xaml");
        var signIn = ReadView("Views", "SignInView.xaml");

        // The Arabic panel, the English catalogue panel and the sign-in card
        // are three surfaces of the same application; they must be built from
        // the same design system.
        Assert.Contains("StaticResource ArabicFieldLabelStyle", panel, StringComparison.Ordinal);
        Assert.Contains("StaticResource PanelHeaderBorderStyle", catalogue, StringComparison.Ordinal);
        Assert.Contains("StaticResource LabFieldTextBoxStyle", signIn, StringComparison.Ordinal);
        Assert.Contains("StaticResource PanelHeaderBorderStyle", signIn, StringComparison.Ordinal);
    }

    private static string ReadView(params string[] segments) =>
        ReadRepositoryFile(Path.Combine(
            new[] { "src", "RefLab.Presentation" }.Concat(segments).ToArray()));

    private static string ReadRepositoryFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);

            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "The repository file was not found: " + relativePath + " above " + AppContext.BaseDirectory + ".");
    }
}
