using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.ValueObjects;

/// <summary>
/// The order identifier is a positional composite. The laboratory is single-site,
/// so the ninth position no longer carries a branch code.
/// </summary>
public class OrderIdentifierTests
{
    [Theory]
    [InlineData(12)]
    [InlineData(14)]
    [InlineData(0)]
    public void Create_rejects_an_identifier_of_the_wrong_length(int length)
    {
        var value = new string('0', length);

        var exception = Assert.Throws<InvalidOrderIdentifierException>(() => OrderIdentifier.Create(value));

        Assert.Equal(length, exception.ActualLength);
        Assert.Equal(OrderIdentifier.Length, exception.ExpectedLength);
    }

    [Fact]
    public void Create_accepts_an_identifier_of_exactly_thirteen_characters()
    {
        var identifier = OrderIdentifier.Create("AB1234567890X");

        Assert.Equal(13, identifier.Value.Length);
        Assert.Equal("AB", identifier.LeadingSegment);
        Assert.Equal("12", identifier.PersonKey);
        Assert.Equal("34567", identifier.MiddleSegment);
        Assert.Equal("890", identifier.DisplayCode);
        Assert.Equal('X', identifier.TrailingCharacter);
    }

    [Fact]
    public void The_person_key_is_read_from_positions_three_and_four()
    {
        var identifier = OrderIdentifier.Create("AB1234567890X");

        Assert.Equal(identifier.Value.Substring(2, 2), identifier.PersonKey);
    }

    [Fact]
    public void The_display_code_is_read_from_positions_ten_to_twelve()
    {
        var identifier = OrderIdentifier.Create("AB1234567890X");

        Assert.Equal(identifier.Value.Substring(9, 3), identifier.DisplayCode);
    }

    [Fact]
    public void The_middle_segment_is_read_from_positions_five_to_nine()
    {
        var identifier = OrderIdentifier.Create("AB1234567890X");

        Assert.Equal("34567", identifier.MiddleSegment);
    }

    [Fact]
    public void FromSegments_rebuilds_the_identifier_in_the_documented_layout()
    {
        var identifier = OrderIdentifier.FromSegments("AB", "12", "34567", "890", 'X');

        Assert.Equal("AB1234567890X", identifier.Value);
        Assert.Equal('X', identifier.TrailingCharacter);
    }

    [Fact]
    public void The_segments_account_for_all_thirteen_positions()
    {
        var identifier = OrderIdentifier.FromSegments("AB", "12", "34567", "890", 'X');

        Assert.Equal(2, identifier.LeadingSegment.Length);
        Assert.Equal(2, identifier.PersonKey.Length);
        Assert.Equal(5, identifier.MiddleSegment.Length);
        Assert.Equal(3, identifier.DisplayCode.Length);
        Assert.Equal(1, identifier.TrailingCharacter.ToString().Length);
        Assert.Equal(OrderIdentifier.Length, identifier.Value.Length);
    }

    [Theory]
    [InlineData("A", "12", "34567", "890")]
    [InlineData("AB", "1", "34567", "890")]
    [InlineData("AB", "12", "3456", "890")]
    [InlineData("AB", "12", "34567", "89")]
    public void FromSegments_rejects_a_segment_of_the_wrong_length(
        string leading, string personKey, string middle, string display)
    {
        Assert.Throws<InvalidOrderIdentifierException>(() =>
            OrderIdentifier.FromSegments(leading, personKey, middle, display, 'X'));
    }

    [Fact]
    public void Two_identifiers_with_the_same_value_are_equal()
    {
        var left = OrderIdentifier.Create("AB1234567890X");
        var right = OrderIdentifier.Create("AB1234567890X");

        Assert.Equal(left, right);
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Two_identifiers_with_different_values_are_not_equal()
    {
        var left = OrderIdentifier.Create("AB1234567890X");
        var right = OrderIdentifier.Create("AB1234567891X");

        Assert.NotEqual(left, right);
        Assert.True(left != right);
    }
}
