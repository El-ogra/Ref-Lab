using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Farming;

/// <summary>
/// A farm that sends samples to the laboratory.
/// </summary>
/// <remarks>
/// Legacy evidence: the <c>sharekat</c> table carries the farm identifier, the
/// farm name, the address, two telephone numbers and a note.
/// </remarks>
public sealed class Farm
{
    private Farm()
    {
    }

    /// <summary>The farm identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The farm name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The farm address.</summary>
    public string? Address { get; private set; }

    /// <summary>The first telephone number of the farm.</summary>
    public string? Phone { get; private set; }

    /// <summary>The second telephone number of the farm.</summary>
    public string? SecondaryPhone { get; private set; }

    /// <summary>The note recorded against the farm.</summary>
    public string? Note { get; private set; }

    /// <summary>Creates a farm.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The identifier is not positive or the name is empty.
    /// </exception>
    public static Farm Create(
        int id,
        string name,
        string? address = null,
        string? phone = null,
        string? secondaryPhone = null,
        string? note = null)
    {
        if (id <= 0)
        {
            throw new InvalidTestDefinitionException("A farm identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTestDefinitionException("A farm must have a name.");
        }

        return new Farm
        {
            Id = id,
            Name = name.Trim(),
            Address = Normalize(address),
            Phone = Normalize(phone),
            SecondaryPhone = Normalize(secondaryPhone),
            Note = Normalize(note),
        };
    }

    /// <summary>Updates the contact details of the farm.</summary>
    public void UpdateContactDetails(string? address, string? phone, string? secondaryPhone, string? note)
    {
        Address = Normalize(address);
        Phone = Normalize(phone);
        SecondaryPhone = Normalize(secondaryPhone);
        Note = Normalize(note);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

/// <summary>
/// A transaction recorded against a farm.
/// </summary>
/// <remarks>
/// Legacy evidence: the <c>sharekathesab</c> table carries the transaction
/// identifier, the farm identifier and name, the transaction date, the
/// <c>wareed</c> (issued) and <c>saader</c> (received) amounts and a note.
/// </remarks>
public sealed class FarmTransaction
{
    private FarmTransaction()
    {
    }

    /// <summary>The transaction identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The farm the transaction belongs to.</summary>
    public int FarmId { get; private set; }

    /// <summary>The farm name as recorded on the transaction.</summary>
    public string FarmName { get; private set; } = string.Empty;

    /// <summary>The date of the transaction.</summary>
    public DateTimeOffset TransactionDate { get; private set; }

    /// <summary>The issued amount.</summary>
    public decimal IssuedAmount { get; private set; }

    /// <summary>The received amount.</summary>
    public decimal ReceivedAmount { get; private set; }

    /// <summary>The note recorded against the transaction.</summary>
    public string? Note { get; private set; }

    /// <summary>Creates a farm transaction.</summary>
    /// <exception cref="InvalidTestDefinitionException">
    /// The identifier is not positive or the farm name is empty.
    /// </exception>
    public static FarmTransaction Create(
        int id,
        int farmId,
        string farmName,
        DateTimeOffset transactionDate,
        decimal issuedAmount = 0m,
        decimal receivedAmount = 0m,
        string? note = null)
    {
        if (id <= 0)
        {
            throw new InvalidTestDefinitionException("A farm transaction identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(farmName))
        {
            throw new InvalidTestDefinitionException("A farm transaction must name the farm.");
        }

        return new FarmTransaction
        {
            Id = id,
            FarmId = farmId,
            FarmName = farmName.Trim(),
            TransactionDate = transactionDate,
            IssuedAmount = issuedAmount,
            ReceivedAmount = receivedAmount,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
        };
    }

    /// <summary>Updates the amounts and the note of the transaction.</summary>
    public void Update(decimal issuedAmount, decimal receivedAmount, string? note)
    {
        IssuedAmount = issuedAmount;
        ReceivedAmount = receivedAmount;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
