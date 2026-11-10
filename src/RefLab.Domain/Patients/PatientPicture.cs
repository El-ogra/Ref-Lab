using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Patients;

/// <summary>
/// A picture attached to a patient order.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the <c>pictures</c> table carries the patient identifier,
/// the request date, the test code and fifteen picture slots
/// (<c>pictpix1</c> to <c>pictpix15</c>), each paired with a value label
/// (<c>Label1V</c> to <c>Label15V</c>) and a text label
/// (<c>Label1T</c> to <c>Label15T</c>).
/// </para>
/// <para>
/// Ref Lab models each picture as one record rather than fifteen fixed columns,
/// so that the number of attachments is not capped by the legacy layout.
/// </para>
/// </remarks>
public sealed class PatientPicture
{
    /// <summary>The number of picture slots the legacy sheet provides.</summary>
    public const int SlotCount = 15;

    private PatientPicture()
    {
    }

    /// <summary>The identity of the picture.</summary>
    public Guid Id { get; private set; }

    /// <summary>The patient the picture belongs to.</summary>
    public PatientId PatientId { get; private set; }

    /// <summary>The order the picture was attached to.</summary>
    public Guid? OrderId { get; private set; }

    /// <summary>The test the picture relates to.</summary>
    public int? TestCode { get; private set; }

    /// <summary>The slot the picture occupies, between 1 and <see cref="SlotCount"/>.</summary>
    public int Slot { get; private set; }

    /// <summary>The stored image path.</summary>
    public string ImagePath { get; private set; } = string.Empty;

    /// <summary>The caption shown with the picture.</summary>
    public string? Caption { get; private set; }

    /// <summary>Creates a patient picture.</summary>
    /// <exception cref="InvalidPatientOrderException">
    /// The slot is outside the available range or the image path is empty.
    /// </exception>
    public static PatientPicture Create(
        PatientId patientId,
        int slot,
        string imagePath,
        string? caption = null,
        Guid? orderId = null,
        int? testCode = null)
    {
        if (slot is < 1 or > SlotCount)
        {
            throw new InvalidPatientOrderException(
                $"A picture slot must be between 1 and {SlotCount}.");
        }

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            throw new InvalidPatientOrderException("A patient picture must carry an image path.");
        }

        return new PatientPicture
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            OrderId = orderId,
            TestCode = testCode,
            Slot = slot,
            ImagePath = imagePath.Trim(),
            Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
        };
    }

    /// <summary>Updates the caption of the picture.</summary>
    public void SetCaption(string? caption) =>
        Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim();
}
