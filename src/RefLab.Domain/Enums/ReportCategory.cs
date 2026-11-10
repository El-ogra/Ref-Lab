namespace RefLab.Domain.Enums;

/// <summary>
/// The commercial category a printed report is filed under.
/// </summary>
/// <remarks>
/// Legacy evidence: the legacy system files its PDF output under
/// <c>D:\PDF\</c> into exactly one of four directories — <c>Contracts</c>,
/// <c>Free</c>, <c>Individual</c> and <c>LabToLab</c>. The display form of the
/// last one is <c>Lab To Lab</c>. The taxonomy mirrors the laboratory's
/// accounting taxonomy, so the report destination is a commercial
/// classification rather than a functional one.
/// </remarks>
public enum ReportCategory
{
    /// <summary>A contract laboratory.</summary>
    Contracts = 1,

    /// <summary>A free or charity case.</summary>
    Free = 2,

    /// <summary>An individual patient.</summary>
    Individual = 3,

    /// <summary>A laboratory-to-laboratory referral.</summary>
    LabToLab = 4,
}
