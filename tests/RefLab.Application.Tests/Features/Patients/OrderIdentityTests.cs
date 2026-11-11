using RefLab.Application.Common;
using RefLab.Application.Features.Patients;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Enums;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Patients;

public sealed class RegisterPatientOrderIdentityTests
{
    [Fact]
    public async Task The_order_carries_the_legacy_identifier_the_referring_party_and_the_physician()
    {
        using var host = new ApplicationTestHost();
        var party = TestData.CreateReferringParty(id: 19, code: "PRY-1", name: "Clinic One");
        host.Db.Seed(party);

        var result = await host.Sender.Send(
            new RegisterPatientCommand(
                "محمد أحمد",
                Sex.Male,
                30m,
                AgeUnit.Years,
                null,
                "Cairo",
                null,
                "0111111111",
                null,
                null,
                "PW-9",
                null,
                "EGP",
                CreateOrder: true,
                LegacyOrderIdentifier: "1511032410037",
                ReferringPartyCode: "PRY-1"),
            CancellationToken.None);

        Assert.Equal("1511032410037", result.LegacyIdentifier);

        var order = Assert.Single(host.Db.Items<PatientOrder>());

        Assert.Equal("1511032410037", order.LegacyIdentifier!.Value);
        Assert.Equal(party.Id, order.ReferringPartyId!.Value);
        Assert.Equal("PW-9", order.TreatingPhysicianCode);
    }

    [Fact]
    public async Task A_legacy_identifier_that_is_not_thirteen_characters_long_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new RegisterPatientCommand(
                    "Name",
                    Sex.Male,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "EGP",
                    true,
                    LegacyOrderIdentifier: "151103241003"),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_referring_party_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(
                new RegisterPatientCommand(
                    "Name",
                    Sex.Male,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "EGP",
                    true,
                    ReferringPartyCode: "MISSING"),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_sex_is_accepted_as_a_recorded_value()
    {
        using var host = new ApplicationTestHost();

        await host.Sender.Send(
            new RegisterPatientCommand(
                "Name",
                Sex.Unknown,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                "EGP",
                true),
            CancellationToken.None);

        Assert.Equal(Sex.Unknown, Assert.Single(host.Db.Items<Patient>()).Sex);
        Assert.Single(host.Db.Items<PatientOrder>());
    }
}

public sealed class AttachOrderIdentifierTests
{
    [Fact]
    public async Task The_identifier_is_attached_to_the_order_and_persisted()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        var result = await host.Sender.Send(
            new AttachOrderIdentifierCommand(order.Id, "15", "11", "03241", "003", '7'),
            CancellationToken.None);

        Assert.Equal("1511032410037", result.Value);
        Assert.Equal("11", result.PersonKey);
        Assert.Equal("003", result.DisplayCode);
        Assert.Equal("7", result.TrailingCharacter);

        var stored = Assert.Single(host.Db.Items<PatientOrder>());

        Assert.Equal("1511032410037", stored.LegacyIdentifier!.Value);
    }

    [Fact]
    public async Task An_unknown_order_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(
                new AttachOrderIdentifierCommand(Guid.NewGuid(), "15", "11", "03241", "003", '7'),
                CancellationToken.None));
    }
}

public sealed class OrderIdentifierBarcodeTests
{
    [Fact]
    public async Task The_legacy_identifier_is_used_as_the_barcode()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = PatientOrder.Create(
            patient.Id,
            TestData.Now,
            "EGP",
            "lab.tech",
            OrderIdentifier.Create("1511032410037"));
        host.Db.Seed(patient, order);

        var result = await host.Sender.Send(
            new GeneratePatientBarcodeCommand(order.Id),
            CancellationToken.None);

        Assert.Equal("1511032410037", result.Barcode);
        Assert.Equal("1511032410037", Assert.Single(host.Db.Items<PatientOrder>()).Barcode);
    }

    [Fact]
    public async Task An_order_without_a_legacy_identifier_still_receives_a_barcode()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        var order = TestData.CreateOrder(patient);
        host.Db.Seed(patient, order);

        var result = await host.Sender.Send(
            new GeneratePatientBarcodeCommand(order.Id),
            CancellationToken.None);

        Assert.StartsWith("B", result.Barcode);
    }
}
