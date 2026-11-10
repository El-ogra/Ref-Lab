using RefLab.Domain.Exceptions;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Patients;

public class PatientPictureTests
{
    [Fact]
    public void The_picture_records_its_slot_and_its_path()
    {
        var picture = PatientPicture.Create(
            PatientId.New(),
            3,
            @"C:\pictures\a.png",
            "caption");

        Assert.Equal(3, picture.Slot);
        Assert.Equal(@"C:\pictures\a.png", picture.ImagePath);
        Assert.Equal("caption", picture.Caption);
    }

    [Fact]
    public void A_slot_outside_the_available_range_is_rejected()
    {
        Assert.Throws<InvalidPatientOrderException>(() =>
            PatientPicture.Create(PatientId.New(), 16, @"C:\pictures\a.png"));
    }

    [Fact]
    public void An_empty_image_path_is_rejected()
    {
        Assert.Throws<InvalidPatientOrderException>(() =>
            PatientPicture.Create(PatientId.New(), 1, "  "));
    }
}
