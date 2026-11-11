using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Rules;

/// <summary>
/// Derives the calculated results that the laboratory produces from measured
/// results.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the constants in <c>constanttable</c> match the shipped
/// manual value for value:
/// <list type="bullet">
/// <item><description><c>CBCCONS.constant1 = 3.3</c> — <c>Hct = Hgb x 3.3</c>.</description></item>
/// <item><description><c>CBCCONS.constant2 = 6.25</c> — <c>Hgb% = Hgb x 6.25</c> for a male above 12 years.</description></item>
/// <item><description><c>CBCCONS.constant3 = 6.75</c> — <c>Hgb% = Hgb x 6.75</c> for a female above 12 years.</description></item>
/// <item><description><c>CBCCONS.constant4 = 7.5</c> — <c>Hgb% = Hgb x 7.50</c> for 1 to 12 years.</description></item>
/// <item><description><c>CBCCONS.constant5 = 8.25</c> — <c>Hgb% = Hgb x 8.25</c> below 1 year.</description></item>
/// </list>
/// </para>
/// <para>
/// The factors are resolved from the configured constant group rather than
/// hard-coded, so that a laboratory can correct a factor without a software
/// change. A wrong factor propagates into every report of that discipline with
/// no other indication, which is why the group is validated before use.
/// </para>
/// </remarks>
public static class ClinicalDerivationPolicy
{
    /// <summary>The constant group holding the blood-count derivation factors.</summary>
    public const string CbcGroupName = ClinicalConstant.CbcGroupName;

    /// <summary>The constant group holding the prothrombin-time factors.</summary>
    public const string PtGroupName = ClinicalConstant.PtGroupName;

    /// <summary>The slot holding the haemoglobin to haematocrit factor.</summary>
    public const int HctFactorSlot = 1;

    /// <summary>The slot holding the adult male haemoglobin percentage factor.</summary>
    public const int AdultMaleFactorSlot = 2;

    /// <summary>The slot holding the adult female haemoglobin percentage factor.</summary>
    public const int AdultFemaleFactorSlot = 3;

    /// <summary>The slot holding the 1 to 12 year haemoglobin percentage factor.</summary>
    public const int ChildFactorSlot = 4;

    /// <summary>The slot holding the infant haemoglobin percentage factor.</summary>
    public const int InfantFactorSlot = 5;

    /// <summary>The age below which the infant factor applies, in years.</summary>
    public const decimal InfantAgeLimitYears = 1m;

    /// <summary>The age above which the adult factors apply, in years.</summary>
    public const decimal AdultAgeLimitYears = 12m;

    /// <summary>
    /// Derives the haematocrit from the haemoglobin: <c>Hct = Hgb x factor</c>.
    /// </summary>
    public static decimal DeriveHematocrit(decimal hemoglobin, ClinicalConstant cbcConstants)
    {
        ArgumentNullException.ThrowIfNull(cbcConstants);

        return Round(ResolveFactor(cbcConstants, HctFactorSlot) * hemoglobin);
    }

    /// <summary>
    /// Derives the haemoglobin percentage from the haemoglobin, selecting the
    /// factor from the patient's sex and age.
    /// </summary>
    /// <param name="hemoglobin">The measured haemoglobin.</param>
    /// <param name="sex">The patient's sex.</param>
    /// <param name="age">The patient's age, in years.</param>
    /// <param name="cbcConstants">The configured blood-count constant group.</param>
    public static decimal DeriveHemoglobinPercentage(
        decimal hemoglobin,
        Sex sex,
        decimal age,
        ClinicalConstant cbcConstants)
    {
        ArgumentNullException.ThrowIfNull(cbcConstants);

        var slot = SelectHemoglobinFactorSlot(sex, age);
        return Round(ResolveFactor(cbcConstants, slot) * hemoglobin);
    }

    /// <summary>
    /// Selects the constant slot that holds the haemoglobin percentage factor
    /// for a patient of the given sex and age.
    /// </summary>
    /// <exception cref="InvalidClinicalDerivationException">
    /// The patient is an adult whose sex is not known. The adult factors are
    /// sex-specific, so no factor can be selected.
    /// </exception>
    public static int SelectHemoglobinFactorSlot(Sex sex, decimal age)
    {
        if (age < InfantAgeLimitYears)
        {
            return InfantFactorSlot;
        }

        if (age <= AdultAgeLimitYears)
        {
            return ChildFactorSlot;
        }

        // The adult factors are sex-specific: 6.25 for a male and 6.75 for a
        // female. There is no factor for a patient whose sex was never
        // established, and applying the female factor to an unknown-sex
        // patient would report a wrong number as though it were derived.
        // The derivation is refused instead.
        return sex switch
        {
            Sex.Male => AdultMaleFactorSlot,
            Sex.Female => AdultFemaleFactorSlot,
            _ => throw new InvalidClinicalDerivationException(
                "The haemoglobin percentage cannot be derived for an adult whose sex is not known."),
        };
    }

