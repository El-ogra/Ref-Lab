using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The enumeration values the presentation layer binds to.
/// </summary>
/// <remarks>
/// The values are exposed as read-only lists so that a combo box can be bound
/// to them with <c>x:Static</c> and so that the display of an enumeration
/// stays a presentation concern.
/// </remarks>
public static class EnumSource
{
    /// <summary>The sex values.</summary>
    public static IReadOnlyList<Sex> SexValues { get; } = Enum.GetValues<Sex>();

    /// <summary>The age units.</summary>
    public static IReadOnlyList<AgeUnit> AgeUnitValues { get; } = Enum.GetValues<AgeUnit>();

    /// <summary>The sex scopes.</summary>
    public static IReadOnlyList<SexScope> SexScopeValues { get; } = Enum.GetValues<SexScope>();

    /// <summary>The report categories.</summary>
    public static IReadOnlyList<ReportCategory> ReportCategoryValues { get; } = Enum.GetValues<ReportCategory>();

    /// <summary>The printer roles.</summary>
    public static IReadOnlyList<PrinterRole> PrinterRoleValues { get; } = Enum.GetValues<PrinterRole>();

    /// <summary>The paper sizes.</summary>
    public static IReadOnlyList<PaperSize> PaperSizeValues { get; } = Enum.GetValues<PaperSize>();

    /// <summary>The sample disposition actions.</summary>
    public static IReadOnlyList<SampleDispositionAction> SampleDispositionActionValues { get; } =
        Enum.GetValues<SampleDispositionAction>();
}
