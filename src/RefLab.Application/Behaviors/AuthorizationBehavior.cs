using MediatR;
using RefLab.Application.Abstractions;
using RefLab.Application.Common;
using RefLab.Domain.Identity;

namespace RefLab.Application.Behaviors;

/// <summary>
/// Requires the signed-in operator to hold a specific permission before the
/// request is handled.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUser _currentUser;

    /// <summary>Initialises a new behaviour.</summary>
    public AuthorizationBehavior(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        if (request is IRequirePermission requirement && !_currentUser.HasPermission(requirement.Permission))
        {
            throw new ForbiddenException(
                $"The operation requires the '{requirement.Permission}' permission.");
        }

        return await next();
    }
}

/// <summary>
/// Implemented by a request that needs a specific function-level permission.
/// </summary>
public interface IRequirePermission
{
    /// <summary>The permission the signed-in operator must hold.</summary>
    Permission Permission { get; }
}
