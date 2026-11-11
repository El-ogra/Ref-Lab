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

    /// <summary>
    /// The moment the operator signed in, when the session was established with
    /// one.
    /// </summary>
    /// <remarks>
    /// The laboratory's own screens carry the moment of the last sign-in in
    /// their status strip, so that the operator can see how long the session
    /// has been open.
    /// </remarks>
    public DateTimeOffset? SignedInAt { get; private set; }

    /// <summary>Establishes the session for an operator.</summary>
    /// <param name="operator">The operator who signed in.</param>
    /// <param name="signedInAt">
    /// The moment the operator signed in, when the caller knows it.
    /// </param>
    public void SignIn(Operator @operator, DateTimeOffset? signedInAt = null)
    {
        ArgumentNullException.ThrowIfNull(@operator);

        _operatorId = @operator.Id;
        _operatorName = @operator.Name;
        SignedInAt = signedInAt;
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
        SignedInAt = null;
        _permissions.Clear();
    }
}
