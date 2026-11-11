using RefLab.Domain.Enums;

namespace RefLab.Domain.Tests.Enums;

/// <summary>
/// The recorded sex is a closed set. The two legacy values keep the meaning
/// every stored record gave them, and the unknown sex is a recorded fact of
/// its own rather than a missing value.
/// </summary>
public class SexTests
{
    [Fact]
    public void Male_keeps_the_value_it_has_always_had()
    {
        Assert.Equal(1, (int)Sex.Male);
    }

    [Fact]
    public void Female_keeps_the_value_it_has_always_had()
    {
        Assert.Equal(2, (int)Sex.Female);
    }

    [Fact]
    public void The_unknown_sex_has_a_value_of_its_own()
    {
        Assert.Equal(3, (int)Sex.Unknown);
    }

    [Fact]
    public void The_enum_is_closed()
    {
        var values = Enum.GetValues<Sex>();

        Assert.Equal(3, values.Length);
        Assert.Contains(Sex.Male, values);
        Assert.Contains(Sex.Female, values);
        Assert.Contains(Sex.Unknown, values);
    }
}
