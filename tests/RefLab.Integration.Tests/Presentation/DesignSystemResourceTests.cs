using System.Text.RegularExpressions;

namespace RefLab.Integration.Tests.Presentation;

/// <summary>
/// The design system of the presentation layer, checked as source.
/// </summary>
/// <remarks>
/// <para>
/// A WPF <c>StaticResource</c> that names a key nobody defined does not fail
/// the build. It fails at run time, when the element that carries it is first
/// realised, and it fails with a message that points at the resource rather
/// than at the mistake. On a laboratory workstation that means a screen that
/// does not open.
/// </para>
/// <para>
/// The tests below therefore read the views and the resource dictionaries as
/// source and check that every key a view asks for is a key the design system
/// actually defines, and that the shared styles of the design system are
/// actually used by the views. They are static checks on purpose: they run on
/// the Linux build agent, where the WPF runtime is absent, and they fail the
/// build rather than the laboratory.
/// </para>
/// </remarks>
public sealed class DesignSystemResourceTests
{
    private static readonly Regex StaticResourcePattern = new(
        @"\{StaticResource\s+([A-Za-z0-9_.]+)\}",
        RegexOptions.Compiled);

    private static readonly Regex ResourceKeyPattern = new(
        @"x:Key\s*=\s*""([^""]+)""",
        RegexOptions.Compiled);

    /// <summary>
    /// The shared styles the design system defines and the views must use.
    /// </summary>
    /// <remarks>
    /// The list is the contract of the design system: a style that nothing
    /// uses is not part of the design system, it is a leftover.
    /// </remarks>
    private static readonly string[] RequiredSharedStyles =
    {
        "PanelHeaderStyle",
        "PanelHeaderBorderStyle",
        "FieldLabelStyle",
        "ArabicFieldLabelStyle",
        "LabFieldTextBoxStyle",
        "MetricTileStyle",
        "MetricTileCaptionStyle",
        "SecondaryActionButtonStyle",
        "LabTabItemStyle",
        "LabGridColumnHeaderStyle",
        "LabRangeCellStyle",
        "LabFlagCellStyle",
        "SubHeadingTextStyle",
        "DeepChromePanelStyle",
    };

    /// <summary>
    /// The palette roles the design system defines and the themes must use.
    /// </summary>
    private static readonly string[] RequiredPaletteRoles =
    {
        "HeaderBandBrush",
        "ActionGreenBrush",
        "ActionOrangeBrush",
        "ActionBlueBrush",
        "GridHeaderBrush",
        "FieldFillBrush",
        "FieldLabelBrush",
        "SurfaceAltBrush",
        "BorderBrush",
        "ChromeDeepBrush",
    };

    [Fact]
    public void Every_static_resource_a_view_asks_for_is_defined()
    {
        var root = RepositoryRoot();
        var presentation = Path.Combine(root, "src", "RefLab.Presentation");

        var defined = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in XamlFiles(presentation))
        {
            foreach (Match match in ResourceKeyPattern.Matches(File.ReadAllText(file)))
            {
                defined.Add(match.Groups[1].Value);
            }
        }

        var unresolved = new List<string>();

        foreach (var file in XamlFiles(presentation))
        {
            var content = File.ReadAllText(file);

            foreach (Match match in StaticResourcePattern.Matches(content))
            {
                var key = match.Groups[1].Value;

                if (!defined.Contains(key))
                {
                    unresolved.Add(
                        $"{Path.GetRelativePath(root, file)} asks for '{key}', which no resource dictionary defines.");
                }
            }
        }

