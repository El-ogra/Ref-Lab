using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RefLab.Presentation.Bootstrap;

namespace RefLab.Presentation;

/// <summary>
/// The WPF application.
/// </summary>
/// <remarks>
/// <para>
/// ⚠ <b>The namespace <c>RefLab.Application</c> shadows the WPF type
/// <c>System.Windows.Application</c> inside this project.</b> Any unqualified
/// reference to <c>Application</c> in this file would resolve to the namespace
/// and fail to compile with CS0118, which
/// <c>TreatWarningsAsErrors=true</c> turns into a build break. The type is
/// therefore always written in its fully qualified form.
/// </para>
/// <para>
/// The application owns the generic host. The host builds the four layers and
/// the main window is resolved from it, so the window and its view model are
/// created through dependency injection.
/// </para>
/// </remarks>
public partial class App : System.Windows.Application
{
    private IHost? _host;

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = CompositionRoot.BuildHost();

        var signInWindow = _host.Services.GetRequiredService<SignInWindow>();
        System.Windows.Application.Current.MainWindow = signInWindow;
        signInWindow.Show();
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        _host = null;

        base.OnExit(e);
    }
}
