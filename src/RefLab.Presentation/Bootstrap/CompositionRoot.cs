using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RefLab.Application;
using RefLab.Infrastructure;
using RefLab.Presentation.Navigation;

namespace RefLab.Presentation.Bootstrap;

/// <summary>
/// The composition root of the application.
/// </summary>
/// <remarks>
/// <para>
/// The composition root is the only place in the solution where the four
/// layers are wired together. It builds a generic host, registers the
/// Application, Infrastructure and Presentation layers, and then shows the
/// WPF main window, which it resolves from the container so that the window
/// receives its view model through dependency injection.
/// </para>
/// <para>
/// ⚠ <b>The namespace <c>RefLab.Application</c> shadows the WPF type
/// <c>System.Windows.Application</c> inside this project.</b> Any unqualified
/// reference to <c>Application</c> in this file resolves to the namespace and
/// fails to compile with CS0118. The WPF type is therefore never referenced by
/// name here, and any new file in this project that needs it must use the
/// fully qualified <c>System.Windows.Application</c> form. This is a
/// project-wide rule, not a local precaution.
/// </para>
/// <para>
/// The database schema is never changed by the shell. The
/// <c>DatabaseInitializer</c> is registered by <c>AddPersistenceLayer</c> but
/// is deliberately never resolved: applying a migration to a laboratory
/// database is an owner-gated operation performed from the command line.
/// </para>
/// </remarks>
public static class CompositionRoot
{
    /// <summary>The name of the environment variable that carries the connection string.</summary>
    public const string ConnectionStringVariable = "REFLAB_CONNECTION_STRING";

    /// <summary>Builds the host and shows the main window.</summary>
    /// <param name="connectionString">
    /// The SQL Server connection string. When <see langword="null"/>, the
    /// <see cref="ConnectionStringVariable"/> environment variable is used.
    /// </param>
    /// <returns>The host that owns the application's services.</returns>
    public static IHost BuildHost(string? connectionString = null)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddApplicationLayer();
        builder.Services.AddInfrastructureLayer();
        builder.Services.AddPersistenceLayer(ResolveConnectionString(connectionString));
        builder.Services.AddPresentationLayer();

        return builder.Build();
    }

    /// <summary>Resolves the main window from the host and shows it.</summary>
    /// <param name="host">The host that owns the services.</param>
    public static void ShowMainWindow(IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        var window = host.Services.GetRequiredService<MainWindow>();

        window.Show();

        // The shell is the entry point of the application: it opens on the
        // sign-in screen, which moves on to the patient work screen once the
        // session has been established. The navigation happens here, after the
        // window is shown, so the sign-in view model can reach the shell.
        host.Services.GetRequiredService<INavigationService>().NavigateTo(ScreenKeys.SignIn);
    }

    private static string ResolveConnectionString(string? connectionString)
    {
        var resolved = connectionString;

        if (string.IsNullOrWhiteSpace(resolved))
        {
            resolved = Environment.GetEnvironmentVariable(ConnectionStringVariable);
        }

        if (string.IsNullOrWhiteSpace(resolved))
        {
            throw new InvalidOperationException(
                $"A SQL Server connection string is required. Set the {ConnectionStringVariable} " +
                "environment variable or pass the connection string explicitly.");
        }

        return resolved;
    }
}
