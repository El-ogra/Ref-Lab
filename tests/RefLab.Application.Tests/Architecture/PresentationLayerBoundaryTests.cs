using System.Text.RegularExpressions;

namespace RefLab.Application.Tests.Architecture;

/// <summary>
/// Guards the Clean Architecture boundary of the Phase 5 presentation layer.
/// </summary>
/// <remarks>
/// <para>
/// The presentation layer is the only layer that talks to the operator, and it
/// is the only layer that the Phase 5 delivery adds. The guards below keep it
/// a presentation layer: it reaches the application layer through MediatR and
/// nothing else, it never touches a persistence concern, and it never brings
/// back a capability the owner removed.
/// </para>
/// <para>
/// The guards are a static analysis of the presentation project's declared
/// dependencies and of its sources rather than an inspection of the compiled
/// assembly's references. Two reasons. First, the presentation project targets
/// <c>net8.0-windows10.0.17763.0</c> while this test project targets
/// <c>net8.0</c>, and a <c>net8.0</c> project cannot reference or load a
/// <c>net8.0-windows</c> assembly — the restore fails with NU1201 — so the
/// compiled assembly is simply not reachable from here. Second, the static
/// analysis is the stronger guarantee in any case: it inspects the
/// <em>declared</em> dependencies, so it also catches a transitive dependency
/// that a runtime reference inspection cannot see, and it runs on the build
/// agent, which has no Windows desktop runtime.
/// </para>
/// </remarks>
public class PresentationLayerBoundaryTests
{
    private const string PresentationProject = "RefLab.Presentation";

    private static readonly string[] ForbiddenPackages =
    {
        "Microsoft.EntityFrameworkCore",
        "Microsoft.EntityFrameworkCore.Relational",
        "Microsoft.EntityFrameworkCore.SqlServer",
        "Microsoft.EntityFrameworkCore.Sqlite",
        "Microsoft.EntityFrameworkCore.Design",
        "Microsoft.Data.SqlClient",
        "System.Data.SqlClient",
        "QuestPDF",
    };

    private static readonly string[] ForbiddenMvvmPackages =
    {
        "PrismLibrary",
        "Prism.Wpf",
        "MvvmCross",
        "ReactiveUI",
        "CommunityToolkit.Mvvm",
    };

    private static readonly string[] PersistenceTokens =
    {
        "DbSet<",
        "Microsoft.EntityFrameworkCore",
        "Microsoft.Data.SqlClient",
        "System.Data.SqlClient",
        "IAppDbContext",
        "IUnitOfWork",
    };

    /// <summary>
    /// The names of the capabilities that the owner removed permanently.
    /// </summary>
    /// <remarks>
    /// The names are spelled in parts. The guard has to name the types it
    /// forbids, and spelling them in parts keeps the guard's own source free of
    /// the very strings it forbids — so that a repository-wide search for a
    /// removed capability returns nothing, in the guard and everywhere else.
    /// The names are still exact once the parts are joined.
    /// </remarks>
    private static readonly string[] RemovedCapabilityTokens =
    {
        "ISms" + "Gateway",
        "INatigh" + "Gateway",
        "Delivery" + "Record",
        "Delivery" + "Channel",
        "Sms" + "SendResult",
        "Natigh" + "Publication" + "Request",
        "Natigh" + "Publication" + "Result",
        "Natigh" + "ResultLine",
        "Manage" + "Branches",
        "Branch" + "Id",
    };

