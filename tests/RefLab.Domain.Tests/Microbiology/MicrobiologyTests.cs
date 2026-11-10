using RefLab.Domain.Exceptions;
using RefLab.Domain.Microbiology;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Microbiology;

public class CultureResultTests
{
    [Fact]
    public void The_culture_records_the_organism_and_the_condition()
    {
        var culture = CultureResult.Create(
            PatientId.New(),
            "C-1",
            "Escherichia coli",
            DateTimeOffset.UtcNow,
            secondOrganism: "Klebsiella",
            condition: "growth",
            colony: "large");

        Assert.Equal("Escherichia coli", culture.Organism);
        Assert.Equal("Klebsiella", culture.SecondOrganism);
        Assert.Equal("growth", culture.Condition);
        Assert.Equal("large", culture.Colony);
    }

    [Fact]
    public void An_empty_organism_is_rejected()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            CultureResult.Create(PatientId.New(), "C-1", "  ", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void An_empty_culture_identifier_is_rejected()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            CultureResult.Create(PatientId.New(), "  ", "Organism", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void A_susceptibility_is_recorded_once()
    {
        var culture = CultureResult.Create(PatientId.New(), "C-1", "Organism", DateTimeOffset.UtcNow);

        culture.AddSusceptibility(AntibioticSensitivity.Create("AMX", "Sensitive"));

        Assert.Single(culture.Susceptibilities);

        Assert.Throws<InvalidTestDefinitionException>(() =>
            culture.AddSusceptibility(AntibioticSensitivity.Create("amx", "Resistant")));
    }

    [Fact]
    public void A_susceptibility_can_be_removed()
    {
        var culture = CultureResult.Create(PatientId.New(), "C-1", "Organism", DateTimeOffset.UtcNow);
        culture.AddSusceptibility(AntibioticSensitivity.Create("AMX", "Sensitive"));

        var removed = culture.RemoveSusceptibility("AMX");

        Assert.True(removed);
        Assert.Empty(culture.Susceptibilities);
    }
}

public class AntibioticTests
{
    [Fact]
    public void The_antibiotic_records_its_names_and_its_clinical_flags()
    {
        var antibiotic = Antibiotic.Create(
            1,
            "AMX",
            "Amoxicillin",
            cultureType: "uro",
            isAllowedInPregnancy: false,
            isAllowedInChildren: true,
            commonNames: new[] { "Amoxil", "  " });

        Assert.Equal("Amoxicillin", antibiotic.ScientificName);
        Assert.True(antibiotic.IsAllowedInChildren);
        Assert.False(antibiotic.IsAllowedInPregnancy);
        Assert.Single(antibiotic.CommonNames);
    }

    [Fact]
    public void An_empty_symbol_is_rejected()
    {
        Assert.Throws<InvalidTestDefinitionException>(() =>
            Antibiotic.Create(1, "  ", "Amoxicillin"));
    }

    [Fact]
    public void The_susceptibility_is_stored()
    {
        var antibiotic = Antibiotic.Create(1, "AMX", "Amoxicillin");

        antibiotic.SetSusceptibility("Sensitive", "mm");

        Assert.Equal("Sensitive", antibiotic.Sensitivity);
        Assert.Equal("mm", antibiotic.SensitivityUnit);
    }
}
