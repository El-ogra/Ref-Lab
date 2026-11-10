using RefLab.Application.Common;
using RefLab.Application.Features.Identity;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Identity;

namespace RefLab.Application.Tests.Features.Identity;

public class UpdateOperatorTests
{
    [Fact]
    public async Task The_operator_is_renamed_and_reassigned()
    {
        using var host = new ApplicationTestHost();
        var @operator = TestData.CreateOperator();
        host.Db.Seed(@operator);

        var result = await host.Sender.Send(
            new UpdateOperatorCommand(1, "senior.tech", "Senior", IsActive: false),
            CancellationToken.None);

        Assert.Equal("senior.tech", result.Name);
        Assert.Equal("Senior", result.RoleCaption);
        Assert.False(result.IsActive);
        Assert.Equal("senior.tech", @operator.Name);
    }

    [Fact]
    public async Task An_unknown_operator_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new UpdateOperatorCommand(99, "x", null, true), CancellationToken.None));
    }

    [Fact]
    public async Task An_empty_name_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator());

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new UpdateOperatorCommand(1, "  ", null, true), CancellationToken.None));
    }

    [Fact]
    public async Task An_operator_without_the_management_permission_is_rejected()
    {
        using var host = new ApplicationTestHost(grantAllPermissions: false);
        host.Db.Seed(TestData.CreateOperator());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(new UpdateOperatorCommand(1, "tech", null, true), CancellationToken.None));
    }
}

public class AssignOperatorPermissionsTests
{
    [Fact]
    public async Task The_granted_permissions_are_replaced()
    {
        using var host = new ApplicationTestHost();
        var @operator = TestData.CreateOperator();
        @operator.Grant(Permission.EnterResults);
        @operator.Grant(Permission.PrintReports);
        host.Db.Seed(@operator);

        var result = await host.Sender.Send(
            new AssignOperatorPermissionsCommand(1, new[] { "ReviewResults" }),
            CancellationToken.None);

        Assert.Equal(new[] { "ReviewResults" }, result.Permissions);
        Assert.False(@operator.Has(Permission.EnterResults));
        Assert.True(@operator.Has(Permission.ReviewResults));
    }

    [Fact]
    public async Task An_unknown_permission_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator());

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new AssignOperatorPermissionsCommand(1, new[] { "NotAPermission" }),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_operator_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new AssignOperatorPermissionsCommand(99, Array.Empty<string>()), CancellationToken.None));
    }
}

public class GetOperatorsTests
{
    [Fact]
    public async Task The_active_operators_are_listed()
    {
        using var host = new ApplicationTestHost();
        var active = TestData.CreateOperator(1, "active.tech");
        var inactive = TestData.CreateOperator(2, "inactive.tech");
        inactive.SetActive(false);

        host.Db.Seed(active, inactive);

        var result = await host.Sender.Send(new GetOperatorsQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("active.tech", result[0].Name);
    }

    [Fact]
    public async Task The_operators_are_found_by_name()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator(1, "alpha"), TestData.CreateOperator(2, "beta"));

        var result = await host.Sender.Send(new GetOperatorsQuery("alph", false), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("alpha", result[0].Name);
    }
}
