using Microsoft.EntityFrameworkCore;
using RefLab.Application.Abstractions;
using RefLab.Domain.Billing;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Configuration;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;
using RefLab.Domain.Microbiology;
using RefLab.Domain.Patients;
using RefLab.Domain.ValueObjects;

namespace RefLab.Infrastructure.Tests.Persistence;

/// <summary>
/// The persistence adapter is exercised against a real database.
/// </summary>
/// <remarks>
/// Every test writes through <see cref="IUnitOfWork"/> and reads back through
/// <see cref="IAppDbContext"/>, so the whole write path — the unit of work,
/// the change tracker, the value conversions and the owned value objects — is
/// covered, not just the model.
/// </remarks>
public class PersistenceIntegrationTests
{
    private static readonly DateTimeOffset Moment = new(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task An_operator_is_written_and_read_back()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var @operator = Operator.Create(1, "lab.tech", "hash:Password2026!", "Technician");

        @operator.Grant(Permission.EnterResults);
        @operator.Grant(Permission.ReviewResults);

        unitOfWork.Add(@operator);

        await unitOfWork.SaveChangesAsync();

        var stored = readSide.Set<Operator>().Single(candidate => candidate.Id == 1);

        Assert.Equal("lab.tech", stored.Name);
        Assert.Equal("Technician", stored.RoleCaption);
        Assert.Equal("hash:Password2026!", stored.PasswordHash);
        Assert.True(stored.Has(Permission.EnterResults));
        Assert.True(stored.Has(Permission.ReviewResults));
        Assert.False(stored.Has(Permission.PrintReports));
    }

    [Fact]
    public async Task A_permission_granted_after_the_read_is_persisted()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;

        var @operator = Operator.Create(1, "lab.tech", "hash");

        unitOfWork.Add(@operator);
        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var loaded = await context.Operators.SingleAsync(candidate => candidate.Id == 1);

        loaded.Grant(Permission.PrintReports);

        unitOfWork.Update(loaded);
        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var reloaded = await context.Operators.SingleAsync(candidate => candidate.Id == 1);

