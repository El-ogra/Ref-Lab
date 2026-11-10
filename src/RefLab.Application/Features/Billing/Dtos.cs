using RefLab.Domain.Billing;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Features.Billing;

/// <summary>
/// A payment as presented to the application layer.
/// </summary>
public sealed record PaymentDto(
    Guid Id,
    Guid OrderId,
    string? LegacyOrderIdentifier,
    DateTimeOffset PaidDate,
    int Type,
    string? ReferringPartyCode,
    string? TestSimpleName,
    decimal Amount,
    string CurrencyCode,
    string? TreatingPhysicianCode,
    string? Comment,
    long Number)
{
    /// <summary>Projects a payment onto its DTO.</summary>
    public static PaymentDto FromDomain(Payment payment) =>
        new(
            payment.Id,
            payment.OrderId,
            payment.LegacyOrderIdentifier,
            payment.PaidDate,
            payment.Type,
            payment.ReferringPartyCode,
            payment.TestSimpleName,
            payment.Amount.Amount,
            payment.Amount.CurrencyCode,
            payment.TreatingPhysicianCode,
            payment.Comment,
            payment.Number);
}

/// <summary>
/// A referring party as presented to the application layer.
/// </summary>
public sealed record ReferringPartyDto(
    int Id,
    string Code,
    string Name,
    string? ContactPersonName,
    string? City,
    string? Phone,
    string? Mobile,
    bool IsActive,
    bool IsSpecial,
    decimal CommissionRate,
    string? PriceMethod,
    string? TreatingPhysicianCode)
{
    /// <summary>Projects a referring party onto its DTO.</summary>
    public static ReferringPartyDto FromDomain(ReferringParty party) =>
        new(
            party.Id,
            party.Code,
            party.Name,
            party.ContactPersonName,
            party.City,
            party.Phone,
            party.Mobile,
            party.IsActive,
            party.IsSpecial,
            party.CommissionRate?.Value ?? 0m,
            party.PriceMethod,
            party.TreatingPhysicianCode);
}

/// <summary>
/// The computed commission earned by a referring party.
/// </summary>
public sealed record CommissionDto(
    int ReferringPartyId,
    string ReferringPartyName,
    decimal CommissionRate,
    decimal BaseAmount,
    decimal CommissionAmount,
    string CurrencyCode)
{
    /// <summary>Projects a commission result onto its DTO.</summary>
    public static CommissionDto FromDomain(
        ReferringParty party,
        Percentage rate,
        Money baseAmount,
        Money commission) =>
        new(
            party.Id,
            party.Name,
            rate.Value,
            baseAmount.Amount,
            commission.Amount,
            commission.CurrencyCode);
}

/// <summary>
/// A price list line as presented to the application layer.
/// </summary>
public sealed record PriceListDto(long Id, string PriceListName, string TestSimpleName, decimal Price)
{
    /// <summary>Projects a price list line onto its DTO.</summary>
    public static PriceListDto FromDomain(PriceList priceList) =>
        new(priceList.Id, priceList.PriceListName, priceList.TestSimpleName, priceList.Price);
}

/// <summary>
/// The outcome of a stock-take over a period.
/// </summary>
public sealed record StockTakeDto(
    DateTimeOffset From,
    DateTimeOffset To,
    string? ReferringPartyCode,
    string? TreatingPhysicianCode,
    int OrderCount,
    decimal GrossAmount,
    decimal DiscountedAmount,
    decimal CommissionAmount,
    string CurrencyCode);

/// <summary>
/// A treating physician as presented to the application layer.
/// </summary>
public sealed record TreatingPhysicianDto(int Id, string Code, string Name, string? Phone)
{
    /// <summary>Projects a treating physician onto its DTO.</summary>
    public static TreatingPhysicianDto FromDomain(TreatingPhysician physician) =>
        new(physician.Id, physician.Code, physician.Name, physician.Phone);
}