    /// <summary>
    /// Derives a ratio result from the two measurements it is computed from.
    /// </summary>
    /// <param name="numeratorAnalyte">The analyte the ratio is measured from.</param>
    /// <param name="denominatorAnalyte">The analyte the ratio is measured against.</param>
    /// <param name="numerator">The measured value of the numerator.</param>
    /// <param name="denominator">The measured value of the denominator.</param>
    /// <param name="unit">The unit the ratio is expressed in, when it has one.</param>
    /// <exception cref="Exceptions.InvalidResultValueException">
    /// An analyte name is blank, or the denominator is zero.
    /// </exception>
    public static RatioResult DeriveRatio(
        string numeratorAnalyte,
        string denominatorAnalyte,
        decimal numerator,
        decimal denominator,
        string? unit = null) =>
        RatioResult.Create(numeratorAnalyte, denominatorAnalyte, numerator, denominator, unit);

    /// <summary>
    /// Derives the international sensitivity index.
    /// </summary>
    /// <param name="patientProthrombinTime">The patient's prothrombin time, in seconds.</param>
    /// <param name="controlProthrombinTime">The control prothrombin time, in seconds.</param>
    /// <param name="normalProthrombinTime">The normal control prothrombin time, in seconds.</param>
    /// <param name="controlIsi">The international sensitivity index of the control.</param>
    /// <remarks>
    /// <para>
    /// Legacy evidence: <c>PTCONS.constant3 = 1.33</c> establishes that an
    /// international sensitivity index is computed and that the control carries
    /// a known index. The legacy formula was <em>not recoverable</em> from the
    /// static evidence; this implementation follows the standard laboratory
    /// definition. The behaviour is flagged for owner confirmation.
    /// </para>
    /// </remarks>
    public static decimal DeriveInternationalSensitivityIndex(
        decimal patientProthrombinTime,
        decimal controlProthrombinTime,
        decimal normalProthrombinTime,
        decimal controlIsi)
    {
        var denominator = normalProthrombinTime - controlProthrombinTime;

        if (denominator == 0m)
        {
            throw new InvalidClinicalDerivationException(
                "The normal control and the control prothrombin time are equal, so the index is undefined.");
        }

        var index = (patientProthrombinTime - controlProthrombinTime) / denominator * controlIsi;

        return Round(index);
    }

    /// <summary>
    /// Interpolates the concentration of a clotting control from its control
    /// time using the calibration curve.
    /// </summary>
    /// <param name="calibration">The calibration points, ordered by control time.</param>
    /// <param name="controlTime">The measured control time, in seconds.</param>
    /// <exception cref="InvalidClinicalDerivationException">
    /// The curve is empty, or the control time falls outside the calibrated range.
    /// </exception>
    public static decimal InterpolateControlConcentration(
        IEnumerable<ProthrombinControlPoint> calibration,
        decimal controlTime)
    {
        ArgumentNullException.ThrowIfNull(calibration);

        var points = calibration
            .OrderBy(point => point.ControlTime)
            .ToArray();

        if (points.Length == 0)
        {
            throw new InvalidClinicalDerivationException("The prothrombin control curve is empty.");
        }

        if (points.Length == 1)
        {
            return points[0].Concentration;
        }

        if (controlTime < points[0].ControlTime || controlTime > points[^1].ControlTime)
        {
            throw new InvalidClinicalDerivationException(
                "The control time falls outside the calibrated range of the prothrombin control curve.");
        }

        for (var index = 0; index < points.Length - 1; index++)
        {
            var lower = points[index];
            var upper = points[index + 1];

            if (controlTime >= lower.ControlTime && controlTime <= upper.ControlTime)
            {
                var span = upper.ControlTime - lower.ControlTime;

                if (span == 0m)
                {
                    return lower.Concentration;
                }

                var ratio = (controlTime - lower.ControlTime) / span;

                return Round(lower.Concentration + (ratio * (upper.Concentration - lower.Concentration)));
            }
        }

        return points[^1].Concentration;
    }

    private static decimal ResolveFactor(ClinicalConstant constants, int slot)
    {
        if (!constants.TryGetNumber(slot, out var factor))
        {
            throw new InvalidClinicalConstantException(constants.GroupName, slot);
        }

        if (factor <= 0m)
        {
            throw new InvalidClinicalDerivationException(
                $"Constant slot {slot} of group '{constants.GroupName}' must hold a positive factor.");
        }

        return factor;
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
