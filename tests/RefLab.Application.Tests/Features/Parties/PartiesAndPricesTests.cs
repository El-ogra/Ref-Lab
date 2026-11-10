using RefLab.Application.Common;
using RefLab.Application.Features.Parties;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Billing;

namespace RefLab.Application.Tests.Features.Parties;

public class ManageTreatingPhysiciansTests
{
    [Fact]
    public async Task The_physician_is_added_to_the_register()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ManageTreatingPhysiciansCommand(7, "PW-9", "Dr Name", "0100000000"),
            CancellationToken.None);

        Assert.Equal("PW-9", result.Code);
        Assert.Equal("0100000000", result.Phone);
        Assert.Single(host.Db.Items<TreatingPhysician>());
    }

    [Fact]
    public async Task The_physician_is_amended_when_it_is_already_known()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.Physician());

        var result = await host.Sender.Send(
            new ManageTreatingPhysiciansCommand(7, "PW-9", "Dr Name", "0111111111"),
            CancellationToken.None);

        Assert.Equal("0111111111", result.Phone);
        Assert.Single(host.Db.Items<TreatingPhysician>());
    }

    [Fact]
    public async Task An_empty_code_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new ManageTreatingPhysiciansCommand(7, "  ", "Dr Name"), CancellationToken.None));
    }
}

public class PriceListTests
{
    [Fact]
    public async Task The_price_is_added_to_the_price_list()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ManagePriceListsCommand(1, "Standard", "CBC", 175m),
            CancellationToken.None);

        Assert.Equal(175m, result.Price);
        Assert.Equal("Standard", result.PriceListName);
        Assert.Single(host.Db.Items<PriceList>());
    }

    [Fact]
    public async Task The_price_is_amended_when_the_line_is_already_known()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreatePriceList());

        var result = await host.Sender.Send(
            new ManagePriceListsCommand(1, "Standard", "CBC", 200m),
            CancellationToken.None);

        Assert.Equal(200m, result.Price);
        Assert.Single(host.Db.Items<PriceList>());
    }

    [Fact]
    public async Task A_negative_price_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new ManagePriceListsCommand(1, "Standard", "CBC", -1m), CancellationToken.None));
    }

    [Fact]
    public async Task The_price_list_is_printed()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreatePriceList(listName: "Standard", analyte: "CBC", price: 150m));

        var result = await host.Sender.Send(new PrintPriceListCommand("Standard", @"D:\out"), CancellationToken.None);

        Assert.EndsWith(".pdf", result.FileName, StringComparison.Ordinal);
        Assert.Equal(1, host.Pdf.CallCount);
    }

    [Fact]
    public async Task An_unknown_price_list_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new PrintPriceListCommand("Missing", @"D:\out"), CancellationToken.None));
    }
}
