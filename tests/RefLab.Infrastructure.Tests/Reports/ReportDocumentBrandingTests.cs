using System.Globalization;
using System.IO.Compression;
using System.Text;
using RefLab.Application.Common;
using RefLab.Domain.Enums;
using RefLab.Infrastructure.Reports;

namespace RefLab.Infrastructure.Tests.Reports;

/// <summary>
/// The laboratory branding travels on the document rather than being baked
/// into the exporter, and the commercial category of a report is carried the
/// same way. These tests prove that the exporter files the document under
/// the category it was given, and that the doctor's signature and the
/// doctor's name reach the printed page.
/// </summary>
public sealed class ReportDocumentBrandingTests
{
    private const string DoctorSignature = "DrSignature";

    private const string DoctorName = "Dr Ahmed Hassan";

    private static ReportDocument BuildDocument(ReportCategory category) =>
        new(
            "LABORATORY REPORT",
            null,
            new[]
            {
                new ReportSection(
                    "Results",
                    new[] { new ReportRow(new[] { "Hgb", "13.4", "g/dL" }) }),
            },
            "header",
            "footer",
            PatientLine: "Patient: Ahmed",
            Category: category,
            DoctorSignature: DoctorSignature,
            DoctorName: DoctorName,
            ThanksText: "Thanks");

    [Fact]
    public async Task The_exported_document_carries_the_category_of_the_document()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-branding-" + Guid.NewGuid().ToString("N"));

