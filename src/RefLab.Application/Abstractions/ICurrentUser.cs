using RefLab.Domain.Identity;

namespace RefLab.Application.Abstractions;

/// <summary>
/// The operator whose session is currently active.
/// </summary>
public interface ICurrentUser
{
    /// <summary>The identifier of the signed-in operator, when there is one.</summary>
    int? OperatorId { get; }

    /// <summary>The name of the signed-in operator, when there is one.</summary>
    string? OperatorName { get; }

    /// <summary>Whether an operator is signed in.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Indicates whether the signed-in operator holds a permission.</summary>
    bool HasPermission(Permission permission);
}
