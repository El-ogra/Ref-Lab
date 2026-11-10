using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Enums;

namespace RefLab.Application.Tests.Features.Catalogue;

public class GetTestCatalogueHandlerTests
{
    [Fact]
    public async Task Only_active_tests_are_returned_by_default()
    {
        using var host = new ApplicationTestHost();
        var active = TestData.CreateTestDefinition(code: 56, name: "Complete Blood Count");
        var inactive = TestData.CreateTestDefinition(code: 57, name: "Old Test");
        inactive.Deactivate();

        host.Db.Seed(active, inactive);

        var result = await host.Sender.Send(new GetTestCatalogueQuery(), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(56, result.Items[0].Code);
    }

    [Fact]
    public async Task Tests_are_found_by_name_or_simple_name()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(
            TestData.CreateTestDefinition(code: 56, name: "Complete Blood Count", simpleName: "CBC"),
            TestData.CreateTestDefinition(code: 57, name: "Liver Profile", simpleName: "LIVER"));

        var result = await host.Sender.Send(
            new GetTestCatalogueQuery("Liver", null),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(57, result.Items[0].Code);
    }

    [Fact]
    public async Task Tests_are_filtered_by_sub_group()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(
            TestData.CreateTestDefinition(code: 56, subGroupCode: 11),
            TestData.CreateTestDefinition(code: 57, subGroupCode: 12));

        var result = await host.Sender.Send(
            new GetTestCatalogueQuery(null, 12),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(57, result.Items[0].Code);
    }

    [Fact]
    public async Task A_test_profile_carries_every_reference_range()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(
            TestData.CreateTestDefinition(),
            TestData.CreateReferenceRange(id: 1),
            TestData.CreateReferenceRange(id: 2, sexScope: SexScope.Male));

        var profile = await host.Sender.Send(new GetTestProfileQuery(56), CancellationToken.None);

        Assert.Equal(56, profile.Test.Code);
        Assert.Equal(2, profile.ReferenceRanges.Count);
    }

    [Fact]
    public async Task An_unknown_test_profile_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new GetTestProfileQuery(999), CancellationToken.None));
    }

    [Fact]
    public async Task The_reference_range_of_a_patient_is_resolved()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient(sex: Sex.Male, age: 30m);

        host.Db.Seed(patient, TestData.CreateReferenceRange(id: 1, sexScope: SexScope.All));
        host.Db.Seed(TestData.CreateReferenceRange(id: 2, sexScope: SexScope.Male, low: 13m, high: 17m));

        var range = await host.Sender.Send(
            new GetReferenceRangeForPatientQuery(patient.Id.Value, 56),
            CancellationToken.None);

        Assert.Equal(2, range.Id);
        Assert.Equal(13m, range.LowLimit);
    }

    [Fact]
    public async Task A_patient_without_a_matching_range_is_rejected()
    {
        using var host = new ApplicationTestHost();
        var patient = TestData.CreatePatient(sex: Sex.Female, age: 30m);

        host.Db.Seed(patient, TestData.CreateReferenceRange(id: 1, sexScope: SexScope.Male));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new GetReferenceRangeForPatientQuery(patient.Id.Value, 56), CancellationToken.None));
    }
}
