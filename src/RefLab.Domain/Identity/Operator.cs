using RefLab.Domain.Exceptions;

namespace RefLab.Domain.Identity;

/// <summary>
/// A laboratory operator — the laboratory user account.
/// </summary>
/// <remarks>
/// <para>
/// Legacy evidence: the table named <c>patients</c> in the legacy database
/// holds <em>operators</em>, not patients. It carries 33 columns:
/// <c>patientusid</c>, <c>patientusname</c>, <c>patientuscap</c>, the clear-text
/// password <c>patientword</c>, the administrator flag <c>patientad</c>, the
/// active flag <c>patientus</c>, the seventeen permission bits
/// <c>grant1</c> to <c>grant17</c>, the attendance fields
/// <c>startwork</c>, <c>endwork</c>, <c>breakyorn</c> and <c>breaktime</c>, the
/// security word <c>SecWord</c>.
/// </para>
/// <para>
/// The name of the legacy table is a relic of the pre-rebranding product and
/// would mis-map the whole schema. Ref Lab calls the concept
/// <see cref="Operator"/>.
/// </para>
/// </remarks>
public sealed class Operator
{
    private Permission[] _permissions = Array.Empty<Permission>();

    private Operator()
    {
    }

    /// <summary>The operator identifier.</summary>
    public int Id { get; private set; }

    /// <summary>The operator name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The role caption shown in the interface.</summary>
    public string? RoleCaption { get; private set; }

    /// <summary>The stored password hash. The clear-text password is never kept.</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>Whether the operator is an administrator.</summary>
    public bool IsAdministrator { get; private set; }

    /// <summary>Whether the operator account is active.</summary>
    public bool IsActive { get; private set; }

    /// <summary>The stored hash of the operator's security word.</summary>
    public string? SecurityWordHash { get; private set; }

    /// <summary>Whether the tip panel is shown to this operator.</summary>
    public bool ShowTip { get; private set; }

    /// <summary>The index of the tip currently shown.</summary>
    public int TipNumber { get; private set; }

    /// <summary>The special limit applied to this operator.</summary>
    public int SpecialLimit { get; private set; }

    /// <summary>The date and time the operator started work, when present.</summary>
    public DateTimeOffset? AttendanceStart { get; private set; }

    /// <summary>The date and time the operator ended work, when present.</summary>
    public DateTimeOffset? AttendanceEnd { get; private set; }

    /// <summary>Whether the operator is currently on a break.</summary>
    public bool IsOnBreak { get; private set; }

    /// <summary>The date and time the operator's break started, when present.</summary>
    public DateTimeOffset? BreakStartedAt { get; private set; }

    /// <summary>The date and time of the operator's last visit.</summary>
    public DateTimeOffset? LastVisit { get; private set; }

    /// <summary>The function-level permissions granted to the operator.</summary>
    /// <remarks>
    /// <para>
    /// The collection is replaced — never mutated in place — whenever a
    /// permission is granted or revoked. The persistence layer maps the
    /// collection onto a single column and detects a change by comparing the
    /// stored value with the current one; mutating the same instance in place
    /// would leave the comparison blind to the change.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<Permission> Permissions
    {
        get => _permissions;
        private set => _permissions = value?.ToArray() ?? Array.Empty<Permission>();
    }

    /// <summary>Creates an operator.</summary>
    /// <exception cref="InvalidOperatorException">
    /// The name is empty or the password hash is empty.
    /// </exception>
    public static Operator Create(int id, string name, string passwordHash, string? roleCaption = null)
    {
        if (id <= 0)
        {
            throw new InvalidOperatorException("An operator identifier must be a positive integer.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperatorException("An operator must have a name.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new InvalidOperatorException("An operator must have a stored password hash.");
        }

        return new Operator
        {
            Id = id,
            Name = name.Trim(),
            RoleCaption = string.IsNullOrWhiteSpace(roleCaption) ? null : roleCaption.Trim(),
            PasswordHash = passwordHash,
            IsActive = true,
        };
    }

    /// <summary>Grants a permission to the operator.</summary>
    public void Grant(Permission permission)
    {
        if (_permissions.Contains(permission))
        {
            return;
        }

        _permissions = _permissions.Append(permission).ToArray();
    }

    /// <summary>Revokes a permission from the operator.</summary>
    public void Revoke(Permission permission) =>
        _permissions = _permissions.Where(held => held != permission).ToArray();

    /// <summary>Indicates whether the operator holds a permission.</summary>
    public bool Has(Permission permission) => IsAdministrator || _permissions.Contains(permission);

    /// <summary>Replaces the stored password hash.</summary>
    /// <remarks>
    /// The caller validates the clear-text password against
    /// <see cref="PasswordPolicy"/> and hashes it before calling this method.
    /// The domain never sees a clear-text password.
    /// </remarks>
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new InvalidOperatorException("An operator must have a stored password hash.");
        }

        PasswordHash = passwordHash;
    }

    /// <summary>Stores the hash of the operator's security word.</summary>
    public void SetSecurityWordHash(string? securityWordHash) =>
        SecurityWordHash = string.IsNullOrWhiteSpace(securityWordHash) ? null : securityWordHash;

    /// <summary>Promotes the operator to administrator, or demotes it.</summary>
    public void SetAdministrator(bool isAdministrator) => IsAdministrator = isAdministrator;

    /// <summary>Activates or deactivates the operator account.</summary>
    public void SetActive(bool isActive) => IsActive = isActive;

    /// <summary>Renames the operator.</summary>
    /// <exception cref="InvalidOperatorException">The name is empty.</exception>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperatorException("An operator must have a name.");
        }

        Name = name.Trim();
    }

    /// <summary>Sets the role caption.</summary>
    public void SetRoleCaption(string? roleCaption) =>
        RoleCaption = string.IsNullOrWhiteSpace(roleCaption) ? null : roleCaption.Trim();

    /// <summary>Records the start of the working session.</summary>
    public void RecordAttendanceStart(DateTimeOffset at)
    {
        AttendanceStart = at;
        AttendanceEnd = null;
        IsOnBreak = false;
        BreakStartedAt = null;
    }

    /// <summary>Records the end of the working session.</summary>
    public void RecordAttendanceEnd(DateTimeOffset at)
    {
        AttendanceEnd = at;
        IsOnBreak = false;
        BreakStartedAt = null;
    }

    /// <summary>Records the start of a break.</summary>
    public void RecordBreakStart(DateTimeOffset at)
    {
        IsOnBreak = true;
        BreakStartedAt = at;
    }

    /// <summary>Records the end of a break.</summary>
    public void RecordBreakEnd()
    {
        IsOnBreak = false;
        BreakStartedAt = null;
    }

    /// <summary>Records the date and time of the operator's last visit.</summary>
    public void RecordLastVisit(DateTimeOffset at) => LastVisit = at;
}
