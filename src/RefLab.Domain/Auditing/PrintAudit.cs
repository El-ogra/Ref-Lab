using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Auditing;

/// <summary>
/// The audit record of a single printed report.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: every print records <c>resultprint</c> (flag),
/// <c>printuser</c> (text 40), <c>printdate</c> (text 40) and
/// <c>printnum</c> (double) on the test row. The sequence origin is the
/// absolute value <c>SETTING.ini [setup] reportstart=1984</c>.
/// </para>
/// <para>
/// The sequence number is a confidentiality mechanism as much as an identifier:
/// a missing or duplicated number reveals a missing report.
/// </para>
/// </remarks>
public sealed class PrintAudit
{
    private PrintAudit()
    {
    }

    /// <summary>The identity of the audit record.</summary>
    public Guid Id { get; private set; }

    /// <summary>The order test the report was produced for.</summary>
    public Guid OrderTestId { get; private set; }

    /// <summary>The operator who printed the report.</summary>
    public string Operator { get; private set; } = string.Empty;

    /// <summary>The moment the report was printed.</summary>
    public DateTimeOffset PrintedAt { get; private set; }

    /// <summary>The report sequence number.</summary>
    public long SequenceNumber { get; private set; }

    /// <summary>The commercial category the report was filed under.</summary>
    public ReportCategory Category { get; private set; }

    /// <summary>Creates a print audit record.</summary>
    /// <exception cref="InvalidPatientOrderException">
    /// The order test identity is empty or the operator is not named.
    /// </exception>
    public static PrintAudit Create(
        Guid orderTestId,
        string @operator,
        DateTimeOffset printedAt,
        long sequenceNumber,
        ReportCategory category = ReportCategory.Individual)
    {
        if (orderTestId == Guid.Empty)
        {
            throw new InvalidPatientOrderException("A print audit record must reference an order test.");
        }

        if (string.IsNullOrWhiteSpace(@operator))
        {
            throw new InvalidPatientOrderException("A print audit record must name the operator who printed.");
        }

        return new PrintAudit
        {
            Id = Guid.NewGuid(),
            OrderTestId = orderTestId,
            Operator = @operator.Trim(),
            PrintedAt = printedAt,
            SequenceNumber = sequenceNumber,
            Category = category,
        };
    }
}
