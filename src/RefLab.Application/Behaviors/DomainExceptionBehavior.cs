using MediatR;
using RefLab.Application.Common;
using RefLab.Domain.Exceptions;

namespace RefLab.Application.Behaviors;

/// <summary>
/// Translates a domain-invariant failure into the application-layer exception
/// contract, so that the presentation layer never sees a domain exception.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class DomainExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            return await next();
        }
        catch (DomainException exception)
        {
            throw new ConflictException(exception.Message);
        }
    }
}