        try
        {
            var exported = await exporter.ExportAsync(
                BuildDocument(ReportCategory.Contracts),
                directory);

            Assert.Equal("Contracts", exported.Category);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [Fact]
    public async Task The_produced_pdf_carries_the_doctor_signature_and_the_doctor_name()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-branding-" + Guid.NewGuid().ToString("N"));

        try
        {
            var exported = await exporter.ExportAsync(
                BuildDocument(ReportCategory.Individual),
                directory);

            var bytes = File.ReadAllBytes(exported.FullPath);

            Assert.Equal("%PDF", Encoding.ASCII.GetString(bytes, 0, 4));

            Assert.True(
                PdfText.Contains(bytes, DoctorSignature),
                "The doctor's signature is not printed on the produced document.");

            Assert.True(
                PdfText.Contains(bytes, DoctorName),
                "The doctor's name is not printed on the produced document.");
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [Fact]
    public async Task The_exported_file_is_a_pdf_file()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-branding-" + Guid.NewGuid().ToString("N"));

        try
        {
            var exported = await exporter.ExportAsync(
                BuildDocument(ReportCategory.Free),
                directory);

            Assert.EndsWith(".pdf", exported.FileName, StringComparison.Ordinal);
            Assert.Equal(exported.FileName, Path.GetFileName(exported.FullPath));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}

/// <summary>
/// Recovers the text of a produced PDF.
/// </summary>
/// <remarks>
/// <para>
/// The PDF engine compresses the content streams of a document and writes its
/// text as the glyph codes of an embedded font subset, so the bytes of the
/// file carry no readable text. The text is therefore recovered the way a
/// viewer recovers it: the streams are inflated and the glyph codes are
/// translated through the character map the document carries.
/// </para>
/// </remarks>
internal static class PdfText
{
    /// <summary>The number of characters inspected before a stream.</summary>
    private const int DictionaryWindow = 2000;

    /// <summary>Whether the produced document carries the given text.</summary>
    public static bool Contains(byte[] pdf, string text)
    {
        var streams = ExtractStreams(pdf);
        var maps = BuildCodeMaps(streams);
        var expected = Compact(text);

        foreach (var stream in streams)
        {
            if (!stream.Contains("BT", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (var map in maps)
            {
                if (Compact(DecodeText(stream, map)).Contains(expected, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>Returns the contents of every stream of the document.</summary>
    private static List<string> ExtractStreams(byte[] pdf)
    {
        var latin = Encoding.Latin1.GetString(pdf);
        var streams = new List<string>();
        var index = 0;

        while (true)
        {
            var at = FindStream(latin, index);

            if (at < 0)
            {
                break;
            }

            index = at + 6;

            var start = at + 6;

            if (start < latin.Length && latin[start] == '\r')
            {
                start++;
            }

            if (start < latin.Length && latin[start] == '\n')
            {
                start++;
            }

            var end = latin.IndexOf("endstream", start, StringComparison.Ordinal);

            if (end < 0)
            {
                break;
            }

            var windowStart = Math.Max(0, at - DictionaryWindow);
            var dictionary = latin.Substring(windowStart, at - windowStart);

            var data = new byte[end - start];
            Array.Copy(pdf, start, data, 0, data.Length);

            if (dictionary.Contains("/FlateDecode", StringComparison.Ordinal))
            {
                var inflated = Inflate(data);

                if (inflated is not null)
                {
                    streams.Add(inflated);
                    continue;
                }
            }

            streams.Add(latin.Substring(start, end - start));
        }

        return streams;
    }

    /// <summary>Finds the next stream keyword that is not the end of a stream.</summary>
    private static int FindStream(string document, int from)
    {
        var index = from;

        while (true)
        {
            var at = document.IndexOf("stream", index, StringComparison.Ordinal);

            if (at < 0)
            {
                return -1;
            }

            index = at + 6;

            if (at >= 3 && string.CompareOrdinal(document, at - 3, "end", 0, 3) == 0)
            {
                continue;
            }

            return at;
        }
    }

    /// <summary>Inflates a compressed stream, or returns nothing when it cannot be read.</summary>
    private static string? Inflate(byte[] data)
    {
        try
        {
            using var input = new MemoryStream(data);
            using var zlib = new ZLibStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();

            zlib.CopyTo(output);

            return Encoding.Latin1.GetString(output.ToArray());
        }
        catch (InvalidDataException)
        {
            return null;
        }
    }

    /// <summary>Builds the character map of every font of the document.</summary>
    private static List<Dictionary<int, string>> BuildCodeMaps(List<string> streams)
    {
        var maps = new List<Dictionary<int, string>>();

        foreach (var stream in streams)
        {
            var map = new Dictionary<int, string>();

            ReadBlock(stream, "beginbfchar", "endbfchar", false, map);
            ReadBlock(stream, "beginbfrange", "endbfrange", true, map);

            if (map.Count > 0)
            {
                maps.Add(map);
            }
        }

        return maps;
    }

    /// <summary>Reads the characters of a character map block.</summary>
    private static void ReadBlock(
        string stream,
        string startMarker,
        string endMarker,
        bool isRange,
        Dictionary<int, string> map)
    {
        var index = 0;

        while (true)
        {
            var start = stream.IndexOf(startMarker, index, StringComparison.Ordinal);

            if (start < 0)
            {
                return;
            }

            var end = stream.IndexOf(endMarker, start, StringComparison.Ordinal);

            if (end < 0)
            {
                return;
            }

            var block = stream.Substring(start + startMarker.Length, end - start - startMarker.Length);
            var tokens = ReadHexTokens(block);

            for (var i = 0; i + (isRange ? 2 : 1) < tokens.Count; i += isRange ? 3 : 2)
            {
                if (!isRange)
                {
                    map[tokens[i].Code] = char.ConvertFromUtf32(CodePoint(tokens[i + 1].Hex));
                    continue;
                }

                var low = tokens[i].Code;
                var high = tokens[i + 1].Code;
                var first = CodePoint(tokens[i + 2].Hex);

                for (var code = low; code <= high; code++)
                {
                    map[code] = char.ConvertFromUtf32(first + (code - low));
                }
            }

            index = end;
        }
    }

    /// <summary>Reads the hexadecimal tokens of a character map block.</summary>
    private static List<(int Code, string Hex)> ReadHexTokens(string text)
    {
        var tokens = new List<(int Code, string Hex)>();
        var index = 0;

        while (index < text.Length)
        {
            var character = text[index];

            if (character == '[')
            {
                // An array of destinations is not a range of characters.
                var close = text.IndexOf(']', index);
                index = close < 0 ? text.Length : close + 1;
                continue;
            }

            if (character != '<')
            {
                index++;
                continue;
            }

            var end = text.IndexOf('>', index);

            if (end < 0)
            {
                break;
            }

            var hex = text.Substring(index + 1, end - index - 1);
            index = end + 1;

            if (hex.Length == 0 || hex.Length % 2 != 0 || !hex.All(IsHexDigit))
            {
                continue;
            }

            tokens.Add((CodePoint(hex), hex));
        }

        return tokens;
    }

    /// <summary>Decodes the text of a content stream through a character map.</summary>
    private static string DecodeText(string stream, Dictionary<int, string> map)
    {
        var builder = new StringBuilder();
        var index = 0;

        while (true)
        {
            var open = stream.IndexOf('<', index);

            if (open < 0)
            {
                break;
            }

            var close = stream.IndexOf('>', open);

            if (close < 0)
            {
                break;
            }

            var hex = stream.Substring(open + 1, close - open - 1);
            index = close + 1;

            if (hex.Length == 0 || hex.Length % 4 != 0 || !hex.All(IsHexDigit))
            {
                continue;
            }

            for (var i = 0; i + 4 <= hex.Length; i += 4)
            {
                if (map.TryGetValue(CodePoint(hex.Substring(i, 4)), out var character))
                {
                    builder.Append(character);
                }
            }
        }

        return builder.ToString();
    }

    /// <summary>Returns the value of a hexadecimal number.</summary>
    private static int CodePoint(string hex) =>
        int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

    /// <summary>Whether the character is a hexadecimal digit.</summary>
    private static bool IsHexDigit(char character) =>
        character is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f';

    /// <summary>Removes every space of a text, so that the words of a page compare as one.</summary>
    private static string Compact(string text) =>
        string.Concat(text.Where(character => !char.IsWhiteSpace(character)));
}
