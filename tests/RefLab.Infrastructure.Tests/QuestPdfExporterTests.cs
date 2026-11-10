using RefLab.Application.Common;
using RefLab.Infrastructure.Reports;

namespace RefLab.Infrastructure.Tests;

/// <summary>
/// The approved PDF library is QuestPDF. These tests prove that the
/// <see cref="Application.Abstractions.IPdfExporter"/> abstraction is
/// compatible with it and that a real PDF is produced.
/// </summary>
public class QuestPdfExporterTests
{
    private static ReportDocument BuildDocument() =>
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
            "footer");

    [Fact]
    public async Task A_real_pdf_file_is_produced()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-pdf-" + Guid.NewGuid().ToString("N"));

        try
        {
            var exported = await exporter.ExportAsync(BuildDocument(), directory);

            Assert.True(File.Exists(exported.FullPath));

            var bytes = await File.ReadAllBytesAsync(exported.FullPath);

            Assert.True(bytes.Length > 100, "The produced file is too small to be a PDF.");
            Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(bytes, 0, 4));
            Assert.EndsWith(".pdf", exported.FileName, StringComparison.Ordinal);
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
    public async Task The_destination_directory_is_created_when_it_is_missing()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-pdf-" + Guid.NewGuid().ToString("N"), "nested");

        try
        {
            var exported = await exporter.ExportAsync(BuildDocument(), directory);

            Assert.True(Directory.Exists(directory));
            Assert.True(File.Exists(exported.FullPath));
        }
        finally
        {
            var parent = Directory.GetParent(directory)?.FullName;

            if (parent is not null && Directory.Exists(parent))
            {
                Directory.Delete(parent, recursive: true);
            }
        }
    }

    [Fact]
    public async Task The_exported_bytes_are_the_bytes_that_were_written()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-pdf-" + Guid.NewGuid().ToString("N"));

        try
        {
            var exported = await exporter.ExportAsync(BuildDocument(), directory);

            var onDisk = await File.ReadAllBytesAsync(exported.FullPath);

            Assert.Equal(onDisk, exported.Content);
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
    public async Task A_report_with_several_sections_and_rows_is_produced()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-pdf-" + Guid.NewGuid().ToString("N"));

        var document = new ReportDocument(
            "LABORATORY REPORT",
            "subtitle",
            new[]
            {
                new ReportSection(
                    "Haematology",
                    new[]
                    {
                        new ReportRow(new[] { "Hgb", "13.4", "g/dL" }),
                        new ReportRow(new[] { "WBC", "7.1", "10^9/L" }),
                    }),
                new ReportSection(
                    "Chemistry",
                    new[]
                    {
                        new ReportRow(new[] { "Glucose", "95", "mg/dL" }),
                    }),
            },
            "header",
            "footer");

        try
        {
            var exported = await exporter.ExportAsync(document, directory);

            Assert.True(File.Exists(exported.FullPath));
            Assert.True(exported.Content.Length > 100);
            Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(exported.Content, 0, 4));
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
    public async Task A_report_without_a_section_is_produced()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-pdf-" + Guid.NewGuid().ToString("N"));

        var document = new ReportDocument(
            "EMPTY REPORT",
            null,
            Array.Empty<ReportSection>(),
            null,
            null);

        try
        {
            var exported = await exporter.ExportAsync(document, directory);

            Assert.True(File.Exists(exported.FullPath));
            Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(exported.Content, 0, 4));
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
    public async Task A_row_of_a_different_width_does_not_break_the_table()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-pdf-" + Guid.NewGuid().ToString("N"));

        var document = new ReportDocument(
            "LABORATORY REPORT",
            null,
            new[]
            {
                new ReportSection(
                    "Results",
                    new[]
                    {
                        new ReportRow(new[] { "Hgb", "13.4", "g/dL" }),
                        new ReportRow(new[] { "comment" }),
                    }),
            },
            null,
            null);

        try
        {
            var exported = await exporter.ExportAsync(document, directory);

            Assert.True(File.Exists(exported.FullPath));
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
    public async Task The_file_name_is_derived_from_the_title()
    {
        var exporter = new QuestPdfExporter();
        var directory = Path.Combine(Path.GetTempPath(), "reflab-pdf-" + Guid.NewGuid().ToString("N"));

        try
        {
            var exported = await exporter.ExportAsync(BuildDocument(), directory);

            Assert.StartsWith("LABORATORY_REPORT_", exported.FileName, StringComparison.Ordinal);
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

    [Fact]
    public async Task An_empty_destination_directory_is_rejected()
    {
        var exporter = new QuestPdfExporter();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            exporter.ExportAsync(BuildDocument(), "  "));
    }
}

public class QuestPdfReportRendererTests
{
    [Fact]
    public void The_report_is_rendered_with_its_sections()
    {
        var renderer = new QuestPdfReportRenderer();

        var document = renderer.RenderLaboratoryReport(new ReportRenderRequest(
            "LABORATORY REPORT",
            "subtitle",
            "patient",
            "AB1234567890X",
            "Male",
            "30",
            "2026-03-01",
            "2026-03-01",
            "header",
            "footer",
            new[] { new ReportSection("Results", new[] { new ReportRow(new[] { "Hgb", "13.4" }) }) }));

        Assert.Equal("LABORATORY REPORT", document.Title);
        Assert.Equal("subtitle", document.Subtitle);
        Assert.Equal("header", document.HeaderText);
        Assert.Single(document.Sections);
        Assert.Single(document.Sections[0].Rows);
    }
}
