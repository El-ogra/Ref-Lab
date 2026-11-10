using RefLab.Application.Common;
using RefLab.Application.Features.Identity;
using RefLab.Application.Tests.Fakes;
using RefLab.Domain.Auditing;
using RefLab.Domain.Enums;
using RefLab.Domain.Identity;

namespace RefLab.Application.Tests.Features.Identity;

public class SignInHandlerTests
{
    [Fact]
    public async Task A_valid_sign_in_returns_the_operator()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator(name: "lab.tech", passwordHash: "hash:Password2026!"));

        var result = await host.Sender.Send(
            new SignInCommand("lab.tech", "Password2026!"),
            CancellationToken.None);

        Assert.Equal("lab.tech", result.Operator.Name);
    }

    [Fact]
    public async Task A_sign_in_records_the_attendance_start()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator());

        await host.Sender.Send(new SignInCommand("lab.tech", "Password2026!"), CancellationToken.None);

        var entries = host.Db.Items<AttendanceEntry>();

        Assert.Single(entries);
        Assert.Equal(AttendanceEventType.StartedWork, entries[0].EventType);
        Assert.Equal(1, host.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task An_unknown_operator_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new SignInCommand("nobody", "Password2026!"), CancellationToken.None));
    }

    [Fact]
    public async Task An_incorrect_password_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator());

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(new SignInCommand("lab.tech", "wrong-password"), CancellationToken.None));
    }

    [Fact]
    public async Task An_inactive_operator_is_rejected()
    {
        using var host = new ApplicationTestHost();
        var @operator = TestData.CreateOperator();
        @operator.SetActive(false);
        host.Db.Seed(@operator);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(new SignInCommand("lab.tech", "Password2026!"), CancellationToken.None));
    }

    [Fact]
    public async Task A_sign_in_without_an_operator_name_is_rejected_by_validation()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new SignInCommand("  ", "Password2026!"), CancellationToken.None));
    }
}

public class ChangePasswordHandlerTests
{
    [Fact]
    public async Task The_password_is_replaced_when_the_current_one_is_correct()
    {
        using var host = new ApplicationTestHost();
        var @operator = TestData.CreateOperator(passwordHash: "hash:Password2026!");
        host.Db.Seed(@operator);

        await host.Sender.Send(
            new ChangePasswordCommand(1, "Password2026!", "NewPassword2026!"),
            CancellationToken.None);

        Assert.Equal("hash:NewPassword2026!", @operator.PasswordHash);
    }

    [Fact]
    public async Task An_incorrect_current_password_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator(passwordHash: "hash:Password2026!"));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(new ChangePasswordCommand(1, "wrong", "NewPassword2026!"), CancellationToken.None));
    }

    [Fact]
    public async Task A_weak_new_password_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator(passwordHash: "hash:Password2026!"));

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(new ChangePasswordCommand(1, "Password2026!", "short"), CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_operator_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            host.Sender.Send(new ChangePasswordCommand(99, "Password2026!", "NewPassword2026!"), CancellationToken.None));
    }
}

public class CreateOperatorHandlerTests
{
    [Fact]
    public async Task An_operator_is_created_with_the_requested_permissions()
    {
        using var host = new ApplicationTestHost();

        var result = await host.Sender.Send(
            new CreateOperatorCommand(
                "new.tech",
                "Password2026!",
                "Technician",
                false,
                new[] { "EnterResults", "ReviewResults" }),
            CancellationToken.None);

        Assert.Equal("new.tech", result.Name);
        Assert.Contains("EnterResults", result.Permissions);
        Assert.Contains("ReviewResults", result.Permissions);
        Assert.Single(host.Db.Items<Operator>());
    }

    [Fact]
    public async Task A_duplicate_operator_name_is_rejected()
    {
        using var host = new ApplicationTestHost();
        host.Db.Seed(TestData.CreateOperator(name: "lab.tech"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            host.Sender.Send(
                new CreateOperatorCommand("lab.tech", "Password2026!", null, false, Array.Empty<string>()),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_unknown_permission_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new CreateOperatorCommand("new.tech", "Password2026!", null, false, new[] { "NotAPermission" }),
                CancellationToken.None));
    }

    [Fact]
    public async Task A_weak_password_is_rejected()
    {
        using var host = new ApplicationTestHost();

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            host.Sender.Send(
                new CreateOperatorCommand("new.tech", "weak", null, false, Array.Empty<string>()),
                CancellationToken.None));
    }

    [Fact]
    public async Task An_operator_without_the_management_permission_is_rejected()
    {
        using var host = new ApplicationTestHost(grantAllPermissions: false);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            host.Sender.Send(
                new CreateOperatorCommand("new.tech", "Password2026!", null, false, Array.Empty<string>()),
                CancellationToken.None));
    }
}
