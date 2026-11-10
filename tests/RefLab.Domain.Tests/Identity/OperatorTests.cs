using RefLab.Domain.Exceptions;
using RefLab.Domain.Identity;

namespace RefLab.Domain.Tests.Identity;

/// <summary>
/// The operator account. The legacy table named <c>patients</c> holds
/// operators, not patients.
/// </summary>
public class OperatorTests
{
    private static Operator CreateOperator() =>
        Operator.Create(id: 1, name: "Lab Technician", passwordHash: "hash-placeholder");

    [Fact]
    public void Create_stores_the_identity_and_the_role()
    {
        var @operator = Operator.Create(1, "Lab Technician", "hash-placeholder", "Technician");

        Assert.Equal(1, @operator.Id);
        Assert.Equal("Lab Technician", @operator.Name);
        Assert.Equal("Technician", @operator.RoleCaption);
        Assert.True(@operator.IsActive);
        Assert.False(@operator.IsAdministrator);
    }

    [Fact]
    public void Create_rejects_an_empty_name()
    {
        Assert.Throws<InvalidOperatorException>(() =>
            Operator.Create(1, "  ", "hash-placeholder"));
    }

    [Fact]
    public void Create_rejects_an_empty_password_hash()
    {
        Assert.Throws<InvalidOperatorException>(() =>
            Operator.Create(1, "Lab Technician", "  "));
    }

    [Fact]
    public void A_granted_permission_is_held_by_the_operator()
    {
        var @operator = CreateOperator();

        @operator.Grant(Permission.EnterResults);

        Assert.True(@operator.Has(Permission.EnterResults));
        Assert.Contains(Permission.EnterResults, @operator.Permissions);
    }

    [Fact]
    public void A_revoked_permission_is_no_longer_held()
    {
        var @operator = CreateOperator();
        @operator.Grant(Permission.EnterResults);

        @operator.Revoke(Permission.EnterResults);

        Assert.False(@operator.Has(Permission.EnterResults));
    }

    [Fact]
    public void An_administrator_holds_every_permission()
    {
        var @operator = CreateOperator();
        @operator.SetAdministrator(true);

        Assert.True(@operator.Has(Permission.ManageOperators));
        Assert.True(@operator.Has(Permission.ManageAccounting));
    }

    [Fact]
    public void An_operator_with_no_grants_holds_no_permission()
    {
        var @operator = CreateOperator();

        Assert.False(@operator.Has(Permission.EnterResults));
        Assert.Empty(@operator.Permissions);
    }

    [Fact]
    public void The_attendance_start_clears_the_previous_session()
    {
        var @operator = CreateOperator();
        var start = new DateTimeOffset(2026, 3, 1, 8, 0, 0, TimeSpan.Zero);

        @operator.RecordAttendanceStart(start);

        Assert.Equal(start, @operator.AttendanceStart);
        Assert.Null(@operator.AttendanceEnd);
        Assert.False(@operator.IsOnBreak);
    }

    [Fact]
    public void A_break_can_be_started_and_ended()
    {
        var @operator = CreateOperator();
        var moment = new DateTimeOffset(2026, 3, 1, 12, 0, 0, TimeSpan.Zero);

        @operator.RecordBreakStart(moment);
        Assert.True(@operator.IsOnBreak);
        Assert.Equal(moment, @operator.BreakStartedAt);

        @operator.RecordBreakEnd();
        Assert.False(@operator.IsOnBreak);
        Assert.Null(@operator.BreakStartedAt);
    }

    [Fact]
    public void The_password_hash_can_be_replaced_but_never_cleared()
    {
        var @operator = CreateOperator();

        @operator.SetPasswordHash("new-hash");
        Assert.Equal("new-hash", @operator.PasswordHash);

        Assert.Throws<InvalidOperatorException>(() => @operator.SetPasswordHash("  "));
    }

    [Fact]
    public void The_security_word_is_stored_as_a_hash_only()
    {
        var @operator = CreateOperator();

        @operator.SetSecurityWordHash("hashed-secret");

        Assert.Equal("hashed-secret", @operator.SecurityWordHash);

        @operator.SetSecurityWordHash(null);
        Assert.Null(@operator.SecurityWordHash);
    }
}
