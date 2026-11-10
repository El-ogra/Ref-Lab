namespace RefLab.Domain.Rules;

/// <summary>
/// One calibration point of the prothrombin-time clotting control curve.
/// </summary>
/// <remarks>
/// Legacy evidence: the <c>PTC</c> table carries <c>control</c> (the control
/// time in seconds), <c>concent</c> (the concentration) and <c>ConstID</c>.
/// The shipped table holds 33 points, for example 12 seconds to 100 and 12.5
/// seconds to 92.
/// </remarks>
public readonly record struct ProthrombinControlPoint(decimal ControlTime, decimal Concentration);
