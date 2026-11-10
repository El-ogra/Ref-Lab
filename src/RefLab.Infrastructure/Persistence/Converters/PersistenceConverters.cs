using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RefLab.Domain.Configuration;
using RefLab.Domain.Identity;
using RefLab.Domain.Microbiology;
using RefLab.Domain.ValueObjects;

namespace RefLab.Infrastructure.Persistence.Converters;

/// <summary>
/// The serialiser options shared by the JSON conversions.
/// </summary>
/// <remarks>
/// The options are passed explicitly at every call site because an
/// expression tree may not use an optional argument.
/// </remarks>
internal static class JsonDefaults
{
    /// <summary>The shared options.</summary>
    internal static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.General);
}

/// <summary>
/// Maps the patient identifier onto the GUID it wraps.
/// </summary>
/// <remarks>
/// The identifier is a value object in the domain and a plain GUID column in
/// the database. Every reference to a patient — the order, the history sheet,
/// the picture and the culture result — uses the same conversion, so the
/// foreign keys line up with the primary key.
/// </remarks>
internal sealed class PatientIdConverter : ValueConverter<PatientId, Guid>
{
    /// <summary>Initialises a new converter.</summary>
    public PatientIdConverter()
        : base(id => id.Value, value => new PatientId(value))
    {
    }
}

/// <summary>
/// Maps a list of nullable strings onto a single JSON column.
/// </summary>
internal sealed class NullableStringListConverter : ValueConverter<IReadOnlyList<string?>, string>
{
    /// <summary>Initialises a new converter.</summary>
    public NullableStringListConverter()
        : base(
            values => JsonSerializer.Serialize((values ?? Array.Empty<string?>()).ToArray(), JsonDefaults.Options),
            value => Deserialize(value))
    {
    }

    private static IReadOnlyList<string?> Deserialize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<string?>();
        }

        try
        {
            return JsonSerializer.Deserialize<string?[]>(value, JsonDefaults.Options) ?? Array.Empty<string?>();
        }
        catch (JsonException)
        {
            return Array.Empty<string?>();
        }
    }
}

/// <summary>
/// Maps a list of strings onto a single JSON column.
/// </summary>
internal sealed class StringListConverter : ValueConverter<IReadOnlyList<string>, string>
{
    /// <summary>Initialises a new converter.</summary>
    public StringListConverter()
        : base(
            values => JsonSerializer.Serialize((values ?? Array.Empty<string>()).ToArray(), JsonDefaults.Options),
            value => Deserialize(value))
    {
    }

    private static IReadOnlyList<string> Deserialize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<string[]>(value, JsonDefaults.Options) ?? Array.Empty<string>();
        }
        catch (JsonException)
        {
            return Array.Empty<string>();
        }
    }
}

/// <summary>
/// Maps the nullable string array of the history sheet onto a single JSON column.
/// </summary>
internal sealed class StringArrayConverter : ValueConverter<string?[], string>
{
    /// <summary>Initialises a new converter.</summary>
    public StringArrayConverter()
        : base(
            values => JsonSerializer.Serialize(values ?? new string?[PatientHistorySlotCount], JsonDefaults.Options),
            value => Deserialize(value))
    {
    }

    /// <summary>
    /// The number of free-form slots a history sheet carries.
    /// </summary>
    private const int PatientHistorySlotCount = 20;

    private static string?[] Deserialize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new string?[PatientHistorySlotCount];
        }

        try
        {
            return JsonSerializer.Deserialize<string?[]>(value, JsonDefaults.Options) ?? new string?[PatientHistorySlotCount];
        }
        catch (JsonException)
        {
            return new string?[PatientHistorySlotCount];
        }
    }
}

/// <summary>
/// Maps the permission collection of an operator onto a single column.
/// </summary>
/// <remarks>
/// The column holds the permission names separated by commas. The names are
/// stored rather than the ordinals so that the column stays readable and
/// survives a renumbering of the enumeration.
/// </remarks>
internal sealed class PermissionCollectionConverter : ValueConverter<IReadOnlyCollection<Permission>, string>
{
    private const char Separator = ',';

    /// <summary>Initialises a new converter.</summary>
    public PermissionCollectionConverter()
        : base(
            permissions => string.Join(
                Separator,
                (permissions ?? Array.Empty<Permission>()).Select(permission => permission.ToString())),
            value => Deserialize(value))
    {
    }

    private static IReadOnlyCollection<Permission> Deserialize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<Permission>();
        }

        var permissions = new List<Permission>();

        foreach (var name in value.Split(Separator, StringSplitOptions.RemoveEmptyEntries))
        {
            if (Enum.TryParse<Permission>(name, ignoreCase: true, out var permission))
            {
                permissions.Add(permission);
            }
        }

        return permissions.ToArray();
    }
}

