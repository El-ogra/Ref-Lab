using RefLab.Application.Common;
using RefLab.Application.Features.Microbiology;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Microbiology;

namespace RefLab.Application.Tests.Features.Microbiology;

public class CultureTests
{
    [Fact]
    public async Task The_culture_result_is_recorded_with_its_susceptibilities()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);

        var result = await host.Sender.Send(
            new RecordCultureResultCommand(
                patient.Id.Value,
                "C-1",
                "Escherichia coli",
                Susceptibilities: new[]
                {
                    new AntibioticSensitivityDto("Amoxicillin", "Sensitive", null),
                }),
            CancellationToken.None);

        Assert.Equal("Escherichia coli", result.Organism);
        Assert.Single(result.Susceptibilities);
        Assert.Equal("Amoxicillin", result.Susceptibilities[0].AntibioticSymbol);
        Assert.Single(host.Db.Items<CultureResult>());
    }

    [Fact]
    public async Task An_empty_organism_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new RecordCultureResultCommand(
                    patient.Id.Value,
                    "C-1",
                    "  ",
                    Susceptibilities: Array.Empty<AntibioticSensitivityDto>()),
                CancellationToken.None));
    }

    [Fact]
    public async Task The_culture_results_are_returned_for_a_patient()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient();
        host.Db.Seed(patient);
        host.Db.Seed(TestData.CreateCultureResult(patient.Id));

        var result = await host.Sender.Send(
            new GetCultureResultsQuery(PatientId: patient.Id.Value),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("C-1", result[0].CultureId);
    }
}

public class AntibioticCatalogueTests
{
    [Fact]
    public async Task The_antibiotic_is_added_to_the_catalogue()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ManageAntibioticCatalogueCommand(
                1,
                "AMX",
                "Amoxicillin",
                new[] { "Amoxil" },
                IsAllowedInPregnancy: false,
                IsAllowedInChildren: true,
                Sensitivity: "Sensitive",
                SensitivityUnit: "mm"),
            CancellationToken.None);

        Assert.Equal("AMX", result.Symbol);
        Assert.Equal("Amoxil", result.CommonNames[0]);
        Assert.True(result.IsAllowedInChildren);
        Assert.Equal("Sensitive", result.Sensitivity);
        Assert.Single(host.Db.Items<Antibiotic>());
    }

    [Fact]
    public async Task An_empty_symbol_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new ManageAntibioticCatalogueCommand(
                    1,
                    "  ",
                    "Amoxicillin",
                    Array.Empty<string>()),
                CancellationToken.None));
    }
}

public class FarmResultTests
{
    [Fact]
    public async Task The_farm_transaction_is_recorded_and_the_farm_is_created()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ManageFarmResultsCommand(
                1,
                5,
                "Farm One",
                TestData.Now,
                100m,
                80m,
                "note",
                "Cairo",
                "0100000000"),
            CancellationToken.None);

        Assert.Equal(100m, result.IssuedAmount);
        Assert.Equal(80m, result.ReceivedAmount);
        Assert.Single(host.Db.Items<Domain.Farming.Farm>());
        Assert.Single(host.Db.Items<Domain.Farming.FarmTransaction>());
    }

    [Fact]
    public async Task An_empty_farm_name_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new ManageFarmResultsCommand(1, 5, "  ", TestData.Now),
                CancellationToken.None));
    }
}
