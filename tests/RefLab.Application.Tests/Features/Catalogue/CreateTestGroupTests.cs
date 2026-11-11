using RefLab.Application.Common;
using RefLab.Application.Features.Catalogue;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.ClinicalKnowledge;

namespace RefLab.Application.Tests.Features.Catalogue;

/// <summary>
/// The custom test group use case — the editor that composes a named group
/// out of the tests the operator has picked.
/// </summary>
/// <remarks>
/// The membership of a test group is the clinical sub-group the test belongs
/// to, so the use case allocates the next free sub-group code and moves the
/// chosen tests into it.
/// </remarks>
public sealed class CreateTestGroupTests
{
    [Fact]
    public async Task The_group_takes_the_next_free_code_and_its_tests_move_to_it()
    {
        using var host = new ApplicationTestHost();

        host.Db.Seed(
            ClinicalSubGroup.Create(10, "BLOOD PICTURE"),
            ClinicalSubGroup.Create(42, "LIVER FUNCTIONS"),
            TestData.CreateTestDefinition(code: 56, name: "Complete Blood Count", subGroupCode: 11),
            TestData.CreateTestDefinition(code: 57, name: "Liver Profile", subGroupCode: 12));

        var result = await host.Sender.Send(
            new CreateTestGroupCommand("CORE PANEL", new[] { 56, 57 }),
            CancellationToken.None);

        Assert.Equal(43, result.Code);

        var groups = host.Db.Items<ClinicalSubGroup>();

        Assert.Equal(3, groups.Count);
        Assert.Contains(43, groups.Select(group => group.Code));
        Assert.Contains("CORE PANEL", groups.Select(group => group.Name));

        var definitions = host.Db.Items<TestDefinition>();

        Assert.Equal(2, definitions.Count);
        Assert.All(definitions, test => Assert.Equal(43, test.SubGroupCode));
    }

    [Fact]
    public async Task The_arrangement_of_a_moved_test_is_kept()
    {
        using var host = new ApplicationTestHost();

        host.Db.Seed(
            ClinicalSubGroup.Create(10, "BLOOD PICTURE"),
            TestData.CreateTestDefinition(code: 56, name: "Complete Blood Count", subGroupCode: 11));

        await host.Sender.Send(
            new CreateTestGroupCommand("CORE PANEL", new[] { 56 }),
            CancellationToken.None);

        var moved = Assert.Single(host.Db.Items<TestDefinition>());

        Assert.Equal(11, moved.SubGroupCode);
        Assert.Equal(1, moved.ArrangementNumber);
    }

    [Fact]
    public async Task A_group_without_a_name_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new CreateTestGroupCommand("  ", new[] { 56 }),
                CancellationToken.None));
    }

    [Fact]
    public async Task A_group_without_a_test_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new CreateTestGroupCommand("CORE PANEL", Array.Empty<int>()),
                CancellationToken.None));
    }

    [Fact]
    public async Task A_group_that_lists_the_same_test_twice_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new CreateTestGroupCommand("CORE PANEL", new[] { 56, 56 }),
                CancellationToken.None));
    }

    [Fact]
    public async Task The_returned_group_carries_its_code_and_name()
    {
        using var host = new ApplicationTestHost();

        host.Db.Seed(
            ClinicalSubGroup.Create(10, "BLOOD PICTURE"),
            TestData.CreateTestDefinition(code: 56, name: "Complete Blood Count", subGroupCode: 11));

        var result = await host.Sender.Send(
            new CreateTestGroupCommand("CORE PANEL", new[] { 56 }),
            CancellationToken.None);

        Assert.Equal(11, result.Code);
        Assert.Equal("CORE PANEL", result.Name);
    }
}
