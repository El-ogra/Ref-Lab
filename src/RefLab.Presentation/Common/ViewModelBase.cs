using CommunityToolkit.Mvvm.ComponentModel;

namespace RefLab.Presentation.Common;

/// <summary>
/// The base class of every view model in the presentation layer.
/// </summary>
/// <remarks>
/// <para>
/// A view model holds observable state, sends MediatR requests and maps the
/// responses into display-ready shapes. It contains no business logic: every
/// value it displays is either copied from a DTO returned by a handler or is a
/// purely presentational value such as a filter string or a selected row.
/// </para>
/// <para>
/// The <see cref="RunAsync(Func{Task})"/> helper owns the busy flag and the
/// error message so that no individual command has to repeat the pattern.
/// </para>
/// </remarks>
public abstract partial class ViewModelBase : ObservableObject
{
    /// <summary>The request executor used to reach the Application layer.</summary>
    protected readonly RequestExecutor _executor;

    /// <summary>Initialises a new view model.</summary>
    /// <param name="executor">The request executor.</param>
    protected ViewModelBase(RequestExecutor executor)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    /// <summary>Whether an operation is currently running.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>The last error message, or <see langword="null"/> when there is none.</summary>
    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>A short status message shown after a successful operation.</summary>
    [ObservableProperty]
    private string? _statusMessage;

    /// <summary>The title of the screen.</summary>
    public virtual string Title => string.Empty;

    /// <summary>Runs an operation, maintaining the busy flag and the error message.</summary>
    /// <param name="operation">The operation to run.</param>
    /// <returns><see langword="true"/> when the operation completed successfully.</returns>
    protected async Task<bool> RunAsync(Func<Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (IsBusy)
        {
            return false;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await operation().ConfigureAwait(true);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception exception)
        {
            ErrorMessage = RequestExecutor.Describe(exception);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
