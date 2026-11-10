namespace RefLab.Domain.Identity;

/// <summary>
/// A function-level permission granted to an operator.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the operator table <c>patients</c> carries 17 boolean
/// columns <c>grant1</c> to <c>grant17</c>. The mapping of any individual bit to
/// a specific capability was <em>never established</em> — the bits are numeric
/// only.
/// </para>
/// <para>
/// Ref Lab replaces the anonymous bits with named capabilities, so that a
/// permission can be granted, audited and reported on. The names below are
/// Ref Lab's own design and are aligned with the capabilities documented in the
/// legacy user manual and vendor guide.
/// </para>
/// <para>
/// The legacy operator table carries seventeen bits. Ref Lab defines fifteen:
/// the seventeenth legacy bit governed the digital delivery of results, which
/// was permanently removed from scope with SMS and NATIGH.COM, and the
/// sixteenth governed the multi-branch features, which the laboratory has
/// dropped in favour of a single-site installation.
/// </para>
/// </remarks>
public enum Permission
{
    /// <summary>Register a new patient.</summary>
    RegisterPatient = 1,

    /// <summary>Amend an existing patient record.</summary>
    EditPatient = 2,

    /// <summary>Search for a patient and open the patient history.</summary>
    SearchPatients = 3,

    /// <summary>Order tests onto a patient order.</summary>
    OrderTests = 4,

    /// <summary>Enter a result value.</summary>
    EnterResults = 5,

    /// <summary>Confirm a result as reviewed.</summary>
    ReviewResults = 6,

    /// <summary>Print a laboratory report.</summary>
    PrintReports = 7,

    /// <summary>Hand over results and settle the account.</summary>
    HandOverResults = 8,

    /// <summary>Maintain the test catalogue.</summary>
    ManageTestCatalogue = 9,

    /// <summary>Maintain the reference ranges.</summary>
    ManageReferenceRanges = 10,

    /// <summary>Maintain prices and price lists.</summary>
    ManagePricing = 11,

    /// <summary>Maintain operators, roles and permissions.</summary>
    ManageOperators = 12,

    /// <summary>Run the statistics and performance module.</summary>
    ViewStatistics = 13,

    /// <summary>Change the laboratory settings, printers and report layout.</summary>
    ManageSettings = 14,

    /// <summary>Run the accounting, stock-take and send-out settlement.</summary>
    ManageAccounting = 15,
}
