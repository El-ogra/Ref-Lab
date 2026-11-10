using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// An abbreviation used on laboratory reports, together with its meaning.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>abbreviations</c> carries <c>abbid</c> (long),
/// <c>abbre</c> (text 30) and <c>mean</c> (text 100). The shipped dictionary
/// holds 650 entries and is unchanged between the two legacy versions.
/// </remarks>
public sealed class Abbreviation
{
    private Abbreviation()
    {
    }

    /// <summary>The abbreviation identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The abbreviated text.</summary>
    public string Text { get; private set; } = string.Empty;

    /// <summary>The expansion of the abbreviation.</summary>
    public string Meaning { get; private set; } = string.Empty;

    /// <summary>Creates an abbreviation.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The identifier is not positive or the abbreviation is empty.
    /// </exception>
    public static Abbreviation Create(int id, string abbreviation, string meaning)
    {
        if (id <= 0)
        {
            throw new InvalidTestDefinitionException("An abbreviation identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(abbreviation))
        {
            throw new InvalidTestDefinitionException("An abbreviation cannot be empty.");
        }

        return new Abbreviation
        {
            Id = id,
            Text = abbreviation.Trim(),
            Meaning = (meaning ?? string.Empty).Trim(),
        };
    }

    /// <summary>Updates the expansion of the abbreviation.</summary>
    public void UpdateMeaning(string meaning) =>
        Meaning = (meaning ?? string.Empty).Trim();
}
