using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// A measurement unit.
/// </summary>
/// <remarks>
/// Legacy evidence: <c>testunits</c> carries <c>unitsid</c> (long) and
/// <c>unitsname</c> (text 15). The shipped dictionary holds 69 units.
/// </remarks>
public sealed class TestUnit
{
    private TestUnit()
    {
    }

    /// <summary>The unit identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The unit name, for example <c>g/dL</c> or <c>U/L</c>.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Creates a measurement unit.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The identifier is not positive or the name is empty.
    /// </exception>
    public static TestUnit Create(int id, string name)
    {
        if (id <= 0)
        {
            throw new InvalidTestDefinitionException("A unit identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTestDefinitionException("A measurement unit must have a name.");
        }

        return new TestUnit { Id = id, Name = name.Trim() };
    }

    /// <summary>Renames the unit.</summary>
    /// <exception cref="InvalidTestDefinitionException">The name is empty.</exception>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTestDefinitionException("A measurement unit must have a name.");
        }

        Name = name.Trim();
    }
}
