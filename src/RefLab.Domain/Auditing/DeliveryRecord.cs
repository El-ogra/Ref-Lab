using RefLab.Domain.Enums;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;
using RefLab.Domain.Patients;

namespace RefLab.Domain.Auditing;

/// <summary>
/// The audit record of a result delivered to a patient through a digital
/// channel.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the <c>SMSSender</c> table queues every outbound message
/// with the recipient mobile number (<c>smsMobile</c>), the full text
/// (<c>SMStext</c>), the message length (<c>SMSLength</c>), the patient
/// (<c>patientid</c>), the doctor's name shown in the message
/// (<c>PatDrName</c>), the sending operator (<c>username</c>) and the timestamp
/// (<c>requestdate</c>).
/// </para>
/// <para>
/// The legacy queue stores the mobile number and the message body in the clear
/// and indefinitely, with no evidenced retention limit. In a laboratory
/// context a mobile number is a personal identifier, so Ref Lab records the
/// delivery audit but the retention period is a policy decision for the owner.
/// </para>
/// </remarks>
public sealed class DeliveryRecord
{
    private DeliveryRecord()
    {
    }

    /// <summary>The identity of the delivery record.</summary>
    public Guid Id { get; private set; }

    /// <summary>The order the delivered result belongs to.</summary>
    public Guid? OrderId { get; private set; }

    /// <summary>The patient the result was delivered to.</summary>
    public PatientId? PatientId { get; private set; }

    /// <summary>The order test the delivered result belongs to.</summary>
    public Guid? OrderTestId { get; private set; }

    /// <summary>The channel the result was delivered through.</summary>
    public DeliveryChannel Channel { get; private set; }

    /// <summary>The recipient mobile number.</summary>
    public string? RecipientMobile { get; private set; }

    /// <summary>The message body.</summary>
    public string MessageText { get; private set; } = string.Empty;

    /// <summary>The name of the doctor shown in the message.</summary>
    public string? DoctorName { get; private set; }

    /// <summary>The operator who sent the message.</summary>
    public string? SentBy { get; private set; }

    /// <summary>The moment the message was queued.</summary>
    public DateTimeOffset QueuedAt { get; private set; }

    /// <summary>The length of the message body.</summary>
    public int MessageLength => MessageText.Length;

    /// <summary>Creates a delivery record.</summary>
    /// <exception cref="InvalidPatientOrderException">The message body is empty.</exception>
    public static DeliveryRecord Create(
        DeliveryChannel channel,
        string messageText,
        DateTimeOffset queuedAt,
        Guid? orderId = null,
        PatientId? patientId = null,
        Guid? orderTestId = null,
        string? recipientMobile = null,
        string? doctorName = null,
        string? sentBy = null)
    {
        if (string.IsNullOrWhiteSpace(messageText))
        {
            throw new InvalidPatientOrderException("A delivery record must carry a message body.");
        }

        return new DeliveryRecord
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            PatientId = patientId,
            OrderTestId = orderTestId,
            Channel = channel,
            RecipientMobile = string.IsNullOrWhiteSpace(recipientMobile) ? null : recipientMobile.Trim(),
            MessageText = messageText.Trim(),
            DoctorName = string.IsNullOrWhiteSpace(doctorName) ? null : doctorName.Trim(),
            SentBy = string.IsNullOrWhiteSpace(sentBy) ? null : sentBy.Trim(),
            QueuedAt = queuedAt,
        };
    }
}
