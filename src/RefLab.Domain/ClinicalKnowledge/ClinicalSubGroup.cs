using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// A clinical sub-group — the discipline a test belongs to, such as
/// <c>BLOOD GLUCOSE</c> or <c>LIVER FUNCTIONS</c>.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>subgrouptable</c> carries <c>subgroupcode</c> (long),
/// <c>subgroupname</c> (text 50), <c>arname</c> (text 50) and
/// <c>deleteprotect</c> (boolean). The 46 shipped rows all carry an empty
/// <c>arname</c>, so the Arabic name is optional here.
/// </remarks>
public sealed class ClinicalSubGroup
{
    private ClinicalSubGroup()
    {
    }

    /// <summary>The sub-group code.</summary>
    public int Code { get; private set; }

    /// <summary>The English sub-group name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The Arabic sub-group name, when one has been supplied.</summary>
    public string? ArabicName { get; private set; }

    /// <summary>Whether the sub-group may not be deleted.</summary>
    public bool IsDeleteProtected { get; private set; }

    /// <summary>Creates a clinical sub-group.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The code is not positive or the name is empty.
    /// </exception>
    public static ClinicalSubGroup Create(int code, string name, string? arabicName = null, bool isDeleteProtected = false)
    {
        if (code <= 0)
        {
            throw new InvalidTestDefinitionException("A clinical sub-group code must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTestDefinitionException("A clinical sub-group must have a name.");
        }

        return new ClinicalSubGroup
        {
            Code = code,
            Name = name.Trim(),
            ArabicName = string.IsNullOrWhiteSpace(arabicName) ? null : arabicName.Trim(),
            IsDeleteProtected = isDeleteProtected,
        };
    }

    /// <summary>Renames the sub-group.</summary>
    /// <exception cref="InvalidTestDefinitionException">The name is empty.</exception>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTestDefinitionException("A clinical sub-group must have a name.");
        }

        Name = name.Trim();
    }

    /// <summary>Sets the Arabic name of the sub-group.</summary>
    public void UpdateArabicName(string? arabicName) =>
        ArabicName = string.IsNullOrWhiteSpace(arabicName) ? null : arabicName.Trim();
}