/// <summary>
/// Maps the susceptibility list of a culture result onto a single JSON column.
/// </summary>
internal sealed class SusceptibilityListConverter : ValueConverter<IReadOnlyList<AntibioticSensitivity>, string>
{
    /// <summary>Initialises a new converter.</summary>
    public SusceptibilityListConverter()
        : base(
            susceptibilities => JsonSerializer.Serialize(
                (susceptibilities ?? Array.Empty<AntibioticSensitivity>())
                .Select(susceptibility => new SusceptibilityPayload(
                    susceptibility.AntibioticSymbol,
                    susceptibility.Reading,
                    susceptibility.Unit))
                .ToArray(), JsonDefaults.Options),
            value => Deserialize(value))
    {
    }

    private static IReadOnlyList<AntibioticSensitivity> Deserialize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<AntibioticSensitivity>();
        }

        try
        {
            var payloads = JsonSerializer.Deserialize<SusceptibilityPayload[]>(value, JsonDefaults.Options);

            if (payloads is null)
            {
                return Array.Empty<AntibioticSensitivity>();
            }

            return payloads
                .Select(payload => AntibioticSensitivity.Create(
                    payload.AntibioticSymbol,
                    payload.Reading,
                    payload.Unit))
                .ToArray();
        }
        catch (JsonException)
        {
            return Array.Empty<AntibioticSensitivity>();
        }
    }

    /// <summary>
    /// The wire shape of one susceptibility.
    /// </summary>
    private sealed record SusceptibilityPayload(
        string AntibioticSymbol,
        string Reading,
        string? Unit);
}

/// <summary>
/// Maps the printer map of the laboratory settings onto a single column.
/// </summary>
/// <remarks>
/// The column holds the role and the printer name separated by an equals sign,
/// with the pairs separated by semicolons.
/// </remarks>
internal sealed class PrinterRoleMapConverter : ValueConverter<IReadOnlyDictionary<PrinterRole, string>, string>
{
    private const char PairSeparator = ';';
    private const char KeyValueSeparator = '=';

    /// <summary>Initialises a new converter.</summary>
    public PrinterRoleMapConverter()
        : base(
            printers => string.Join(
                PairSeparator,
                (printers ?? new Dictionary<PrinterRole, string>())
                .OrderBy(pair => (int)pair.Key)
                .Select(pair => $"{pair.Key}{KeyValueSeparator}{pair.Value}")),
            value => Deserialize(value))
    {
    }

    private static IReadOnlyDictionary<PrinterRole, string> Deserialize(string value)
    {
        var printers = new Dictionary<PrinterRole, string>();

        if (string.IsNullOrWhiteSpace(value))
        {
            return printers;
        }

        foreach (var pair in value.Split(PairSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf(KeyValueSeparator, StringComparison.Ordinal);

            if (separator <= 0)
            {
                continue;
            }

            var key = pair[..separator];
            var printer = pair[(separator + 1)..];

            if (Enum.TryParse<PrinterRole>(key, ignoreCase: true, out var role))
            {
                printers[role] = printer;
            }
        }

        return printers;
    }
}

/// <summary>
/// The comparers that make the change tracker see a collection that was
/// replaced rather than mutated in place.
/// </summary>
/// <remarks>
/// The domain replaces a collection instance whenever it changes — see
/// <c>Operator.Grant</c>, <c>CultureResult.AddSusceptibility</c> and
/// <c>PatientHistory.SetSlot</c>. The comparers below compare the contents
/// rather than the identity of the collection, so that the change tracker
/// reports the difference and the column is rewritten.
/// </remarks>
internal static class PersistenceComparers
{

    /// <summary>Compares two lists of nullable strings by content.</summary>
    public static readonly ValueComparer<IReadOnlyList<string?>> NullableStringList =
        new(
            (left, right) => SequenceEqualNullable(left, right),
            value => HashNullableStringList(value),
            value => value.ToArray());

    /// <summary>Compares two lists of strings by content.</summary>
    public static readonly ValueComparer<IReadOnlyList<string>> StringList =
        new(
            (left, right) => SequenceEqualString(left, right),
            value => HashStringList(value),
            value => value.ToArray());

    /// <summary>Compares two arrays of nullable strings by content.</summary>
    public static readonly ValueComparer<string?[]> StringArray =
        new(
            (left, right) => SequenceEqualArray(left, right),
            value => HashArray(value),
            value => value.ToArray());

    /// <summary>Compares two permission collections by content.</summary>
    public static readonly ValueComparer<IReadOnlyCollection<Permission>> PermissionCollection =
        new(
            (left, right) => SequenceEqualPermissions(left, right),
            value => HashPermissions(value),
            value => value.ToArray());

