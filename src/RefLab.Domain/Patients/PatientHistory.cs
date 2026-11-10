using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Patients;

/// <summary>
/// The free-form medical history sheet carried by a patient.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the <c>history</c> table carries <c>selector</c> (integer),
/// <c>labid</c> (text 50), <c>requestdate</c>, twenty free-text columns
/// <c>column1</c> to <c>column20</c>, the <c>printflag</c> boolean and
/// <c>noofcol</c> (the number of columns actually used).
/// </para>
/// <para>
/// The slots are deliberately untyped: the legacy laboratory defined its own
/// meaning for each of the twenty positions and that meaning was never
/// documented.
/// </para>
/// </remarks>
public sealed class PatientHistory
{
    /// <summary>The number of free-form slots the sheet provides.</summary>
    public const int SlotCount = 20;

    private string?[] _slots = new string?[SlotCount];

    private PatientHistory()
    {
    }

    /// <summary>The free-form slots of the sheet.</summary>
    /// <remarks>
    /// The array is replaced — never mutated in place — whenever a slot is
    /// written, so that the persistence layer can detect the change by
    /// comparing the stored value with the current one.
    /// </remarks>
    public string?[] Slots
    {
        get => _slots;
        private set => _slots = value ?? new string?[SlotCount];
    }

    /// <summary>The patient the sheet belongs to.</summary>
    public PatientId PatientId { get; private set; }

    /// <summary>The selector that distinguishes several sheets for one patient.</summary>
    public int Selector { get; private set; }

    /// <summary>The date the sheet was taken.</summary>
    public DateTimeOffset RequestDate { get; private set; }

    /// <summary>Whether the sheet is printed with the report.</summary>
    public bool PrintFlag { get; private set; }

    /// <summary>The number of slots actually in use.</summary>
    public int UsedSlotCount { get; private set; }

    /// <summary>Creates a history sheet.</summary>
    public static PatientHistory Create(PatientId patientId, int selector, DateTimeOffset requestDate)
    {
        if (selector < 0)
        {
            throw new InvalidPatientOrderException("A history sheet selector cannot be negative.");
        }

        return new PatientHistory
        {
            PatientId = patientId,
            Selector = selector,
            RequestDate = requestDate,
            PrintFlag = true,
        };
    }

    /// <summary>Writes a value into one of the free-form slots.</summary>
    /// <exception cref="InvalidPatientOrderException">
    /// The slot number is outside the range 1 to <see cref="SlotCount"/>.
    /// </exception>
    public void SetSlot(int slotNumber, string? value)
    {
        if (slotNumber is < 1 or > SlotCount)
        {
            throw new InvalidPatientOrderException(
                $"A history slot must be between 1 and {SlotCount}.");
        }

        var slots = (string?[])_slots.Clone();

        slots[slotNumber - 1] = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        _slots = slots;

        var highestUsed = 0;

        for (var index = 0; index < _slots.Length; index++)
        {
            if (_slots[index] is not null)
            {
                highestUsed = index + 1;
            }
        }

        UsedSlotCount = highestUsed;
    }

    /// <summary>Reads the value held in one of the free-form slots.</summary>
    public string? GetSlot(int slotNumber)
    {
        if (slotNumber is < 1 or > SlotCount)
        {
            throw new InvalidPatientOrderException(
                $"A history slot must be between 1 and {SlotCount}.");
        }

        return _slots[slotNumber - 1];
    }

    /// <summary>Sets whether the sheet is printed with the report.</summary>
    public void SetPrintFlag(bool printFlag) => PrintFlag = printFlag;
}
