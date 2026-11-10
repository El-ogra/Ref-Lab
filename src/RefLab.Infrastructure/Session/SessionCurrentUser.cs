using RefLab.Domain.Identity;

namespace RefLab.Infrastructure.Session;

/// <summary>
/// The signed-in operator of the current desktop session.
/// </summary>
/// <remarks>
/// The laboratory is a single-workstation desktop application, so the session
/// is held in the process. The presentation layer sets the operator at sign-in
/// and clears it at sign-out.
/// </remarks>
public sealed class SessionCurrentUser : Application.Abstractions.ICurrentUser
{
    private int? _operatorId;
    private string? _operatorName;
    private readonly HashSet<Permission> _permissions = new();

    /// <inheritdoc />
    public int? OperatorId => _operatorId;

    /// <inheritdoc />
    public string? OperatorName => _operatorName;

    /// <inheritdoc />
    public bool IsAuthenticated => _operatorId.HasValue;

    /// <inheritdoc />
    public bool HasPermission(Permission permission) => _permissions.Contains(permission);

    /// <summary>Establishes the session for an operator.</summary>
    public void SignIn(Operator @operator)
    {
        ArgumentNullException.ThrowIfNull(@operator);

        _operatorId = @operator.Id;
        _operatorName = @operator.Name;
        _permissions.Clear();

        foreach (var permission in @operator.Permissions)
        {
            _permissions.Add(permission);
        }
    }

    /// <summary>Ends the session.</summary>
    public void SignOut()
    {
        _operatorId = null;
        _operatorName = null;
        _permissions.Clear();
    }
}
