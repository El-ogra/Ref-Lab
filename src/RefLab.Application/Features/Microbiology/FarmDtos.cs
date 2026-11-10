using RefLab.Domain.Farming;

namespace RefLab.Application.Features.Microbiology;

/// <summary>
/// A farm as presented to the application layer.
/// </summary>
public sealed record FarmDto(int Id, string Name, string? Address, string? Phone, string? SecondaryPhone, string? Note)
{
    /// <summary>Projects a farm onto its DTO.</summary>
    public static FarmDto FromDomain(Farm farm) =>
        new(farm.Id, farm.Name, farm.Address, farm.Phone, farm.SecondaryPhone, farm.Note);
}

/// <summary>
/// A farm transaction as presented to the application layer.
/// </summary>
public sealed record FarmTransactionDto(
    int Id,
    int FarmId,
    string FarmName,
    DateTimeOffset TransactionDate,
    decimal IssuedAmount,
    decimal ReceivedAmount,
    string? Note)
{
    /// <summary>Projects a farm transaction onto its DTO.</summary>
    public static FarmTransactionDto FromDomain(FarmTransaction transaction) =>
        new(
            transaction.Id,
            transaction.FarmId,
            transaction.FarmName,
            transaction.TransactionDate,
            transaction.IssuedAmount,
            transaction.ReceivedAmount,
            transaction.Note);
}
