using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Enums;
using RefLab.Domain.ValueObjects;

namespace RefLab.Application.Tests.Features.Catalogue;

public class CreateTestDefinitionTests
{
    [Fact]
    public async Task The_test_is_added_to_the_catalogue_with_its_prices()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new CreateTestDefinitionCommand(56, "Complete Blood Count", "CBC", 11, 150m, 100m, 200m, "EDTA Blood", "صورة دم كاملة", "CBC"),
            CancellationToken.None);

        Assert.Equal(56, result.Code);
        Assert.Equal(150m, result.PatientPrice);
        Assert.Equal("EDTA Blood", result.SampleType);
        Assert.Equal("صورة دم كاملة", result.ArabicName);
        Assert.Single(host.Db.Items<TestDefinition>());
    }

    [Fact]
    public async Task A_duplicate_test_code_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateTestDefinition(code: 56));

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new CreateTestDefinitionCommand(56, "Duplicate", "CBC", 11, 150m, 100m, 200m, null, null, null),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_empty_name_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new CreateTestDefinitionCommand(56, "  ", "CBC", 11, 150m, 100m, 200m, null, null, null),
                CancellationToken.None));
    }
}

public class UpdateTestDefinitionTests
{
    [Fact]
    public async Task The_descriptive_attributes_and_the_prices_are_amended()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateTestDefinition());

        var result = await host.Sender.Send(
            new UpdateTestDefinitionCommand(56, Name: "CBC with differential", SimpleName: "CBC", Abbreviation: "S", SampleType: "Serum", Description: "description", PatientPrice: 175m, IsActive: false),
            CancellationToken.None);

        Assert.Equal("CBC with differential", result.Name);
        Assert.Equal("S", result.Abbreviation);
        Assert.Equal(175m, result.PatientPrice);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task The_urine_analysis_cannot_be_renamed()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateTestDefinition(code: 4, name: "Urine Analysis", simpleName: "URINE"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new UpdateTestDefinitionCommand(4, Name: "Urine Examination"),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_test_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(
                new UpdateTestDefinitionCommand(999, Name: "x"),
                CancellationToken.None));
    }
}

public class ReferenceRangeManagementTests
{
    [Fact]
    public async Task The_range_is_added_with_its_limits()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new CreateReferenceRangeCommand(56, "Hgb", 11, SexScope.Male, AgeUnit.Years, AgeFrom: 0m, AgeTo: 120m, LowLimit: 12m, HighLimit: 16m, LowExceptionText: "منخفضة", HighExceptionText: "عالٍ", RangeText: "Male 12 - 16 g/dL", Unit: "g/dL"),
            CancellationToken.None);

        Assert.Equal(12m, result.LowLimit);
        Assert.Equal(SexScope.Male, result.SexScope);
        Assert.Single(host.Db.Items<ReferenceRange>());
    }

    [Fact]
    public async Task An_age_window_that_ends_before_it_starts_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new CreateReferenceRangeCommand(56, "Hgb", 11, SexScope.All, AgeUnit.Years, AgeFrom: 12m, AgeTo: 5m),
                CancellationToken.None));
    }

    [Fact]
    public async Task The_limits_of_an_existing_range_are_amended()
    {
        using var host = new ApplicationTestHost();
        var range = TestData.CreateReferenceRange(id: 3);
        host.Db.Seed(range);

        var result = await host.Sender.Send(
            new UpdateReferenceRangeCommand(3, LowLimit: 13m, HighLimit: 17m, RangeText: "Male 13 - 17 g/dL"),
            CancellationToken.None);

        Assert.Equal(13m, result.LowLimit);
        Assert.Equal(17m, result.HighLimit);
        Assert.Equal(13m, range.Interval.LowLimit);
    }

    [Fact]
    public async Task Inverted_limits_are_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateReferenceRange(id: 3));

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new UpdateReferenceRangeCommand(3, LowLimit: 17m, HighLimit: 13m), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_range_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new UpdateReferenceRangeCommand(99, 1m, 2m), CancellationToken.None));
    }
}

public class UnitsAndAbbreviationsTests
{
    [Fact]
    public async Task A_unit_and_a_conversion_factor_are_added()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new ManageUnitsAndConversionsCommand("g/L", "Albumin", "g/dL", "g/L", 10m, 2, 1),
            CancellationToken.None);

        Assert.Single(result.Units);
        Assert.Equal("g/L", result.Units[0].Name);
        Assert.Single(result.Conversions);
        Assert.Equal(10m, result.Conversions[0].Factor);
    }

    [Fact]
    public async Task Nothing_supplied_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new ManageUnitsAndConversionsCommand(null, null, null, null, null, 0, 0), CancellationToken.None));
    }

    [Fact]
    public async Task The_abbreviation_is_added_and_then_amended()
    {
        using var host = new ApplicationTestHost();

        var created = await host.Sender.Send(
            new ManageAbbreviationsCommand(5, "CBC", "Complete Blood Count"),
            CancellationToken.None);

        Assert.Equal("CBC", created.Text);

        var amended = await host.Sender.Send(
            new ManageAbbreviationsCommand(5, "CBC", "Complete Blood Count with differential"),
            CancellationToken.None);

        Assert.Equal("Complete Blood Count with differential", amended.Meaning);
        Assert.Single(host.Db.Items<Abbreviation>());
    }

    [Fact]
    public async Task The_clinical_sub_group_is_added_and_then_amended()
    {
        using var host = new ApplicationTestHost();

        var created = await host.Sender.Send(
            new ManageClinicalSubGroupsCommand(11, "BLOOD PICTURE", "صورة دم", true),
            CancellationToken.None);

        Assert.Equal("BLOOD PICTURE", created.Name);

        var amended = await host.Sender.Send(
            new ManageClinicalSubGroupsCommand(11, "BLOOD COUNT", null, null),
            CancellationToken.None);

        Assert.Equal("BLOOD COUNT", amended.Name);
        Assert.Single(host.Db.Items<ClinicalSubGroup>());
    }
}
