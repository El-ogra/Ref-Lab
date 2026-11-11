using MediatR;

namespace RefLab.Presentation.Tests.Fakes;

/// <summary>
/// A mediator that answers with a prepared response and records the request
/// that was sent.
/// </summary>
/// <remarks>
/// The presentation layer reaches the Application layer exclusively through
/// MediatR, so a view-model test needs a mediator. This fake keeps the test
/// at the presentation level: the view model's own behaviour — the guards, the
/// messages, the navigation and the session — is what is under test, not the
/// use case behind it. The use case itself is covered by the integration test
/// project, against a real database.
/// </remarks>
public sealed class StubMediator : IMediator
{
    private readonly Func<IRequest<object>, Task<object?>> _responder;

    /// <summary>Initialises a new mediator.</summary>
    /// <param name="responder">
    /// The function that produces the response. It is asynchronous so that a
    /// test can hold a request open and observe what the view model does
    /// while the request is still in flight.
    /// </param>
    public StubMediator(Func<IRequest<object>, Task<object?>> responder)
    {
        _responder = responder ?? throw new ArgumentNullException(nameof(responder));
    }

    /// <summary>
    /// Creates a mediator that answers immediately.
    /// </summary>
    /// <param name="responder">The function that produces the response.</param>
    /// <returns>The mediator.</returns>
    public static StubMediator Immediate(Func<IRequest<object>, object?> responder)
    {
        ArgumentNullException.ThrowIfNull(responder);

        return new StubMediator(request => Task.FromResult(responder(request)));
    }

    /// <summary>The requests that were sent, in order.</summary>
    public List<object> Sent { get; } = new();

    /// <inheritdoc />
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Sent.Add(request);

        var response = await _responder((IRequest<object>)(object)request).ConfigureAwait(true);

        if (response is null)
        {
            return default(TResponse)!;
        }

        return (TResponse)response;
    }

    /// <inheritdoc />
    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        Sent.Add(request);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Sent.Add(request);

        return Task.FromResult<object?>(null);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default) =>
        Empty<TResponse>();

    /// <inheritdoc />
    public IAsyncEnumerable<object?> CreateStream(
        object request,
        CancellationToken cancellationToken = default) =>
        Empty<object?>();

    /// <inheritdoc />
    public Task Publish(object notification, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    /// <inheritdoc />
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification =>
        Task.CompletedTask;

    private static async IAsyncEnumerable<T> Empty<T>()
    {
        await Task.CompletedTask;

        yield break;
    }
}