    /// <summary>Compares two susceptibility lists by content.</summary>
    public static readonly ValueComparer<IReadOnlyList<AntibioticSensitivity>> SusceptibilityList =
        new(
            (left, right) => SequenceEqualSusceptibilities(left, right),
            value => HashSusceptibilities(value),
            value => value.ToArray());

    /// <summary>Compares two printer maps by content.</summary>
    public static readonly ValueComparer<IReadOnlyDictionary<PrinterRole, string>> PrinterRoleMap =
        new(
            (left, right) => SequenceEqualPrinters(left, right),
            value => HashPrinters(value),
            value => new Dictionary<PrinterRole, string>(value));

    private static bool SequenceEqualNullable(
        IReadOnlyList<string?>? left,
        IReadOnlyList<string?>? right)
    {
        var first = left ?? Array.Empty<string?>();
        var second = right ?? Array.Empty<string?>();

        return first.Count == second.Count && !first.Where((item, index) => item != second[index]).Any();
    }

    private static bool SequenceEqualString(
        IReadOnlyList<string>? left,
        IReadOnlyList<string>? right)
    {
        var first = left ?? Array.Empty<string>();
        var second = right ?? Array.Empty<string>();

        return first.Count == second.Count && !first.Where((item, index) => item != second[index]).Any();
    }

    private static bool SequenceEqualArray(string?[]? left, string?[]? right)
    {
        var first = left ?? Array.Empty<string?>();
        var second = right ?? Array.Empty<string?>();

        return first.Length == second.Length && !first.Where((item, index) => item != second[index]).Any();
    }

    private static bool SequenceEqualPermissions(
        IReadOnlyCollection<Permission>? left,
        IReadOnlyCollection<Permission>? right)
    {
        var first = (left ?? Array.Empty<Permission>()).OrderBy(permission => permission).ToArray();
        var second = (right ?? Array.Empty<Permission>()).OrderBy(permission => permission).ToArray();

        return first.SequenceEqual(second);
    }

    private static bool SequenceEqualSusceptibilities(
        IReadOnlyList<AntibioticSensitivity>? left,
        IReadOnlyList<AntibioticSensitivity>? right)
    {
        var first = left ?? Array.Empty<AntibioticSensitivity>();
        var second = right ?? Array.Empty<AntibioticSensitivity>();

        if (first.Count != second.Count)
        {
            return false;
        }

        for (var index = 0; index < first.Count; index++)
        {
            if (!string.Equals(first[index].AntibioticSymbol, second[index].AntibioticSymbol, StringComparison.Ordinal) ||
                !string.Equals(first[index].Reading, second[index].Reading, StringComparison.Ordinal) ||
                !string.Equals(first[index].Unit, second[index].Unit, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static int HashNullableStringList(IReadOnlyList<string?> value) =>
        value.Aggregate(17, (hash, item) => HashCode.Combine(hash, item is null ? 0 : StringComparer.Ordinal.GetHashCode(item)));

    private static int HashStringList(IReadOnlyList<string> value) =>
        value.Aggregate(19, (hash, item) => HashCode.Combine(hash, StringComparer.Ordinal.GetHashCode(item)));

    private static int HashArray(string?[] value) =>
        value.Aggregate(23, (hash, item) => HashCode.Combine(hash, item is null ? 0 : StringComparer.Ordinal.GetHashCode(item)));

    private static int HashPermissions(IReadOnlyCollection<Permission> value) =>
        value.Aggregate(29, (hash, permission) => HashCode.Combine(hash, (int)permission));

    private static int HashSusceptibilities(IReadOnlyList<AntibioticSensitivity> value) =>
        value.Aggregate(31, (hash, item) => HashCode.Combine(
            hash,
            StringComparer.Ordinal.GetHashCode(item.AntibioticSymbol),
            StringComparer.Ordinal.GetHashCode(item.Reading),
            item.Unit is null ? 0 : StringComparer.Ordinal.GetHashCode(item.Unit)));

    private static bool SequenceEqualPrinters(
        IReadOnlyDictionary<PrinterRole, string>? left,
        IReadOnlyDictionary<PrinterRole, string>? right)
    {
        var first = left ?? new Dictionary<PrinterRole, string>();
        var second = right ?? new Dictionary<PrinterRole, string>();

        if (first.Count != second.Count)
        {
            return false;
        }

        foreach (var pair in first)
        {
            if (!second.TryGetValue(pair.Key, out var printer) ||
                !string.Equals(printer, pair.Value, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static int HashPrinters(IReadOnlyDictionary<PrinterRole, string> value) =>
        value.Aggregate(37, (hash, pair) => HashCode.Combine(
            hash,
            (int)pair.Key,
            StringComparer.Ordinal.GetHashCode(pair.Value)));
}
