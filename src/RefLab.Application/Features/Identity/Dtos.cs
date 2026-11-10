using RefLab.Domain.Identity;

namespace RefLab.Application.Features.Identity;

/// <summary>
/// An operator account as presented to the application layer.
/// </summary>
public sealed record OperatorDto(
    int Id,
    string Name,
    string? RoleCaption,
    bool IsAdministrator,
    bool IsActive,
    IReadOnlyList<string> Permissions)
{
    /// <summary>Projects an operator onto its DTO.</summary>
    public static OperatorDto FromDomain(Operator @operator) =>
        new(
            @operator.Id,
            @operator.Name,
            @operator.RoleCaption,
            @operator.IsAdministrator,
            @operator.IsActive,
            @operator.Permissions.Select(permission => permission.ToString()).OrderBy(value => value).ToArray());
}

/// <summary>
/// The outcome of a successful sign-in.
/// </summary>
public sealed record SignInResultDto(OperatorDto Operator, DateTimeOffset SignedInAt);
