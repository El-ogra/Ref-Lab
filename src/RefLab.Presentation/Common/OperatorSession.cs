using RefLab.Application.Features.Identity;
using RefLab.Domain.Identity;
using RefLab.Infrastructure.Session;

namespace RefLab.Presentation.Common;

/// <summary>
/// Establishes and clears the session of the signed-in operator.
/// </summary>
/// <remarks>
/// <para>
/// The session lives in <see cref="SessionCurrentUser"/>, the singleton that
/// implements <see cref="RefLab.Application.Abstractions.ICurrentUser"/> and
/// that the Application layer's authorisation behaviour consults before every
/// permission-gated request. The presentation layer therefore has to
/// establish the session itself: the authorisation behaviour refuses a
/// permission-gated command when the session holds no permissions.
/// </para>
/// <para>
/// The Application layer returns an operator DTO and never exposes the stored
/// password hash, which is correct: the hash must not leave the domain. The
/// session singleton reads only the identifier, the name and the granted
/// permissions from the operator, so the principal is rehydrated here with a
/// placeholder hash that is never stored, never verified and never leaves
/// this process.
/// </para>
/// <para>
/// Only the permissions the DTO carries are granted. That reproduces the
/// behaviour the infrastructure test suite already pins down: the session
/// reports the permissions that were granted explicitly, and it does not
/// widen them.
/// </para>
/// </remarks>
public sealed class OperatorSession
{
    /// <summary>
    /// The placeholder hash of the rehydrated principal. It is never stored,
    /// never compared with a password and never leaves the process.
    /// </summary>
    private const string PrincipalHashPlaceholder = "session-principal";

    private readonly SessionCurrentUser _session;

    /// <summary>Initialises a new session helper.</summary>
    public OperatorSession(SessionCurrentUser session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <summary>Establishes the session for the signed-in operator.</summary>
    /// <param name="operator">The operator returned by the sign-in use case.</param>
    public void Establish(OperatorDto @operator)
    {
        ArgumentNullException.ThrowIfNull(@operator);

        var principal = Operator.Create(
            @operator.Id,
            @operator.Name,
            PrincipalHashPlaceholder,
            @operator.RoleCaption);

        principal.SetAdministrator(@operator.IsAdministrator);
        principal.SetActive(@operator.IsActive);

        foreach (var name in @operator.Permissions)
        {
            if (Enum.TryParse<Permission>(name, ignoreCase: false, out var permission))
            {
                principal.Grant(permission);
            }
        }

        _session.SignIn(principal);
    }

    /// <summary>Ends the session.</summary>
    public void Clear() => _session.SignOut();
}
