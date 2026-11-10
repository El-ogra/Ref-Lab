using RefLab.Domain.Exceptions;
using RefLab.Domain.Rules;

namespace RefLab.Domain.ClinicalKnowledge;

/// <summary>
/// One calibration point of the prothrombin-time clotting control curve.
/// </summary>
/// <remarks>
/// Legacy evidence: the <c>PTC</c> table carries <c>control</c> (the control
/// time in seconds), <c>concent</c> (the concentration) and <c>ConstID</c>.
/// The shipped table holds 33 points, for example 12 seconds to 100 and
/// 12.5 seconds to 92.
/// </remarks>
public sealed class ProthrombinControlCalibration
{
    private ProthrombinControlCalibration()
    {
    }

    /// <summary>The calibration point identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The control time, in seconds.</summary>
    public decimal ControlTime { get; private set; }

    /// <summary>The concentration the control time corresponds to.</summary>
    public decimal Concentration { get; private set; }

    /// <summary>Creates a calibration point.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The identifier is not positive or the control time is negative.
    /// </exception>
    public static ProthrombinControlCalibration Create(int id, decimal controlTime, decimal concentration)
    {
        if (id <= 0)
        {
            throw new InvalidTestDefinitionException(
                "A prothrombin control calibration identifier must be a positive integer.");
        }

        if (controlTime < 0m)
        {
            throw new InvalidTestDefinitionException("A control time cannot be negative.");
        }

        return new ProthrombinControlCalibration
        {
            Id = id,
            ControlTime = controlTime,
            Concentration = concentration,
        };
    }

    /// <summary>Converts the calibration point into a control-curve point.</summary>
    public ProthrombinControlPoint ToPoint() => new(ControlTime, Concentration);
}