        Assert.True(reloaded.Has(Permission.PrintReports));
    }

    [Fact]
    public async Task A_patient_and_its_order_are_written_and_read_back()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female, 29m, AgeUnit.Years);
        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "lab.tech");

        unitOfWork.Add(patient);
        unitOfWork.Add(order);

        await unitOfWork.SaveChangesAsync();

        var stored = readSide.Set<PatientOrder>()
            .Single(candidate => candidate.Id == order.Id);

        Assert.Equal("EGP", stored.CurrencyCode);
        Assert.Equal("lab.tech", stored.CreatedBy);

        var storedPatient = readSide.Set<Patient>().Single(candidate => candidate.Id == patient.Id);

        Assert.Equal("Heba Ashraf", storedPatient.Name);
        Assert.Equal(Sex.Female, storedPatient.Sex);
        Assert.Equal(storedPatient.Id, stored.PatientId);
    }

    [Fact]
    public async Task The_money_the_percentage_and_the_identifier_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female);
        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "lab.tech");

        order.AttachLegacyIdentifier(OrderIdentifier.Create("AB1234567890X"));
        order.SetAmounts(
            Money.Create(350m, "EGP"),
            Money.Create(320m, "EGP"),
            Money.Create(350m, "EGP"),
            Percentage.Create(8.57m),
            8.57m);

        unitOfWork.Add(patient);
        unitOfWork.Add(order);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<PatientOrder>().Single(candidate => candidate.Id == order.Id);

        Assert.Equal("AB1234567890X", stored.LegacyIdentifier!.Value);
        Assert.Equal(350m, stored.TotalPrice!.Amount);
        Assert.Equal("EGP", stored.TotalPrice.CurrencyCode);
        Assert.Equal(320m, stored.TotalPriceAfterDiscount!.Amount);
        Assert.Equal(8.57m, stored.Discount!.Value);
    }

    [Fact]
    public async Task The_order_identifier_keeps_its_thirteen_character_layout()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female);
        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "lab.tech");

        order.AttachLegacyIdentifier(OrderIdentifier.FromSegments("AB", "12", "34567", "890", 'X'));

        unitOfWork.Add(patient);
        unitOfWork.Add(order);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<PatientOrder>().Single(candidate => candidate.Id == order.Id);
        var identifier = stored.LegacyIdentifier!;

        Assert.Equal(13, identifier.Value.Length);
        Assert.Equal("12", identifier.PersonKey);
        Assert.Equal("890", identifier.DisplayCode);
        Assert.Equal("34567", identifier.MiddleSegment);
        Assert.Equal('X', identifier.TrailingCharacter);
    }

    [Fact]
    public async Task The_requested_tests_of_an_order_are_read_back_with_the_order()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female);
        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "lab.tech");
        var test = OrderTest.Create(order.Id, 56, "Hgb", 1, Moment, "lab.tech");

        order.AddTest(test);

        unitOfWork.Add(patient);
        unitOfWork.Add(order);
        unitOfWork.Add(test);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<PatientOrder>()
            .Include(candidate => candidate.Tests)
            .Single(candidate => candidate.Id == order.Id);

        Assert.Single(stored.Tests);
        Assert.Equal("Hgb", stored.Tests.First().TestSimpleName);
    }

    [Fact]
    public async Task The_result_and_the_frozen_reference_range_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female);
        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "lab.tech");
        var test = OrderTest.Create(order.Id, 56, "Hgb", 1, Moment, "lab.tech");

        test.EnterResult(ResultValue.FromNumber(13.4m, "g/dL"), Moment, "lab.tech");
        test.SetPrice(Money.Create(120m, "EGP"));
        test.AttachReferenceSnapshot(ReferenceSnapshot.Capture(
            56,
            "Hgb",
            SexScope.All,
            AgeBand.Create(0m, 120m, AgeUnit.Years),
            ReferenceInterval.Create(
                12m,
                16m,
                "Low",
                "High",
                null,
                null,
                "12 - 16",
                "g/dL",
                CriticalInterval.Create(7m, 20m, "critical", "urgent", "call")),
            Moment));

        order.AddTest(test);

        unitOfWork.Add(patient);
        unitOfWork.Add(order);
        unitOfWork.Add(test);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<OrderTest>().Single(candidate => candidate.Id == test.Id);

        Assert.Equal("13.4", stored.Result!.Text);
        Assert.Equal(13.4m, stored.Result.NumericValue);
        Assert.Equal("g/dL", stored.Result.Unit);
        Assert.Equal(120m, stored.Price!.Amount);

        var snapshot = stored.ReferenceSnapshot!;

        Assert.Equal(56, snapshot.TestCode);
        Assert.Equal(SexScope.All, snapshot.SexScope);
        Assert.Equal(0m, snapshot.AgeBand.From);
        Assert.Equal(120m, snapshot.AgeBand.To);
        Assert.Equal(AgeUnit.Years, snapshot.AgeBand.Unit);
        Assert.Equal(12m, snapshot.Interval.LowLimit);
        Assert.Equal(16m, snapshot.Interval.HighLimit);
        Assert.Equal("12 - 16", snapshot.Interval.RangeText);
        Assert.Equal(7m, snapshot.Interval.CriticalInterval.LowCriticalLimit);
        Assert.Equal(20m, snapshot.Interval.CriticalInterval.HighCriticalLimit);
        Assert.Equal("call", snapshot.Interval.CriticalInterval.CriticalComment);
    }

    [Fact]
    public async Task The_additional_results_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female);
        var order = PatientOrder.Create(patient.Id, Moment, "EGP", "lab.tech");
        var test = OrderTest.Create(order.Id, 56, "Hgb", 1, Moment, "lab.tech");

        test.SetAdditionalResult(0, "  haematocrit  ", Moment, "lab.tech");
        test.SetAdditionalResult(2, "red cells", Moment, "lab.tech");

        unitOfWork.Add(patient);
        unitOfWork.Add(order);
        unitOfWork.Add(test);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<OrderTest>().Single(candidate => candidate.Id == test.Id);

        Assert.Equal("haematocrit", stored.AdditionalResults[0]);
        Assert.Null(stored.AdditionalResults[1]);
        Assert.Equal("red cells", stored.AdditionalResults[2]);
    }

    [Fact]
    public async Task The_slots_of_a_history_sheet_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female);
        var history = PatientHistory.Create(patient.Id, 1, Moment);

        history.SetSlot(1, "  haemoglobin  ");
        history.SetSlot(3, "sugar");

        unitOfWork.Add(patient);
        unitOfWork.Add(history);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<PatientHistory>()
            .Single(candidate => candidate.PatientId == patient.Id && candidate.Selector == 1);

        Assert.Equal("haemoglobin", stored.GetSlot(1));
        Assert.Null(stored.GetSlot(2));
        Assert.Equal("sugar", stored.GetSlot(3));
        Assert.Equal(3, stored.UsedSlotCount);
    }

    [Fact]
    public async Task The_values_of_a_clinical_constant_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var constant = ClinicalConstant.Create(ClinicalConstant.CbcGroupName, "1", "2", null, "4", "5", "6", "7");

        unitOfWork.Add(constant);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<ClinicalConstant>()
            .Single(candidate => candidate.GroupName == ClinicalConstant.CbcGroupName);

        Assert.Equal(7, stored.Values.Count);
        Assert.Equal("1", stored.Values[0]);
        Assert.Null(stored.Values[2]);
        Assert.Equal("7", stored.Values[6]);
    }

    [Fact]
    public async Task The_susceptibilities_of_a_culture_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var patient = Patient.Create("Heba Ashraf", Sex.Female);
        var culture = CultureResult.Create(patient.Id, "C-1", "Staphylococcus aureus", Moment);

        culture.AddSusceptibility(AntibioticSensitivity.Create("CXM", "Sensitive"));
        culture.AddSusceptibility(AntibioticSensitivity.Create("AK", "Resistant", "mm"));

        unitOfWork.Add(patient);
        unitOfWork.Add(culture);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<CultureResult>().Single(candidate => candidate.Id == culture.Id);

        Assert.Equal(2, stored.Susceptibilities.Count);
        Assert.Equal("CXM", stored.Susceptibilities[0].AntibioticSymbol);
        Assert.Equal("Sensitive", stored.Susceptibilities[0].Reading);
        Assert.Null(stored.Susceptibilities[0].Unit);
        Assert.Equal("AK", stored.Susceptibilities[1].AntibioticSymbol);
        Assert.Equal("mm", stored.Susceptibilities[1].Unit);
    }

    [Fact]
    public async Task The_common_names_of_an_antibiotic_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var antibiotic = Antibiotic.Create(1, "CXM", "Cefuroxime", "Urine", false, false, new[] { "Zinacef", "Ceftin" });

        unitOfWork.Add(antibiotic);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<Antibiotic>().Single(candidate => candidate.Id == 1);

        Assert.Equal(new[] { "Zinacef", "Ceftin" }, stored.CommonNames);
    }

    [Fact]
    public async Task The_printers_of_the_laboratory_settings_survive_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var settings = LaboratorySettings.Create();

        settings.BindPrinter(PrinterRole.Report, "HP LaserJet");
        settings.BindPrinter(PrinterRole.Barcode, "Zebra ZD421");

        unitOfWork.Add(settings);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<LaboratorySettings>().Single();

        Assert.Equal("HP LaserJet", stored.GetPrinter(PrinterRole.Report));
        Assert.Equal("Zebra ZD421", stored.GetPrinter(PrinterRole.Barcode));
        Assert.Null(stored.GetPrinter(PrinterRole.WorkSheet));
    }

    [Fact]
    public async Task A_commission_rate_survives_a_round_trip()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var party = ReferringParty.Create(1, "DR-1", "Dr Heba Ashraf");

        unitOfWork.Add(party);

        await unitOfWork.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var stored = readSide.Set<ReferringParty>().Single(candidate => candidate.Id == 1);

        Assert.Equal("DR-1", stored.Code);
        Assert.Equal("Dr Heba Ashraf", stored.Name);
    }

    [Fact]
    public async Task An_entity_is_removed_through_the_unit_of_work()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        var @operator = Operator.Create(1, "lab.tech", "hash");

        unitOfWork.Add(@operator);
        await unitOfWork.SaveChangesAsync();

        unitOfWork.Remove(@operator);
        await unitOfWork.SaveChangesAsync();

        Assert.Empty(readSide.Set<Operator>());
    }

    [Fact]
    public async Task A_query_reads_through_the_query_side()
    {
        using var database = new SqliteTestDatabase();
        using var context = database.CreateContext();

        var unitOfWork = (IUnitOfWork)context;
        var readSide = (IAppDbContext)context;

        unitOfWork.Add(Operator.Create(1, "lab.tech", "hash"));
        unitOfWork.Add(Operator.Create(2, "senior.tech", "hash"));

        await unitOfWork.SaveChangesAsync();

        var names = readSide.Set<Operator>()
            .Where(@operator => @operator.Name.StartsWith("senior"))
            .Select(@operator => @operator.Name)
            .ToArray();

        Assert.Equal(new[] { "senior.tech" }, names);
    }
}
