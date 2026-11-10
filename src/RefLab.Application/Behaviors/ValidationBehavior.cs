using FluentValidation;
using MediatR;
using RefLab.Application.Common;

namespace RefLab.Application.Behaviors;

/// <summary>
/// Runs every FluentValidation validator registered for the request before the
/// handler is invoked.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>Initialises a new behaviour.</summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var validators = _validators.ToArray();

        if (validators.Length > 0)
        {
            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .Select(failure => failure.ErrorMessage)
                .ToArray();

            if (failures.Length > 0)
            {
                throw new RequestValidationException(failures);
            }
        }

        return await next();
    }
}
