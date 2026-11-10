using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Billing;

/// <summary>
/// One line of a price list — the price charged for one analyte under one
/// price list.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: <c>pricelist</c> carries <c>pricenum</c> (long),
/// <c>testsimp</c> (text 50), <c>pricelistname</c> (text 50) and
/// <c>testprice</c> (double). The companion table <c>pricemenu</c> carries only
/// <c>pricelistname</c> and is therefore the menu of the distinct price-list
/// names, which Ref Lab derives from this entity rather than storing twice.
/// </para>
/// </remarks>
public sealed class PriceList
{
    private PriceList()
    {
    }

    /// <summary>The price line identifier.</summary>
    public long Id { get; private set; }

    /// <summary>The simplified analyte name the price applies to.</summary>
    public string TestSimpleName { get; private set; } = string.Empty;

    /// <summary>The name of the price list the line belongs to.</summary>
    public string PriceListName { get; private set; } = string.Empty;

    /// <summary>The price charged.</summary>
    public decimal Price { get; private set; }

    /// <summary>Creates a price list line.</summary>
    /// <exception cref="InvalidPaymentException">
    /// The analyte name or the price list name is empty.
    /// </exception>
    public static PriceList Create(long id, string testSimpleName, string priceListName, decimal price)
    {
        if (id <= 0)
        {
            throw new InvalidPaymentException("A price list line identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(testSimpleName))
        {
            throw new InvalidPaymentException("A price list line must name the analyte it prices.");
        }

        if (string.IsNullOrWhiteSpace(priceListName))
        {
            throw new InvalidPaymentException("A price list line must name its price list.");
        }

        return new PriceList
        {
            Id = id,
            TestSimpleName = testSimpleName.Trim(),
            PriceListName = priceListName.Trim(),
            Price = price,
        };
    }

    /// <summary>Changes the price charged.</summary>
    public void SetPrice(decimal price) => Price = price;
}