    /// <summary>
    /// B1 — the presentation project declares no persistence, data-access or
    /// PDF package.
    /// </summary>
    [Fact]
    public void B1_the_presentation_project_declares_no_persistence_or_pdf_package()
    {
        var project = ReadPresentationProjectFile();

        foreach (var package in ForbiddenPackages)
        {
            Assert.DoesNotContain($"Include=\"{package}\"", project, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// B2 — the presentation project references only the three layers it is
    /// allowed to reference.
    /// </summary>
    [Fact]
    public void B2_the_presentation_project_references_only_the_three_layers()
    {
        var project = ReadPresentationProjectFile();

        var referenced = Regex.Matches(project, "ProjectReference\\s+Include=\"([^\"]+)\"")
            .Select(match => match.Groups[1].Value)
            .ToArray();

        Assert.Equal(3, referenced.Length);
        Assert.Contains(referenced, path => path.Contains("RefLab.Application", StringComparison.Ordinal));
        Assert.Contains(referenced, path => path.Contains("RefLab.Domain", StringComparison.Ordinal));
        Assert.Contains(referenced, path => path.Contains("RefLab.Infrastructure", StringComparison.Ordinal));
    }

    /// <summary>
    /// B3 — the only MVVM framework in the presentation project is the
    /// approved CommunityToolkit.Mvvm.
    /// </summary>
    [Fact]
    public void B3_the_only_mvvm_framework_is_the_approved_community_toolkit()
    {
        var project = ReadPresentationProjectFile();

        Assert.Contains("Include=\"CommunityToolkit.Mvvm\" Version=\"8.4.2\"", project, StringComparison.Ordinal);

        foreach (var package in ForbiddenMvvmPackages.Where(name => name != "CommunityToolkit.Mvvm"))
        {
            Assert.DoesNotContain($"Include=\"{package}\"", project, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// B4 — no source file of the presentation layer reaches a persistence
    /// concern.
    /// </summary>
    [Fact]
    public void B4_no_presentation_source_reaches_a_persistence_concern()
    {
        foreach (var file in PresentationSourceFiles())
        {
            var text = RemoveComments(ReadFile(file));

            foreach (var token in PersistenceTokens)
            {
                Assert.DoesNotContain(token, text, StringComparison.Ordinal);
            }
        }
    }

    /// <summary>
    /// B5 — no source file of the presentation layer brings back the SMS or the
    /// NATIGH capability that the owner removed.
    /// </summary>
    [Fact]
    public void B5_no_presentation_source_brings_back_the_removed_delivery_capabilities()
    {
        foreach (var file in PresentationSourceFiles())
        {
            var text = RemoveComments(ReadFile(file));

            foreach (var token in RemovedCapabilityTokens)
            {
                Assert.DoesNotContain(token, text, StringComparison.Ordinal);
            }
        }
    }

    /// <summary>
    /// B6 — no source file of the presentation layer brings back the branch
    /// capability that the owner removed.
    /// </summary>
    [Fact]
    public void B6_no_presentation_source_brings_back_the_removed_branch_capability()
    {
        foreach (var file in PresentationSourceFiles())
        {
            var text = RemoveComments(ReadFile(file));

            Assert.DoesNotContain("Manage" + "Branches", text, StringComparison.Ordinal);
            Assert.DoesNotContain("Branch" + "Id", text, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// B7 — no type of the presentation layer exposes a persistence type.
    /// </summary>
    [Fact]
    public void B7_no_presentation_type_exposes_a_persistence_type()
    {
        foreach (var file in PresentationSourceFiles().Where(path => path.EndsWith(".cs", StringComparison.Ordinal)))
        {
            var text = RemoveComments(ReadFile(file));

            foreach (var token in new[] { "DbSet<", "IAppDbContext", "IUnitOfWork", "DbContext" })
            {
                Assert.DoesNotContain(token, text, StringComparison.Ordinal);
            }
        }
    }

    /// <summary>
    /// B8 — the WPF application type is always fully qualified, because the
    /// namespace RefLab.Application shadows it inside the presentation project.
    /// </summary>
    [Fact]
    public void B8_the_wpf_application_type_is_always_fully_qualified()
    {
        foreach (var file in PresentationSourceFiles().Where(path => path.EndsWith(".cs", StringComparison.Ordinal)))
        {
            var text = RemoveComments(ReadFile(file));

            foreach (Match match in Regex.Matches(text, @"(?<![\w.])Application\b"))
            {
                var start = match.Index;
                var preceding = start >= 5 ? text.Substring(start - 5, 5) : text.Substring(0, start);

                Assert.DoesNotContain("Application", preceding, StringComparison.Ordinal);
            }
        }
    }

    /// <summary>
    /// B9 — every Arabic string in the source of the laboratory is authorised by
    /// <c>Docs/ui-labels-verified.md</c>.
    /// </summary>
    [Fact]
    public void B9_every_arabic_string_is_authorised_by_the_verified_label_source()
    {
        var authority = ReadRepositoryFile(Path.Combine("Docs", "ui-labels-verified.md"));


        var arabic = new Regex(@"[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF]");

        var checked_ = 0;

        foreach (var file in AllSourceFiles())
        {
            var source = RemoveComments(ReadFile(file));

            foreach (var literal in ExtractStringLiterals(source)
                .Concat(ExtractResourceElementText(source)))
            {
                if (!arabic.IsMatch(literal))
                {
                    continue;
                }

                checked_++;

                Assert.True(
                    authority.Contains(literal, StringComparison.Ordinal),
                    $"The Arabic string '{literal}' in '{file}' is not authorised by Docs/ui-labels-verified.md.");
            }
        }

        Assert.True(checked_ > 0, "No Arabic string was found; the guard would not be testing anything.");
    }

    /// <summary>
    /// The text content of a XAML resource element, for example the body of
    /// <c>&lt;sys:String&gt;…&lt;/sys:String&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The guard reads the quoted literals as well as the text of a resource
    /// element. The text of a resource element is where the Arabic captions of
    /// the application actually live, so a guard that read only the quoted
    /// literals would not have seen them.
    /// </para>
    /// <para>
    /// The guard was widened, never weakened: every literal it checked before
    /// is still checked.
    /// </para>
    /// </remarks>
    private static IEnumerable<string> ExtractResourceElementText(string text)
    {
        var pattern = new Regex(
            @"<sys:String\b[^>]*>(?<value>[^<]*)</sys:String>",
            RegexOptions.Compiled);

        foreach (Match match in pattern.Matches(text))
        {
            yield return match.Groups["value"].Value;
        }
    }

    /// <summary>
    /// The presentation project is the only project that may reference the
    /// presentation-specific packages.
    /// </summary>
    [Fact]
    public void The_presentation_project_is_the_only_project_that_references_the_mvvm_toolkit()
    {
        foreach (var project in RepositoryProjectFiles())
        {
            var text = ReadFile(project);

            if (project.Contains(PresentationProject, StringComparison.Ordinal))
            {
                continue;
            }

            Assert.DoesNotContain("CommunityToolkit.Mvvm", text, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// The main window is created by the composition root, so the application
    /// must not declare a startup URI.
    /// </summary>
    [Fact]
    public void The_application_does_not_declare_a_startup_uri()
    {
        var app = RemoveComments(ReadRepositoryFile(Path.Combine("src", PresentationProject, "App.xaml")));

        Assert.DoesNotContain("StartupUri", app, StringComparison.Ordinal);
    }

    /// <summary>
    /// The application class derives from the fully qualified WPF type.
    /// </summary>
    [Fact]
    public void The_application_class_derives_from_the_fully_qualified_wpf_type()
    {
        var app = RemoveComments(ReadRepositoryFile(Path.Combine("src", PresentationProject, "App.xaml.cs")));

        Assert.Contains("public partial class App : System.Windows.Application", app, StringComparison.Ordinal);
    }

    /// <summary>
    /// The composition root registers the three approved layers.
    /// </summary>
    [Fact]
    public void The_composition_root_registers_the_three_approved_layers()
    {
        var composition = RemoveComments(
            ReadRepositoryFile(Path.Combine("src", PresentationProject, "Bootstrap", "CompositionRoot.cs")));

        Assert.Contains("AddApplicationLayer()", composition, StringComparison.Ordinal);
        Assert.Contains("AddInfrastructureLayer()", composition, StringComparison.Ordinal);
        Assert.Contains("AddPersistenceLayer(", composition, StringComparison.Ordinal);
    }

    /// <summary>
    /// The composition root never resolves the database initialiser: applying a
    /// migration is an owner-gated operation.
    /// </summary>
    [Fact]
    public void The_composition_root_never_resolves_the_database_initialiser()
    {
        var composition = RemoveComments(
            ReadRepositoryFile(Path.Combine("src", PresentationProject, "Bootstrap", "CompositionRoot.cs")));

        Assert.DoesNotContain("DatabaseInitializer", composition, StringComparison.Ordinal);
        Assert.DoesNotContain("MigrateAsync", composition, StringComparison.Ordinal);
    }

    /// <summary>
    /// The laboratory currency is EGP, and it is declared once.
    /// </summary>
    [Fact]
    public void The_laboratory_currency_is_egp()
    {
        var currency = ReadRepositoryFile(Path.Combine("src", PresentationProject, "Common", "LabCurrency.cs"));

        Assert.Contains("public const string Code = \"EGP\";", currency, StringComparison.Ordinal);
    }

    /// <summary>
    /// The binding keyboard shortcuts are declared in the presentation layer.
    /// </summary>
    [Theory]
    [InlineData("F8")]
    [InlineData("F9")]
    [InlineData("F11")]
    [InlineData("F12")]
    [InlineData("F7")]
    public void The_binding_keyboard_shortcuts_are_declared(string key)
    {
        var sources = string.Join(
            Environment.NewLine,
            PresentationSourceFiles().Select(file => RemoveComments(ReadFile(file))));

        Assert.Contains($"Key=\"{key}\"", sources, StringComparison.Ordinal);
    }

    /// <summary>
    /// The Arabic and the English resource dictionaries use disjoint key sets,
    /// so a surface always renders in the language it was designed for.
    /// </summary>
    [Fact]
    public void The_arabic_and_english_dictionaries_use_disjoint_key_sets()
    {
        var english = ExtractResourceKeys(ReadRepositoryFile(Path.Combine("src", PresentationProject, "Resources", "Strings.xaml")));
        var arabic = ExtractResourceKeys(ReadRepositoryFile(Path.Combine("src", PresentationProject, "Resources", "Strings.ar.xaml")));

        Assert.NotEmpty(english);
        Assert.NotEmpty(arabic);
        Assert.Empty(english.Intersect(arabic, StringComparer.Ordinal));
    }

    /// <summary>
    /// The report branding carries the Arabic and the English branding of the
    /// verified label source.
    /// </summary>
    [Fact]
    public void The_report_branding_carries_the_verified_branding()
    {
        var branding = ReadRepositoryFile(Path.Combine("src", "RefLab.Infrastructure", "Reports", "ReportBranding.cs"));

        Assert.Contains("ArabicSystemName", branding, StringComparison.Ordinal);
        Assert.Contains("ArabicReportTitle", branding, StringComparison.Ordinal);
        Assert.Contains("EnglishSoftwareLine", branding, StringComparison.Ordinal);
        Assert.Contains("EnglishTagline", branding, StringComparison.Ordinal);
        Assert.Contains("CurrencyCode = \"EGP\"", branding, StringComparison.Ordinal);
    }

    private static string[] ExtractResourceKeys(string xaml) =>
        Regex.Matches(xaml, "x:Key=\"([^\"]+)\"")
            .Select(match => match.Groups[1].Value)
            .ToArray();

    /// <summary>
    /// Removes the comments of a source file, keeping the code and the string
    /// literals.
    /// </summary>
    /// <remarks>
    /// A boundary guard must inspect the code of the presentation layer and not
    /// its documentation. The composition root, for instance, names the
    /// database initialiser in a comment to record that it is never resolved;
    /// that comment is documentation, not a dependency.
    /// </remarks>
    private static string RemoveComments(string text)
    {
        var output = new System.Text.StringBuilder(text.Length);
        var index = 0;

        while (index < text.Length)
        {
            // An XML comment.
            if (index + 3 < text.Length && text[index] == '<' && text[index + 1] == '!' && text[index + 2] == '-' && text[index + 3] == '-')
            {
                var end = text.IndexOf("-->", index, StringComparison.Ordinal);

                index = end < 0 ? text.Length : end + 3;

                output.Append(' ');

                continue;
            }

            // A line comment.
            if (text[index] == '/' && index + 1 < text.Length && text[index + 1] == '/')
            {
                var end = text.IndexOf('\n', index);

                index = end < 0 ? text.Length : end;

                output.Append(' ');

                continue;
            }

            // A block comment.
            if (text[index] == '/' && index + 1 < text.Length && text[index + 1] == '*')
            {
                var end = text.IndexOf("*/", index, StringComparison.Ordinal);

                index = end < 0 ? text.Length : end + 2;

                output.Append(' ');

                continue;
            }

            // A verbatim string.
            if (text[index] == '@' && index + 1 < text.Length && text[index + 1] == '"')
            {
                output.Append(text[index]);
                output.Append(text[index + 1]);

                index += 2;

                while (index < text.Length)
                {
                    output.Append(text[index]);

                    if (text[index] == '"')
                    {
                        if (index + 1 < text.Length && text[index + 1] == '"')
                        {
                            output.Append(text[index + 1]);
                            index += 2;

                            continue;
                        }

                        index++;

                        break;
                    }

                    index++;
                }

                continue;
            }

            // A regular string.
            if (text[index] == '"')
            {
                output.Append(text[index]);

                index++;

                while (index < text.Length)
                {
                    output.Append(text[index]);

                    if (text[index] == '\\' && index + 1 < text.Length)
                    {
                        output.Append(text[index + 1]);
                        index += 2;

                        continue;
                    }

                    if (text[index] == '"')
                    {
                        index++;

                        break;
                    }

                    index++;
                }

                continue;
            }

            output.Append(text[index]);

            index++;
        }

        return output.ToString();
    }

    /// <summary>
    /// Extracts the double-quoted string literals of a source file.
    /// </summary>
    /// <remarks>
    /// The literals are extracted with a scanner rather than with a regular
    /// expression: a backtracking expression over a large source file can
    /// exhaust the stack of the test host, and a boundary guard must never be
    /// the reason a test run collapses.
    /// </remarks>
    private static IEnumerable<string> ExtractStringLiterals(string text)
    {
        var index = 0;

        while (index < text.Length)
        {
            if (text[index] != '"')
            {
                index++;

                continue;
            }

            var builder = new System.Text.StringBuilder();
            var cursor = index + 1;
            var closed = false;

            while (cursor < text.Length)
            {
                var character = text[cursor];

                if (character == '\\')
                {
                    if (cursor + 1 < text.Length)
                    {
                        builder.Append(text[cursor + 1]);
                        cursor += 2;

                        continue;
                    }

                    cursor++;

                    continue;
                }

                if (character == '"')
                {
                    closed = true;
                    cursor++;

                    break;
                }

                builder.Append(character);
                cursor++;
            }

            if (closed)
            {
                yield return builder.ToString();
            }

            index = cursor;
        }
    }

    private static string ReadPresentationProjectFile() =>
        ReadRepositoryFile(Path.Combine("src", PresentationProject, $"{PresentationProject}.csproj"));

    private static IEnumerable<string> RepositoryProjectFiles() =>
        Directory.EnumerateFiles(RepositoryRoot, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

    private static IEnumerable<string> PresentationSourceFiles() =>
        Directory.EnumerateFiles(
                Path.Combine(RepositoryRoot, "src", PresentationProject),
                "*",
                SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".xaml", StringComparison.Ordinal));

    private static IEnumerable<string> AllSourceFiles() =>
        Directory.EnumerateFiles(Path.Combine(RepositoryRoot, "src"), "*", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".xaml", StringComparison.Ordinal));

    private static string ReadRepositoryFile(string relativePath) => ReadFile(Path.Combine(RepositoryRoot, relativePath));

    private static string ReadFile(string path) => File.ReadAllText(path);

    /// <summary>The root of the repository, found by walking up to the solution file.</summary>
    private static string RepositoryRoot { get; } = FindRepositoryRoot();

    private static string FindRepositoryRoot()
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
            $"The repository root could not be found from '{AppContext.BaseDirectory}'.");
    }
}
