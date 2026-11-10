using Microsoft.Extensions.DependencyInjection;

namespace RefLab.Presentation.Navigation;

/// <summary>
/// Resolves the view of a screen from the dependency-injection container.
/// </summary>
public sealed class ViewLocator : IViewLocator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>Initialises a new locator.</summary>
    public ViewLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public object? Locate(string screenKey)
    {
        if (string.IsNullOrWhiteSpace(screenKey))
        {
            return null;
        }

        var viewType = ScreenRegistry.ViewTypes.TryGetValue(screenKey, out var candidate) ? candidate : null;

        if (viewType is null)
        {
            return null;
        }

        return _serviceProvider.GetRequiredService(viewType);
    }
}
