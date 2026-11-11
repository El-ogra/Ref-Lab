using System.Text.RegularExpressions;

namespace RefLab.Integration.Tests.Presentation;

/// <summary>
/// The contract between the views and the view models.
/// </summary>
/// <remarks>
/// <para>
/// A WPF binding to a property that does not exist fails silently at run
/// time: the control simply never raises its command, and the application
/// looks as though the operator pressed the wrong button. That is exactly
/// what happened in the presentation layer — sixty-five bindings across
/// fourteen views referred to command properties that the MVVM toolkit does
/// not generate, because the toolkit strips the trailing <c>Async</c> from a
/// method name when it derives the name of the command.
/// </para>
/// <para>
/// The tests below read the views and the view models as source and check
/// that every binding resolves. They are static checks on purpose: they run
/// on the Linux build agent, where the WPF runtime is absent, and they fail
/// the build rather than the laboratory.
/// </para>
/// <para>
/// The run-time behaviour of the commands behind those bindings is covered by
/// the presentation test project, which executes on Windows.
/// </para>
/// </remarks>
public sealed class PresentationSourceContractTests
{
    private static readonly Regex BindingPattern = new(
        @"\{Binding\s+([A-Za-z0-9_]+Command)\}",
        RegexOptions.Compiled);

    private static readonly Regex RelayCommandPattern = new(
        @"\[RelayCommand\]\s*\r?\n\s*private\s+(?:async\s+)?(?:Task|void)\s+([A-Za-z0-9_]+)\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex KeyBindingPattern = new(
        @"<KeyBinding\s+Key=""(?<key>[^""]+)""\s+Command=""\{Binding\s+(?<command>[A-Za-z0-9_]+)\}""",
        RegexOptions.Compiled);

    [Fact]
    public void Every_command_binding_of_every_view_resolves_against_its_view_model()
    {
        var root = RepositoryRoot();

        var viewModels = LoadViewModels(root);

        Assert.NotEmpty(viewModels);

        var unresolved = new List<string>();

        foreach (var view in LoadViews(root))
        {
            var viewModelName = Path.GetFileNameWithoutExtension(view.RelativePath).Replace("View", "ViewModel");

            if (!viewModels.TryGetValue(viewModelName, out var commands))
            {
                // A view without a view model of the same name is not covered
                // by this check; the controls are bound to the shell or to a
                // panel that carries its own view model.
                continue;
            }

            foreach (Match match in BindingPattern.Matches(view.Content))
            {
                var bound = match.Groups[1].Value;

                if (!commands.Contains(bound))
                {
                    unresolved.Add($"{view.RelativePath} binds {bound}, which {viewModelName} does not declare.");
                }
            }
        }

        Assert.True(
            unresolved.Count == 0,
            "The views bind commands that do not exist:" + Environment.NewLine +
            string.Join(Environment.NewLine, unresolved));
    }

    [Fact]
    public void Every_keyboard_shortcut_of_the_patient_work_screen_binds_a_real_command()
    {
        var root = RepositoryRoot();

        var view = Path.Combine(root, "src", "RefLab.Presentation", "Views", "PatientWorkView.xaml");

        Assert.True(File.Exists(view), $"The patient work view was not found at {view}.");

        var content = File.ReadAllText(view);

        var commands = LoadViewModels(root)["PatientWorkViewModel"];

        var shortcuts = new List<string>();

        foreach (Match match in KeyBindingPattern.Matches(content))
        {
            var key = match.Groups["key"].Value;
            var command = match.Groups["command"].Value;

            if (!commands.Contains(command))
            {
                shortcuts.Add($"F-key {key} is bound to {command}, which PatientWorkViewModel does not declare.");
            }
        }

        Assert.True(
            shortcuts.Count == 0,
            "The keyboard shortcuts are bound to commands that do not exist:" + Environment.NewLine +
            string.Join(Environment.NewLine, shortcuts));
    }

    [Fact]
    public void The_four_legacy_shortcuts_are_declared()
    {
        var root = RepositoryRoot();

        var view = Path.Combine(root, "src", "RefLab.Presentation", "Views", "PatientWorkView.xaml");

        var content = File.ReadAllText(view);

        foreach (var key in new[] { "F8", "F9", "F11", "F12" })
        {
            Assert.Contains($"Key=\"{key}\"", content, StringComparison.Ordinal);
        }
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

    private static Dictionary<string, HashSet<string>> LoadViewModels(string root)
    {
        var directory = Path.Combine(root, "src", "RefLab.Presentation", "ViewModels");

        var result = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(file);

            var classMatch = Regex.Match(content, @"public sealed partial class (\w+)");

            if (!classMatch.Success)
            {
                continue;
            }

            var commands = new HashSet<string>(StringComparer.Ordinal);

            foreach (Match match in RelayCommandPattern.Matches(content))
            {
                var method = match.Groups[1].Value;

                // The MVVM toolkit strips a trailing "Async" from the method
                // name when it derives the name of the generated command.
                commands.Add(method.EndsWith("Async", StringComparison.Ordinal)
                    ? method[..^"Async".Length] + "Command"
                    : method + "Command");
            }

            if (commands.Count > 0)
            {
                result[classMatch.Groups[1].Value] = commands;
            }
        }

        return result;
    }

    private static IEnumerable<(string RelativePath, string Content)> LoadViews(string root)
    {
        var directory = Path.Combine(root, "src", "RefLab.Presentation", "Views");

        foreach (var file in Directory.EnumerateFiles(directory, "*.xaml", SearchOption.AllDirectories))
        {
            yield return (Path.GetRelativePath(root, file), File.ReadAllText(file));
        }
    }
}
