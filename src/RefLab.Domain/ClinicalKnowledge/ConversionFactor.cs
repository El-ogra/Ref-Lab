using RefLab.Domain.Exceptions;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// The factor that converts a measurement from one unit to another for a given
/// analyte.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>converts</c> carries <c>testsimp</c> (text 100),
/// <c>Funit</c> (text 50), <c>Sunit</c> (text 50), <c>Factors</c> (double),
/// <c>Ffix</c> (integer) and <c>Sfix</c> (integer). The shipped dictionary
/// holds 167 rows, for example Albumin g/dL to g/L with a factor of 10.
/// </para>
/// <para>
/// The two <c>fix</c> columns hold the number of decimal places used when the
/// source and the converted value are displayed.
/// </para>
/// </remarks>
public sealed class ConversionFactor
{
    private ConversionFactor()
    {
    }

    /// <summary>The simple name of the analyte the factor applies to.</summary>
    public string TestSimpleName { get; private set; } = string.Empty;

    /// <summary>The unit being converted from.</summary>
    public string FromUnit { get; private set; } = string.Empty;

    /// <summary>The unit being converted to.</summary>
    public string ToUnit { get; private set; } = string.Empty;

    /// <summary>The multiplicative conversion factor.</summary>
    public decimal Factor { get; private set; }

    /// <summary>The number of decimal places of the source unit.</summary>
    public int FromDecimals { get; private set; }

    /// <summary>The number of decimal places of the converted unit.</summary>
    public int ToDecimals { get; private set; }

    /// <summary>Creates a conversion factor.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// A unit name is empty or a decimal count is negative.
    /// </exception>
    public static ConversionFactor Create(
        string testSimpleName,
        string fromUnit,
        string toUnit,
        decimal factor,
        int fromDecimals = 0,
        int toDecimals = 0)
    {
        if (string.IsNullOrWhiteSpace(testSimpleName))
        {
            throw new InvalidTestDefinitionException("A conversion factor must name the analyte it applies to.");
        }

        if (string.IsNullOrWhiteSpace(fromUnit) || string.IsNullOrWhiteSpace(toUnit))
        {
            throw new InvalidTestDefinitionException("A conversion factor must name both units.");
        }

        if (fromDecimals < 0 || toDecimals < 0)
        {
            throw new InvalidTestDefinitionException("A decimal count cannot be negative.");
        }

        return new ConversionFactor
        {
            TestSimpleName = testSimpleName.Trim(),
            FromUnit = fromUnit.Trim(),
            ToUnit = toUnit.Trim(),
            Factor = factor,
            FromDecimals = fromDecimals,
            ToDecimals = toDecimals,
        };
    }

    /// <summary>Updates the factor and the decimal counts.</summary>
    /// <exception cref="InvalidTestDefinitionException">A decimal count is negative.</exception>
    public void UpdateFactor(decimal factor, int fromDecimals, int toDecimals)
    {
        if (fromDecimals < 0 || toDecimals < 0)
        {
            throw new InvalidTestDefinitionException("A decimal count cannot be negative.");
        }

        Factor = factor;
        FromDecimals = fromDecimals;
        ToDecimals = toDecimals;
    }

    /// <summary>Converts a value expressed in <see cref="FromUnit"/> into <see cref="ToUnit"/>.</summary>
    public decimal Convert(decimal value) => value * Factor;
}
