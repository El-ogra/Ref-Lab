using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// A test in the laboratory catalogue.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>testtable</c> carries 38 columns and 609 shipped rows.
/// The catalogue is unchanged between the two legacy versions — the clinical
/// knowledge layer of the product did not move in seven years, and Ref Lab
/// carries it forward as data.
/// </para>
/// <para>
/// The test number 4 (the urine analysis) is protected: neither its number nor
/// its name may be changed. The legacy manual states this rule but the legacy
/// code did not enforce it; Ref Lab enforces it in the domain.
/// </para>
/// </remarks>
public sealed class TestDefinition
{
    /// <summary>
    /// The number carried by the urine analysis, which may not be renumbered
    /// or renamed.
    /// </summary>
    public const int UrineAnalysisTestCode = 4;

    private TestDefinition()
    {
    }

    /// <summary>The test code. The primary key of the catalogue.</summary>
    public int Code { get; private set; }

    /// <summary>The billing code shown on invoices.</summary>
    public string? BillingCode { get; private set; }

    /// <summary>The ordering position of the test within its group.</summary>
    public int ArrangementNumber { get; private set; }

    /// <summary>The clinical sub-group the test belongs to.</summary>
    public int SubGroupCode { get; private set; }

    /// <summary>
    /// The simplified name of the analyte. This is the textual key used by the
    /// conversion factors, the abbreviations and the standard comments.
    /// </summary>
    public string SimpleName { get; private set; } = string.Empty;

    /// <summary>The short abbreviation used on the report.</summary>
    public string? Abbreviation { get; private set; }

    /// <summary>The English name of the test.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The Arabic name of the test, when one has been supplied.</summary>
    public string? ArabicName { get; private set; }

    /// <summary>The name used on the work sheet.</summary>
    public string? WorkSheetName { get; private set; }

    /// <summary>The secondary name used on the work sheet.</summary>
    public string? WorkSheetName2 { get; private set; }

    /// <summary>The name used in the patient's medical history.</summary>
    public string? HistoryName { get; private set; }

    /// <summary>The symbol printed on the barcode.</summary>
    public string? BarcodeSymbol { get; private set; }

    /// <summary>The name of the clinical sub-group, denormalised for reporting.</summary>
    public string? SubGroupName { get; private set; }

    /// <summary>The login group the test is filed under.</summary>
    public string? LogGroup { get; private set; }

    /// <summary>Whether the test may be ordered.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Whether the test is performed by another laboratory and is therefore
    /// enrolled on the send-out list automatically.
    /// </summary>
    public bool IsSendOut { get; private set; }

    /// <summary>The standing destination laboratory for a send-out test.</summary>
    public string? OtherLabName { get; private set; }

    /// <summary>The price charged to the external laboratory.</summary>
    public decimal OtherLabPrice { get; private set; }

    /// <summary>The price charged to the patient.</summary>
    public decimal PatientPrice { get; private set; }

    /// <summary>The price charged to a contract laboratory.</summary>
    public decimal LabPrice { get; private set; }

    /// <summary>Whether the operator may attach an open question to the test.</summary>
    public bool AllowsQuestion { get; private set; }

    /// <summary>The question text raised with the operator.</summary>
    public string? Question { get; private set; }

    /// <summary>The clinical description of the test.</summary>
    public string? Description { get; private set; }

    /// <summary>The clinical significance of the test.</summary>
    public string? Effect { get; private set; }

    /// <summary>The usage guidance of the test.</summary>
    public string? Usage { get; private set; }

    /// <summary>The type of sample the test requires.</summary>
    public string? SampleType { get; private set; }

    /// <summary>Whether the test may not be deleted.</summary>
    public bool IsDeleteProtected { get; private set; }

    /// <summary>The turnaround time recorded on the order, in minutes.</summary>
    public int? TestTime { get; private set; }

    /// <summary>Whether the test is printed together with its group.</summary>
    public bool PrintWithGroup { get; private set; }

    /// <summary>The first barcode symbol.</summary>
    public string? Barcode1 { get; private set; }

    /// <summary>The second barcode symbol.</summary>
    public string? Barcode2 { get; private set; }

    /// <summary>The third barcode symbol.</summary>
    public string? Barcode3 { get; private set; }

    /// <summary>Whether the test appears on the report.</summary>
    public string? SeeReport { get; private set; }

    /// <summary>The reference-range type of the test.</summary>
    public string? NormalType { get; private set; }

    /// <summary>Whether the test is added with an order number.</summary>
    public bool AddOrderNumber { get; private set; }

    /// <summary>Whether the test carries a test order number.</summary>
    public bool HasTestOrderNumber { get; private set; }

    /// <summary>The reference-range type code shared with the reference ranges.</summary>
    public string? ReferenceRangeType { get; private set; }

    /// <summary>
    /// The critical limits of the analyte, taken from the method sheet.
    /// </summary>
    public CriticalInterval CriticalInterval { get; private set; } = CriticalInterval.None;