        Assert.True(
            unresolved.Count == 0,
            "The views ask for resources that do not exist:" + Environment.NewLine
            + string.Join(Environment.NewLine, unresolved));
    }

    [Fact]
    public void Every_shared_style_of_the_design_system_is_used_by_a_view()
    {
        var root = RepositoryRoot();
        var presentation = Path.Combine(root, "src", "RefLab.Presentation");

        var used = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in XamlFiles(presentation))
        {
            foreach (Match match in StaticResourcePattern.Matches(File.ReadAllText(file)))
            {
                used.Add(match.Groups[1].Value);
            }
        }

        var unused = RequiredSharedStyles.Where(style => !used.Contains(style)).ToArray();

        Assert.True(
            unused.Length == 0,
            "The design system defines styles that no view uses: "
            + string.Join(", ", unused));
    }

    [Fact]
    public void Every_palette_role_is_defined_by_both_themes()
    {
        var root = RepositoryRoot();

        var light = DefinedKeys(Path.Combine(root, "src", "RefLab.Presentation", "Resources", "Themes", "Light.xaml"));
        var dark = DefinedKeys(Path.Combine(root, "src", "RefLab.Presentation", "Resources", "Themes", "Dark.xaml"));

        foreach (var role in RequiredPaletteRoles)
        {
            Assert.Contains(role, light);
            Assert.Contains(role, dark);
        }
    }

    [Fact]
    public void The_two_themes_define_the_same_palette_roles()
    {
        var root = RepositoryRoot();
        var themes = Path.Combine(root, "src", "RefLab.Presentation", "Resources", "Themes");

        // The two themes differ in their colours, not in their structure: the
        // light theme carries the layout styles and the dark theme overrides
        // the palette, so that a surface keeps the same structure whichever
        // theme is applied. What has to match is the set of palette roles —
        // a role that exists in one theme and not in the other would leave a
        // style without a colour.
        var light = PaletteRoles(Path.Combine(themes, "Light.xaml"));
        var dark = PaletteRoles(Path.Combine(themes, "Dark.xaml"));

        var onlyInLight = light.Except(dark).OrderBy(key => key, StringComparer.Ordinal).ToArray();
        var onlyInDark = dark.Except(light).OrderBy(key => key, StringComparer.Ordinal).ToArray();

        Assert.True(
            onlyInLight.Length == 0 && onlyInDark.Length == 0,
            "The two themes must define the same palette roles, or a style is left without a colour. "
            + $"Only in the light theme: {string.Join(", ", onlyInLight)}. "
            + $"Only in the dark theme: {string.Join(", ", onlyInDark)}.");
    }

    /// <summary>
    /// The palette roles a theme defines — its colours and the brushes built
    /// from them. The layout styles are not roles and are not compared.
    /// </summary>
    private static HashSet<string> PaletteRoles(string path)
    {
        var content = File.ReadAllText(path);

        var roles = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match match in Regex.Matches(
            content,
            @"<(Color|SolidColorBrush)\s+[^>]*x:Key\s*=\s*""([^""]+)"""))
        {
            roles.Add(match.Groups[2].Value);
        }

        return roles;
    }

    [Fact]
    public void The_english_and_arabic_dictionaries_keep_disjoint_key_sets()
    {
        var root = RepositoryRoot();
        var resources = Path.Combine(root, "src", "RefLab.Presentation", "Resources");

        var english = DefinedKeys(Path.Combine(resources, "Strings.xaml"));
        var arabic = DefinedKeys(Path.Combine(resources, "Strings.ar.xaml"));

        var shared = english.Intersect(arabic, StringComparer.Ordinal).ToArray();

        Assert.True(
            shared.Length == 0,
            "The English and the Arabic dictionary must not share a key, or a surface "
            + "renders in the wrong language: " + string.Join(", ", shared));
    }

    [Fact]
    public void The_arabic_dictionary_holds_only_arabic()
    {
        var root = RepositoryRoot();
        var path = Path.Combine(root, "src", "RefLab.Presentation", "Resources", "Strings.ar.xaml");

        var content = File.ReadAllText(path);

        var values = Regex.Matches(content, @"<sys:String[^>]*>([^<]*)</sys:String>");

        Assert.NotEmpty(values);

        var withoutArabic = values
            .Cast<Match>()
            .Select(match => match.Groups[1].Value)
            .Where(value => !value.Any(character => character >= '\u0600' && character <= '\u08FF'))
            .ToArray();

        Assert.True(
            withoutArabic.Length == 0,
            "The Arabic dictionary holds a value that carries no Arabic: "
            + string.Join(" | ", withoutArabic));
    }

    private static readonly Regex StyleDefinitionPattern = new(
        @"<Style\b(?<attributes>[^>]*)>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex StyleUsePattern = new(
        @"<(?<element>[A-Za-z_][A-Za-z0-9_.:]*)(?<attributes>[^>]*?)\bStyle\s*=\s*""\s*\{StaticResource\s+(?<key>[A-Za-z0-9_.]+)\s*\}""",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex TargetTypePattern = new(
        @"TargetType\s*=\s*""(?<target>\{[^}]*\}\s*)?(?<type>[A-Za-z_][A-Za-z0-9_.]*)""",
        RegexOptions.Compiled);

    private static readonly Regex BasedOnPattern = new(
        @"BasedOn\s*=\s*""\s*\{StaticResource\s+(?<key>[A-Za-z0-9_.]+)\s*\}""",
        RegexOptions.Compiled);

    /// <summary>
    /// The base type of the WPF element types the design system styles.
    /// </summary>
    /// <remarks>
    /// A WPF <c>Style</c> declares a <c>TargetType</c> and may only be applied
    /// to an element of that type or of a type derived from it. The hierarchy
    /// below lets a source check decide that relationship without the WPF
    /// runtime, so a style borrowed by a sibling control — the check-box style
    /// applied to a radio button — fails the build instead of the laboratory.
    /// </remarks>
    private static readonly Dictionary<string, string?> ElementBaseTypes = new(StringComparer.Ordinal)
    {
        ["FrameworkElement"] = "UIElement",
        ["UIElement"] = null,

        ["Control"] = "FrameworkElement",
        ["ContentControl"] = "Control",
        ["HeaderedContentControl"] = "ContentControl",
        ["HeaderedItemsControl"] = "ItemsControl",
        ["ItemsControl"] = "Control",
        ["Selector"] = "ItemsControl",
        ["MultiSelector"] = "Selector",
        ["RangeBase"] = "Control",

        ["ButtonBase"] = "ContentControl",
        ["Button"] = "ButtonBase",
        ["ToggleButton"] = "ButtonBase",
        ["CheckBox"] = "ToggleButton",
        ["RadioButton"] = "ToggleButton",
        ["RepeatButton"] = "ButtonBase",

        ["TextBoxBase"] = "Control",
        ["TextBox"] = "TextBoxBase",
        ["PasswordBox"] = "Control",
        ["ComboBox"] = "Selector",
        ["ListBox"] = "Selector",
        ["ListView"] = "ListBox",
        ["TabControl"] = "Selector",
        ["TabItem"] = "HeaderedContentControl",
        ["Label"] = "ContentControl",
        ["DatePicker"] = "Control",
        ["ProgressBar"] = "RangeBase",
        ["DataGrid"] = "MultiSelector",
        ["DataGridColumnHeader"] = "ButtonBase",
        ["DataGridCell"] = "ContentControl",
        ["ScrollViewer"] = "ContentControl",
        ["UserControl"] = "ContentControl",
        ["Window"] = "ContentControl",

        ["Panel"] = "FrameworkElement",
        ["Grid"] = "Panel",
        ["DockPanel"] = "Panel",
        ["StackPanel"] = "Panel",
        ["WrapPanel"] = "Panel",

        ["Decorator"] = "FrameworkElement",
        ["Border"] = "Decorator",
        ["TextBlock"] = "FrameworkElement",
        ["Image"] = "FrameworkElement",
    };

    [Fact]
    public void Every_style_a_view_applies_targets_a_compatible_element_type()
    {
        var root = RepositoryRoot();
        var presentation = Path.Combine(root, "src", "RefLab.Presentation");

        var definitions = new Dictionary<string, List<(string TargetType, string? BasedOn)>>(StringComparer.Ordinal);
        var defects = new List<string>();

        foreach (var file in XamlFiles(presentation))
        {
            var content = File.ReadAllText(file);

            foreach (Match match in StyleDefinitionPattern.Matches(content))
            {
                var attributes = match.Groups["attributes"].Value;
                var key = ResourceKeyPattern.Match(attributes);
                var target = TargetTypePattern.Match(attributes);

                if (!key.Success || !target.Success)
                {
                    continue;
                }

                var basedOn = BasedOnPattern.Match(attributes) is { Success: true } basedOnMatch
                    ? basedOnMatch.Groups["key"].Value
                    : null;

                if (!definitions.TryGetValue(key.Groups[1].Value, out var targets))
                {
                    targets = new List<(string, string?)>();
                    definitions[key.Groups[1].Value] = targets;
                }

                targets.Add((target.Groups["type"].Value, basedOn));
            }
        }

        // A BasedOn relationship is only legal when the derived style's target
        // is the base style's target or a type derived from it.
        foreach (var (key, targets) in definitions)
        {
            foreach (var (targetType, basedOn) in targets)
            {
                if (basedOn is null || !definitions.TryGetValue(basedOn, out var bases))
                {
                    continue;
                }

                foreach (var (baseType, _) in bases)
                {
                    if (ElementBaseTypes.ContainsKey(targetType)
                        && ElementBaseTypes.ContainsKey(baseType)
                        && !IsAssignableTo(targetType, baseType))
                    {
                        defects.Add(
                            $"The style '{key}' targets '{targetType}' but inherits from '{basedOn}', "
                            + $"which targets '{baseType}'. A style may only inherit from a style whose "
                            + "target is the same type or a base type.");
                    }
                }
            }
        }

        foreach (var file in XamlFiles(presentation))
        {
            var content = File.ReadAllText(file);
            var relative = Path.GetRelativePath(root, file);

            foreach (Match match in StyleUsePattern.Matches(content))
            {
                var key = match.Groups["key"].Value;
                var element = match.Groups["element"].Value;
                var localElement = element.Contains(':') ? element[(element.IndexOf(':') + 1)..] : element;

                if (!definitions.TryGetValue(key, out var targets)
                    || !ElementBaseTypes.ContainsKey(localElement))
                {
                    continue;
                }

                foreach (var (targetType, _) in targets)
                {
                    if (!ElementBaseTypes.ContainsKey(targetType))
                    {
                        continue;
                    }

                    if (!IsAssignableTo(localElement, targetType))
                    {
                        defects.Add(
                            $"{relative} applies the style '{key}' (TargetType '{targetType}') to "
                            + $"<{element}>, which is not '{targetType}' or a type derived from it.");
                    }
                }
            }
        }

        Assert.True(
            defects.Count == 0,
            "A style is applied to an incompatible element type:" + Environment.NewLine
            + string.Join(Environment.NewLine, defects.Distinct(StringComparer.Ordinal)));
    }

    private static bool IsAssignableTo(string elementType, string styleTargetType)
    {
        var current = elementType;

        while (current is not null)
        {
            if (string.Equals(current, styleTargetType, StringComparison.Ordinal))
            {
                return true;
            }

            current = ElementBaseTypes.TryGetValue(current, out var baseType) ? baseType : null;
        }

        return false;
    }

    private static HashSet<string> DefinedKeys(string path) =>
        new(
            ResourceKeyPattern.Matches(File.ReadAllText(path))
                .Cast<Match>()
                .Select(match => match.Groups[1].Value),
            StringComparer.Ordinal);

    private static IEnumerable<string> XamlFiles(string directory) =>
        Directory.EnumerateFiles(directory, "*.xaml", SearchOption.AllDirectories);

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
