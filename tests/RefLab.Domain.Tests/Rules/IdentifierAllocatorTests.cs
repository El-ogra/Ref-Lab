using RefLab.Domain.Exceptions;
using RefLab.Domain.Rules;

namespace RefLab.Domain.Tests.Rules;

/// <summary>
/// The order identifier layout is a hard data contract. It is 13 characters
/// long; its ninth position carried the branch code in the legacy multi-branch
/// product and is no longer used.
/// </summary>
public class IdentifierAllocatorTests
{
    [Fact]
    public void The_identifier_is_thirteen_characters_long()
    {
        var identifier = IdentifierAllocator.Allocate("AB", "12", "34567", "890", 'X');

        Assert.Equal(13, identifier.Value.Length);
    }

    [Fact]
    public void The_person_key_occupies_positions_three_and_four()
    {
        var identifier = IdentifierAllocator.Allocate("AB", "12", "34567", "890", 'X');

        Assert.Equal("12", identifier.PersonKey);
        Assert.Equal(identifier.Value.Substring(2, 2), identifier.PersonKey);
    }

    [Fact]
    public void The_display_code_occupies_positions_ten_to_twelve()
    {
        var identifier = IdentifierAllocator.Allocate("AB", "12", "34567", "890", 'X');

        Assert.Equal("890", identifier.DisplayCode);
        Assert.Equal(identifier.Value.Substring(9, 3), identifier.DisplayCode);
    }

    [Fact]
    public void The_middle_segment_occupies_positions_five_to_nine()
    {
        var identifier = IdentifierAllocator.Allocate("AB", "12", "34567", "890", 'X');

        Assert.Equal("34567", identifier.MiddleSegment);
    }

    [Fact]
    public void The_trailing_character_occupies_position_thirteen()
    {
        var identifier = IdentifierAllocator.Allocate("AB", "12", "34567", "890", 'X');

        Assert.Equal('X', identifier.TrailingCharacter);
    }

    [Theory]
    [InlineData("A", "12", "34567", "890")]
    [InlineData("AB", "1", "34567", "890")]
    [InlineData("AB", "12", "3456", "890")]
    [InlineData("AB", "12", "34567", "89")]
    public void A_segment_of_the_wrong_length_is_rejected(
        string leading, string personKey, string middle, string display)
    {
        Assert.Throws<InvalidOrderIdentifierException>(() =>
            IdentifierAllocator.Allocate(leading, personKey, middle, display, 'X'));
    }
}
