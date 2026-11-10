using MediatR;
using RefLab.Application.Common;
using RefLab.Domain.Exceptions;

namespace RefLab.Presentation.Common;

/// <summary>
/// Sends requests to the Application layer and translates the Application
/// exception hierarchy into operator-visible outcomes.
/// </summary>
/// <remarks>
/// <para>
/// The presentation layer communicates with the Application layer exclusively
/// through MediatR. This executor is the single place where that channel is
/// used, so the translation of the exception hierarchy is applied uniformly
/// and no view model has to know about the Application layer's exception
/// types.
/// </para>
/// <para>
/// A domain exception must never reach the operator: the
/// <c>DomainExceptionBehavior</c> pipeline behaviour translates it into a
/// <see cref="ConflictException"/> before it leaves the Application layer. The
/// handler for <see cref="DomainException"/> below is a defensive backstop
/// only.
/// </para>
/// </remarks>
public sealed class RequestExecutor
{
    private readonly IMediator _mediator;

    /// <summary>Initialises a new executor.</summary>
    public RequestExecutor(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>Sends a request and awaits its response.</summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="UiValidationException">The request failed validation.</exception>
    /// <exception cref="UiOperationException">The operation failed.</exception>
    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            return await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
        }
        catch (RequestValidationException exception)
        {
            throw new UiValidationException(exception.Errors);
        }
        catch (DomainException exception)
        {
            throw new UiOperationException(exception.Message, exception);
        }
    }

    /// <summary>Describes an exception as an operator-facing message.</summary>
    /// <param name="exception">The exception to describe.</param>
    public static string Describe(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception switch
        {
            UiValidationException validation => validation.Errors.Count == 0
                ? "The request is not valid."
                : string.Join(" ", validation.Errors),
            NotFoundException notFound => $"{notFound.EntityName} was not found.",
            ForbiddenException => "The signed-in operator is not permitted to perform this action.",
            ConflictException conflict => conflict.Message,
            ConcurrencyException => "The record was modified by another user. Reload and try again.",
            UiOperationException operation => operation.Message,
            _ => "The operation could not be completed.",
        };
    }
}
