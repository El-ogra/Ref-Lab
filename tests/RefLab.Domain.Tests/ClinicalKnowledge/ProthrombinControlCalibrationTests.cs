using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Tests.ClinicalKnowledge;

public class ProthrombinControlCalibrationTests
{
    [Fact]
    public void The_calibration_point_converts_into_a_control_curve_point()
    {
        var calibration = ProthrombinControlCalibration.Create(1, 12m, 100m);

        var point = calibration.ToPoint();

        Assert.Equal(12m, point.ControlTime);
        Assert.Equal(100m, point.Concentration);
    }

    [Fact]
    public void A_negative_control_time_is_rejected()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            ProthrombinControlCalibration.Create(1, -1m, 100m));
    }
}
