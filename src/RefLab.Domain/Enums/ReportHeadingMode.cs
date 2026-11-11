namespace RefLab.Domain.Enums;

/// <summary>
/// Which results a printed laboratory report includes.
/// </summary>
/// <remarks>
/// <para>
/// The reference system's report preparation screen offers the report heading
/// in three forms: every result, the normal results only, and the abnormal
/// results only. The choice is made when the report is prepared and applies to
/// the document that is produced.
/// </para>
/// <para>
/// The values are ordered so that the default — every result — is the zero
/// value and therefore the value an existing laboratory record already holds.
/// </para>
/// </remarks>
public enum ReportHeadingMode
{
    /// <summary>The report carries every result of the selected tests.</summary>
    All = 0,

    /// <summary>The report carries the results that lie inside their reference interval.</summary>
    Normal = 1,

    /// <summary>The report carries the results that lie outside their reference interval.</summary>
    Abnormal = 2,
}
