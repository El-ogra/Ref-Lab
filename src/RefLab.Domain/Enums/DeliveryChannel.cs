namespace RefLab.Domain.Enums;

/// <summary>
/// The channel through which a laboratory result reaches the patient.
/// </summary>
/// <remarks>
/// Legacy evidence: three channels are established — physical hand-over
/// (<c>patienttest.give</c>), web publication through NATIGH.COM and SMS
/// notification through the <c>SMSSender</c> queue.
/// </remarks>
public enum DeliveryChannel
{
    /// <summary>The report was printed and handed to the patient or their representative.</summary>
    HandOver = 1,

    /// <summary>The result was published to the patient's NATIGH.COM account.</summary>
    NatighWebPublication = 2,

    /// <summary>The result was delivered as an SMS notification.</summary>
    SmsNotification = 3,
}
