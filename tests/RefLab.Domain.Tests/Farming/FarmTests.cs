using RefLab.Domain.Exceptions;
using RefLab.Domain.Farming;

namespace RefLab.Domain.Tests.Farming;

public class FarmTests
{
    [Fact]
    public void The_farm_records_its_contact_details()
    {
        var farm = Farm.Create(1, "Farm One", "Cairo", "0100000000", "0111111111", "note");

        Assert.Equal("Farm One", farm.Name);
        Assert.Equal("Cairo", farm.Address);
        Assert.Equal("0100000000", farm.Phone);
    }

    [Fact]
    public void An_empty_farm_name_is_rejected()
    {
        Assert.Throws<InvalidTestDefinitionException>(() => Farm.Create(1, "  "));
    }

    [Fact]
    public void The_transaction_records_the_issued_and_the_received_amounts()
    {
        var transaction = FarmTransaction.Create(1, 5, "Farm One", DateTimeOffset.UtcNow, 100m, 80m, "note");

        Assert.Equal(100m, transaction.IssuedAmount);
        Assert.Equal(80m, transaction.ReceivedAmount);

        transaction.Update(120m, 90m, null);

        Assert.Equal(120m, transaction.IssuedAmount);
        Assert.Null(transaction.Note);
    }

    [Fact]
    public void An_empty_farm_name_is_rejected_on_a_transaction()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            FarmTransaction.Create(1, 5, "  ", DateTimeOffset.UtcNow));
    }
}
