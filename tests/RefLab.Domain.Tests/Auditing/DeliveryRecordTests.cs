using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;

namespace RefLab.Domain.Tests.Auditing;

/// <summary>
/// Every digital delivery is queued and auditable, exactly as the legacy
/// <c>SMSSender</c> table does.
/// </summary>
public class DeliveryRecordTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_stores_the_message_and_its_metadata()
    {
        var record = DeliveryRecord.Create(
            DeliveryChannel.SmsNotification,
            "Your result is ready",
            Moment,
            patientId: RefLab.Domain.ValueObjects.PatientId.New(),
            recipientMobile: "0111111111",
            doctorName: "Dr. Name",
            sentBy: "lab.tech");

        Assert.Equal(DeliveryChannel.SmsNotification, record.Channel);
        Assert.Equal("Your result is ready", record.MessageText);
        Assert.Equal(20, record.MessageLength);
        Assert.Equal("0111111111", record.RecipientMobile);
        Assert.Equal("lab.tech", record.SentBy);
    }

    [Fact]
    public void Create_rejects_an_empty_message()
    {
        Assert.Throws<Exceptions.InvalidPatientOrderException>(() =>
            DeliveryRecord.Create(DeliveryChannel.SmsNotification, "   ", Moment));
    }

    [Fact]
    public void The_message_length_reflects_the_stored_text()
    {
        var record = DeliveryRecord.Create(DeliveryChannel.NatighWebPublication, "abcd", Moment);

        Assert.Equal(4, record.MessageLength);
    }
}
