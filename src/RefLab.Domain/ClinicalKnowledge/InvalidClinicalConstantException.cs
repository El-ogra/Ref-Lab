namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// Raised when a clinical constant slot does not hold the expected value.
/// </summary>
public sealed class InvalidClinicalConstantException : Exceptions.DomainException
{
    /// <summary>Initialises a new exception.</summary>
    public InvalidClinicalConstantException(string groupName, int slot)
        : base($"Constant slot {slot} of group '{groupName}' does not hold a numeric value.")
    {
        GroupName = groupName;
        Slot = slot;
    }

    /// <summary>The constant group that was read.</summary>
    public string GroupName { get; }

    /// <summary>The slot that was read.</summary>
    public int Slot { get; }
}
