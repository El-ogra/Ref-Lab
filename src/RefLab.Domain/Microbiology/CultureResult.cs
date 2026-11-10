using RefLab.Domain.Exceptions;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Microbiology;

/// <summary>
/// The susceptibility of one organism to one antibiotic.
/// </summary>
/// <remarks>
/// Legacy evidence: the <c>cultureanti</c> table records the antimicrobial
/// susceptibility that belongs to a culture result.
/// </remarks>
public sealed class AntibioticSensitivity
{
    private AntibioticSensitivity()
    {
    }

    /// <summary>The antibiotic symbol.</summary>
    public string AntibioticSymbol { get; private set; } = string.Empty;

    /// <summary>The susceptibility reading, for example <c>Sensitive</c>.</summary>
    public string Reading { get; private set; } = string.Empty;

    /// <summary>The measurement unit of the reading, when the reading is quantified.</summary>
    public string? Unit { get; private set; }

    /// <summary>Creates a susceptibility record.</summary>
    /// <exception cref="InvalidTestDefinitionException">The symbol or reading is empty.</exception>
    public static AntibioticSensitivity Create(string antibioticSymbol, string reading, string? unit = null)
    {
        if (string.IsNullOrWhiteSpace(antibioticSymbol))
        {
            throw new InvalidTestDefinitionException("A susceptibility record must name the antibiotic.");
        }

        if (string.IsNullOrWhiteSpace(reading))
        {
            throw new InvalidTestDefinitionException("A susceptibility record must carry a reading.");
        }

        return new AntibioticSensitivity
        {
            AntibioticSymbol = antibioticSymbol.Trim(),
            Reading = reading.Trim(),
            Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim(),
        };
    }
}

/// <summary>
/// A microbiology culture result recorded against a patient order.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the <c>cultures</c> table carries the culture identifier
/// <c>cultid</c>, the isolated organism <c>organism</c> and <c>organism2</c>,
/// the <c>condition</c>, the colony description <c>colony</c> and the
/// <c>reactions</c>.
/// </para>
/// <para>
/// The sample disposition recorded against the microbiology tables is modelled
/// by <c>RefLab.Domain.SendOut.SampleDisposition</c>.
/// </para>
/// </remarks>
public sealed class CultureResult
{
    private AntibioticSensitivity[] _susceptibilities = Array.Empty<AntibioticSensitivity>();

    private CultureResult()
    {
    }

    /// <summary>The identity of the culture result.</summary>
    public Guid Id { get; private set; }

    /// <summary>The patient the culture belongs to.</summary>
    public PatientId PatientId { get; private set; }

    /// <summary>The order the culture belongs to.</summary>
    public Guid? OrderId { get; private set; }

    /// <summary>The date the sample was taken.</summary>
    public DateTimeOffset RequestDate { get; private set; }

    /// <summary>The laboratory culture identifier.</summary>
    public string CultureId { get; private set; } = string.Empty;

    /// <summary>The isolated organism.</summary>
    public string Organism { get; private set; } = string.Empty;

    /// <summary>The second isolated organism, when two are reported.</summary>
    public string? SecondOrganism { get; private set; }

    /// <summary>The condition of the sample.</summary>
    public string? Condition { get; private set; }

    /// <summary>The colony description.</summary>
    public string? Colony { get; private set; }

    /// <summary>The reactions observed on the culture.</summary>
    public string? Reactions { get; private set; }

    /// <summary>The antimicrobial susceptibilities recorded for the organism.</summary>
    /// <remarks>
    /// The list is replaced — never mutated in place — whenever a
    /// susceptibility is added or removed, so that the persistence layer can
    /// detect the change by comparing the stored value with the current one.
    /// </remarks>
    public IReadOnlyList<AntibioticSensitivity> Susceptibilities
    {
        get => _susceptibilities;
        private set => _susceptibilities = value?.ToArray() ?? Array.Empty<AntibioticSensitivity>();
    }

    /// <summary>Creates a culture result.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The culture identifier or the organism is empty.
    /// </exception>
    public static CultureResult Create(
        PatientId patientId,
        string cultureId,
        string organism,
        DateTimeOffset requestDate,
        Guid? orderId = null,
        string? secondOrganism = null,
        string? condition = null,
        string? colony = null,
        string? reactions = null)
    {
        if (string.IsNullOrWhiteSpace(cultureId))
        {
            throw new InvalidTestDefinitionException("A culture result must have a culture identifier.");
        }

        if (string.IsNullOrWhiteSpace(organism))
        {
            throw new InvalidTestDefinitionException("A culture result must name the isolated organism.");
        }

        return new CultureResult
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            OrderId = orderId,
            RequestDate = requestDate,
            CultureId = cultureId.Trim(),
            Organism = organism.Trim(),
            SecondOrganism = string.IsNullOrWhiteSpace(secondOrganism) ? null : secondOrganism.Trim(),
            Condition = string.IsNullOrWhiteSpace(condition) ? null : condition.Trim(),
            Colony = string.IsNullOrWhiteSpace(colony) ? null : colony.Trim(),
            Reactions = string.IsNullOrWhiteSpace(reactions) ? null : reactions.Trim(),
        };
    }

    /// <summary>Records an antimicrobial susceptibility for the organism.</summary>
    public void AddSusceptibility(AntibioticSensitivity susceptibility)
    {
        ArgumentNullException.ThrowIfNull(susceptibility);

        if (_susceptibilities.Any(existing =>
                string.Equals(existing.AntibioticSymbol, susceptibility.AntibioticSymbol, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidTestDefinitionException(
                $"A susceptibility for '{susceptibility.AntibioticSymbol}' is already recorded on this culture.");
        }

        _susceptibilities = _susceptibilities.Append(susceptibility).ToArray();
    }

    /// <summary>Removes the susceptibility recorded for an antibiotic.</summary>
    public bool RemoveSusceptibility(string antibioticSymbol)
    {
        var remaining = _susceptibilities
            .Where(existing => !string.Equals(
                existing.AntibioticSymbol,
                antibioticSymbol,
                StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var removed = remaining.Length != _susceptibilities.Length;

        _susceptibilities = remaining;

        return removed;
    }
}