    /// <summary>Creates a test definition.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The code is not positive, or the name or the simplified name is empty.
    /// </exception>
    public static TestDefinition Create(
        int code,
        string name,
        string simpleName,
        int subGroupCode,
        int arrangementNumber = 0)
    {
        if (code <= 0)
        {
            throw new InvalidTestDefinitionException("A test code must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTestDefinitionException("A test must have a name.");
        }

        if (string.IsNullOrWhiteSpace(simpleName))
        {
            throw new InvalidTestDefinitionException("A test must have a simplified analyte name.");
        }

        return new TestDefinition
        {
            Code = code,
            Name = name.Trim(),
            SimpleName = simpleName.Trim(),
            SubGroupCode = subGroupCode,
            ArrangementNumber = arrangementNumber,
            IsActive = true,
            CriticalInterval = CriticalInterval.None,
        };
    }

    /// <summary>
    /// Renames the test.
    /// </summary>
    /// <exception cref="TestNumberImmutableException">
    /// The test is the protected urine analysis.
    /// </exception>
    public void Rename(string newName)
    {
        EnsureNotProtected();

        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new InvalidTestDefinitionException("A test must have a name.");
        }

        Name = newName.Trim();
    }

    /// <summary>
    /// Changes the code of the test.
    /// </summary>
    /// <exception cref="TestNumberImmutableException">
    /// The test is the protected urine analysis.
    /// </exception>
    public void ChangeCode(int newCode)
    {
        EnsureNotProtected();

        if (newCode <= 0)
        {
            throw new InvalidTestDefinitionException("A test code must be a positive integer.");
        }

        Code = newCode;
    }

    /// <summary>Updates the three catalogue prices.</summary>
    public void UpdatePrices(decimal patientPrice, decimal labPrice, decimal otherLabPrice)
    {
        PatientPrice = patientPrice;
        LabPrice = labPrice;
        OtherLabPrice = otherLabPrice;
    }

    /// <summary>Sets the send-out flag and its standing destination.</summary>
    public void ConfigureSendOut(bool isSendOut, string? otherLabName)
    {
        IsSendOut = isSendOut;
        OtherLabName = string.IsNullOrWhiteSpace(otherLabName) ? null : otherLabName.Trim();
    }

    /// <summary>Sets the critical limits of the analyte.</summary>
    public void SetCriticalInterval(CriticalInterval criticalInterval)
    {
        ArgumentNullException.ThrowIfNull(criticalInterval);
        CriticalInterval = criticalInterval;
    }

    /// <summary>
    /// Updates the descriptive attributes of the test.
    /// </summary>
    /// <remarks>
    /// The test number and the test name are deliberately not updated here: the
    /// name is changed by <see cref="Rename"/> and the number by
    /// <see cref="ChangeCode"/>, each of which enforces the protection of the
    /// urine analysis.
    /// </remarks>
    public void UpdateDetails(
        string? simpleName = null,
        string? abbreviation = null,
        string? arabicName = null,
        string? sampleType = null,
        string? description = null,
        string? effect = null,
        string? usage = null)
    {
        if (simpleName is not null)
        {
            if (string.IsNullOrWhiteSpace(simpleName))
            {
                throw new InvalidTestDefinitionException("A test must have a simplified analyte name.");
            }

            SimpleName = simpleName.Trim();
        }

        // A null argument means "not supplied", so the stored value is left
        // untouched. A value is cleared deliberately through its own setter.
        if (abbreviation is not null)
        {
            Abbreviation = string.IsNullOrWhiteSpace(abbreviation) ? null : abbreviation.Trim();
        }

        if (arabicName is not null)
        {
            ArabicName = string.IsNullOrWhiteSpace(arabicName) ? null : arabicName.Trim();
        }

        if (sampleType is not null)
        {
            SampleType = string.IsNullOrWhiteSpace(sampleType) ? null : sampleType.Trim();
        }

        if (description is not null)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        if (effect is not null)
        {
            Effect = string.IsNullOrWhiteSpace(effect) ? null : effect.Trim();
        }

        if (usage is not null)
        {
            Usage = string.IsNullOrWhiteSpace(usage) ? null : usage.Trim();
        }
    }

    /// <summary>Sets the Arabic name of the test.</summary>
    public void SetArabicName(string? arabicName) =>
        ArabicName = string.IsNullOrWhiteSpace(arabicName) ? null : arabicName.Trim();

    /// <summary>Sets the clinical sub-group and the ordering position.</summary>
    public void MoveTo(int subGroupCode, int arrangementNumber)
    {
        SubGroupCode = subGroupCode;
        ArrangementNumber = arrangementNumber;
    }

    /// <summary>Deactivates the test so that it can no longer be ordered.</summary>
    public void Deactivate() => IsActive = false;

    /// <summary>Reactivates the test.</summary>
    public void Activate() => IsActive = true;

    /// <summary>Indicates whether the test is the protected urine analysis.</summary>
    public bool IsUrineAnalysis => Code == UrineAnalysisTestCode;

    private void EnsureNotProtected()
    {
        if (IsUrineAnalysis)
        {
            throw new TestNumberImmutableException(Code);
        }
    }
}
