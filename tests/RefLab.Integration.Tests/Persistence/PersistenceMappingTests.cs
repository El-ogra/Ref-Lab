using Microsoft.EntityFrameworkCore;
using RefLab.Application.Abstractions;
using RefLab.Domain.Auditing;
using RefLab.Domain.Billing;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;
using RefLab.Domain.Farming;
using RefLab.Domain.Identity;
using RefLab.Domain.Microbiology;
using RefLab.Domain.Patients;
using RefLab.Domain.SendOut;
using RefLab.Domain.ValueObjects;
using RefLab.Integration.Tests.Integration;

namespace RefLab.Integration.Tests.Persistence;

/// <summary>
/// Every entity of the model is mapped, and every mapping survives a round
/// trip through the database.
/// </summary>
/// <remarks>
/// <para>
/// Section 8.1 of the Phase 0 discovery report requires the integration level
/// to cover <em>every repository and every mapping</em>. This class therefore
/// does two things: it pins down the shape of the model — so that an entity
/// can never be added without being mapped, and a mapped entity can never be
/// dropped from the model — and it writes and reads back every entity that
/// the Infrastructure test suite does not already cover.
/// </para>
/// <para>
/// The round trips run through the real unit of work and the real change
/// tracker, so the value conversions and the owned value objects are covered
/// and not merely the model.
/// </para>
/// </remarks>
public sealed class PersistenceMappingTests : IClassFixture<IntegrationTestHostFixture>
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    private readonly IntegrationTestHostFixture _fixture;

    /// <summary>Initialises a new test class.</summary>
    /// <param name="fixture">The fixture that stands up the application.</param>
    public PersistenceMappingTests(IntegrationTestHostFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>The host under test.</summary>
    private IntegrationTestHost Host => _fixture.Host;

    /// <summary>The unit of work of the long-lived context.</summary>
    private IUnitOfWork UnitOfWork => (IUnitOfWork)Host.Context;

    [Fact]
    public void Every_domain_entity_is_mapped()
    {
        var expected = new[]
        {
            typeof(Operator),
            typeof(LaboratorySettings),
            typeof(TestDefinition),
            typeof(ReferenceRange),
            typeof(ClinicalConstant),
            typeof(ClinicalSubGroup),
            typeof(TestUnit),
            typeof(ConversionFactor),
            typeof(Abbreviation),
            typeof(ProthrombinControlCalibration),
            typeof(Patient),
            typeof(PatientOrder),
            typeof(OrderTest),
            typeof(PatientHistory),
            typeof(PatientPicture),
            typeof(Payment),
            typeof(PriceList),
            typeof(ReferringParty),
            typeof(TreatingPhysician),
            typeof(Antibiotic),
            typeof(CultureResult),
            typeof(SendOutRequest),
            typeof(SampleDisposition),
            typeof(PrintAudit),
            typeof(AttendanceEntry),
            typeof(Farm),
            typeof(FarmTransaction),
        };

        var mapped = Host.Context.Model.GetEntityTypes()
            .Where(type => !type.IsOwned())
            .Select(type => type.ClrType)
            .Where(type => type.Namespace is not null && type.Namespace.StartsWith("RefLab.Domain", StringComparison.Ordinal))
            .Distinct()
            .OrderBy(type => type.Name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected.Length, mapped.Length);

        foreach (var entity in expected)
        {
            Assert.Contains(entity, mapped);
        }
    }

    [Fact]
    public void Every_mapped_entity_has_a_table_and_a_key()
    {
        var entityTypes = Host.Context.Model.GetEntityTypes()
            .Where(type => !type.IsOwned())
            .Where(type => type.ClrType.Namespace is not null &&
                           type.ClrType.Namespace.StartsWith("RefLab.Domain", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(entityTypes);

        foreach (var entityType in entityTypes)
        {
            Assert.False(
                string.IsNullOrWhiteSpace(entityType.GetTableName()),
                $"{entityType.ClrType.Name} is not mapped to a table.");
            Assert.NotNull(entityType.FindPrimaryKey());
        }
    }

    [Fact]
    public void The_value_objects_are_mapped_as_owned_types()
    {
        var owned = Host.Context.Model.GetEntityTypes()
            .Where(type => type.IsOwned())
            .Select(type => type.ClrType)
            .ToArray();

        Assert.Contains(typeof(Money), owned);
        Assert.Contains(typeof(Percentage), owned);
        Assert.Contains(typeof(OrderIdentifier), owned);
        Assert.Contains(typeof(ResultValue), owned);
        Assert.Contains(typeof(AgeBand), owned);
        Assert.Contains(typeof(ReferenceInterval), owned);
        Assert.Contains(typeof(CriticalInterval), owned);
    }

    [Fact]
    public async Task The_laboratory_settings_survive_a_round_trip()
    {
        var settings = LaboratorySettings.Create();

        settings.SetPaperSize(PaperSize.A4);
        settings.SetReportSequenceStart(1984);
        settings.SetBarcodeAutoCreated(true);
        settings.SetWorkSheetPrinted(true);
        settings.SetHistoryDisplayMode(2);
        settings.SetGlobalHeaderTexts("Synthetic header", "Synthetic secondary header");
        settings.SetMargins(1m, 1m, 2m, 2m);
        settings.SetBackgroundColour("#FFFFFF");
        settings.SetDefaultAccountType("Cash");
        settings.SetConnectionReceipt("Synthetic receipt header", "Synthetic receipt footer");
        settings.BindPrinter(PrinterRole.Report, "Synthetic Report Printer");
        settings.BindPrinter(PrinterRole.Barcode, "Synthetic Barcode Printer");

        UnitOfWork.Add(settings);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        var stored = Host.ReadAll<LaboratorySettings>().Single();

        Assert.Equal(PaperSize.A4, stored.PaperSize);
        Assert.Equal(1984, stored.ReportSequenceStart);
        Assert.True(stored.IsBarcodeAutoCreated);
        Assert.True(stored.IsWorkSheetPrinted);
        Assert.Equal(2, stored.HistoryDisplayMode);
        Assert.Equal("Synthetic header", stored.GlobalHeaderText);
        Assert.Equal("Synthetic secondary header", stored.GlobalHeaderSecondaryText);
        Assert.Equal(1m, stored.LeftMargin);
        Assert.Equal(2m, stored.TopMargin);
        Assert.Equal("#FFFFFF", stored.BackgroundColour);
        Assert.Equal("Cash", stored.DefaultAccountType);
        Assert.Equal("Synthetic receipt header", stored.ConnectionReceiptHeader);
        Assert.Equal("Synthetic receipt footer", stored.ConnectionReceiptFooter);
        Assert.Equal("Synthetic Report Printer", stored.Printers[PrinterRole.Report]);
        Assert.Equal("Synthetic Barcode Printer", stored.Printers[PrinterRole.Barcode]);
    }

    [Fact]
    public async Task The_clinical_knowledge_rows_survive_a_round_trip()
    {
        var subGroup = ClinicalSubGroup.Create(
            SyntheticData.NextCatalogueId(),
            "SYNTHETIC SUB GROUP",
            "مجموعة اصطناعية",
            isDeleteProtected: false);

        var unit = TestUnit.Create(SyntheticData.NextCatalogueId(), "SYNTHETIC UNIT");

        var factor = ConversionFactor.Create(
            "SYNALB",
            "g/dL",
            "g/L",
            10m,
            fromDecimals: 1,
            toDecimals: 0);

        var abbreviation = Abbreviation.Create(
            SyntheticData.NextCatalogueId(),
            "SYN",
            "Synthetic abbreviation");

        var calibration = ProthrombinControlCalibration.Create(
            SyntheticData.NextCatalogueId(),
            controlTime: 12.5m,
            concentration: 40m);

        UnitOfWork.Add(subGroup);
        UnitOfWork.Add(unit);
        UnitOfWork.Add(factor);
        UnitOfWork.Add(abbreviation);
        UnitOfWork.Add(calibration);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        Assert.Equal("SYNTHETIC SUB GROUP", Host.ReadAll<ClinicalSubGroup>().Single().Name);
        Assert.Equal("SYNTHETIC UNIT", Host.ReadAll<TestUnit>().Single().Name);

        var storedFactor = Host.ReadAll<ConversionFactor>().Single();
        Assert.Equal("SYNALB", storedFactor.TestSimpleName);
        Assert.Equal("g/dL", storedFactor.FromUnit);
        Assert.Equal("g/L", storedFactor.ToUnit);
        Assert.Equal(10m, storedFactor.Factor);

        Assert.Equal("Synthetic abbreviation", Host.ReadAll<Abbreviation>().Single().Meaning);
        Assert.Equal(12.5m, Host.ReadAll<ProthrombinControlCalibration>().Single().ControlTime);
    }

    [Fact]
    public async Task The_pricing_rows_survive_a_round_trip()
    {
        var priceList = PriceList.Create(
            SyntheticData.NextCatalogueId(),
            "SYNALB",
            "SYNTHETIC PRICE LIST",
            250m);

        var physician = TreatingPhysician.Create(
            SyntheticData.NextCatalogueId(),
            "SYNP",
            "Synthetic physician");

        physician.SetPhone("0100000000");

        UnitOfWork.Add(priceList);
        UnitOfWork.Add(physician);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        var storedPriceList = Host.ReadAll<PriceList>().Single();

        Assert.Equal("SYNALB", storedPriceList.TestSimpleName);
        Assert.Equal("SYNTHETIC PRICE LIST", storedPriceList.PriceListName);
        Assert.Equal(250m, storedPriceList.Price);

        var storedPhysician = Host.ReadAll<TreatingPhysician>().Single();

        Assert.Equal("SYNP", storedPhysician.Code);
        Assert.Equal("Synthetic physician", storedPhysician.Name);
        Assert.Equal("0100000000", storedPhysician.Phone);
    }

    [Fact]
    public async Task The_patient_picture_survives_a_round_trip()
    {
        var patient = Patient.Create("P6-Synthetic-Picture", Sex.Female, 30m, AgeUnit.Years);

        UnitOfWork.Add(patient);

        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "p6.mapper");

        UnitOfWork.Add(order);

        await UnitOfWork.SaveChangesAsync();

        var picture = PatientPicture.Create(patient.Id, 1, "/synthetic/picture.png", "A caption", order.Id, null);

        UnitOfWork.Add(picture);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        var stored = Host.ReadAll<PatientPicture>().Single();

        Assert.Equal(patient.Id.Value, stored.PatientId.Value);
        Assert.Equal(1, stored.Slot);
        Assert.Equal("/synthetic/picture.png", stored.ImagePath);
        Assert.Equal("A caption", stored.Caption);
        Assert.Equal(order.Id, stored.OrderId);
    }

    [Fact]
    public async Task The_send_out_rows_survive_a_round_trip()
    {
        var (orderTestId, _) = await SeedOrderTestAsync();

        var request = SendOutRequest.Create(orderTestId, "Synthetic external laboratory", 300m, 350m);

        request.MarkDispatched(Moment);
        request.MarkSettled(Moment);

        var disposition = SampleDisposition.Create(
            orderTestId,
            SampleDispositionAction.Split,
            Moment,
            "p6.mapper",
            "Synthetic external laboratory");

        UnitOfWork.Add(request);
        UnitOfWork.Add(disposition);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        var storedRequest = Host.ReadAll<SendOutRequest>().Single();

        Assert.Equal(orderTestId, storedRequest.OrderTestId);
        Assert.Equal("Synthetic external laboratory", storedRequest.DestinationLabName);
        Assert.Equal(300m, storedRequest.OutPrice);
        Assert.True(storedRequest.IsSettled);

        var storedDisposition = Host.ReadAll<SampleDisposition>().Single();

        Assert.Equal(orderTestId, storedDisposition.OrderTestId);
        Assert.Equal(SampleDispositionAction.Split, storedDisposition.Action);
        Assert.Equal("p6.mapper", storedDisposition.RecordedBy);
    }

    [Fact]
    public async Task The_attendance_entry_survives_a_round_trip()
    {
        var entry = AttendanceEntry.Create(
            _fixture.OperatorId,
            _fixture.OperatorName,
            AttendanceEventType.StartedWork,
            Moment);

        UnitOfWork.Add(entry);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        var stored = Host.ReadAll<AttendanceEntry>().Single();

        Assert.Equal(_fixture.OperatorId, stored.OperatorId);
        Assert.Equal(_fixture.OperatorName, stored.OperatorName);
        Assert.Equal(AttendanceEventType.StartedWork, stored.EventType);
        Assert.Equal(Moment, stored.OccurredAt);
    }

    [Fact]
    public async Task The_farm_rows_survive_a_round_trip()
    {
        var farm = Farm.Create(
            SyntheticData.NextCatalogueId(),
            "SYNTHETIC FARM",
            "Synthetic address",
            "0100000001",
            "0100000002",
            "A synthetic note");

        UnitOfWork.Add(farm);

        await UnitOfWork.SaveChangesAsync();

        var transaction = FarmTransaction.Create(
            SyntheticData.NextCatalogueId(),
            farm.Id,
            farm.Name,
            Moment,
            issuedAmount: 100m,
            receivedAmount: 40m,
            note: "A synthetic transaction");

        UnitOfWork.Add(transaction);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        var storedFarm = Host.ReadAll<Farm>().Single();

        Assert.Equal("SYNTHETIC FARM", storedFarm.Name);
        Assert.Equal("0100000001", storedFarm.Phone);
        Assert.Equal("0100000002", storedFarm.SecondaryPhone);

        var storedTransaction = Host.ReadAll<FarmTransaction>().Single();

        Assert.Equal(farm.Id, storedTransaction.FarmId);
        Assert.Equal(100m, storedTransaction.IssuedAmount);
        Assert.Equal(40m, storedTransaction.ReceivedAmount);
    }

    [Fact]
    public async Task The_antibiotic_and_the_culture_survive_a_round_trip()
    {
        var antibiotic = Antibiotic.Create(
            SyntheticData.NextCatalogueId(),
            "SYN-CIP",
            "Synthetic ciprofloxacin");

        UnitOfWork.Add(antibiotic);

        var patient = Patient.Create("P6-Synthetic-Culture", Sex.Male, 44m, AgeUnit.Years);

        UnitOfWork.Add(patient);

        await UnitOfWork.SaveChangesAsync();

        var culture = CultureResult.Create(patient.Id, "SYN-CULTURE", "Escherichia coli", Moment);

        culture.AddSusceptibility(AntibioticSensitivity.Create("SYN-CIP", "Sensitive", "mm"));

        UnitOfWork.Add(culture);

        await UnitOfWork.SaveChangesAsync();

        Host.Context.ChangeTracker.Clear();

        Assert.Equal("Synthetic ciprofloxacin", Host.ReadAll<Antibiotic>().Single().ScientificName);

        var stored = Host.ReadAll<CultureResult>().Single();

        Assert.Equal("Escherichia coli", stored.Organism);
        Assert.Equal(patient.Id.Value, stored.PatientId.Value);
    }

    private async Task<(Guid OrderTestId, Guid OrderId)> SeedOrderTestAsync()
    {
        var suffix = SyntheticData.Unique();

        var patient = Patient.Create(SyntheticData.PatientName(suffix), Sex.Male, 30m, AgeUnit.Years);

        UnitOfWork.Add(patient);

        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "p6.mapper");

        UnitOfWork.Add(order);

        await UnitOfWork.SaveChangesAsync();

        var orderTest = OrderTest.Create(
            order.Id,
            IntegrationTestHostFixture.SharedTestCode,
            "SYNALB",
            1,
            Moment,
            "p6.mapper");

        UnitOfWork.Add(orderTest);

        await UnitOfWork.SaveChangesAsync();

        return (orderTest.Id, order.Id);
    }
}
