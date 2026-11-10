using RefLab.Application.Common;

namespace RefLab.Application.Abstractions;

/// <summary>
/// Builds a printable laboratory report from domain data.
/// </summary>
public interface IReportRenderer
{
    /// <summary>Renders the laboratory report for the supplied order.</summary>
    ReportDocument RenderLaboratoryReport(ReportRenderRequest request);
}
