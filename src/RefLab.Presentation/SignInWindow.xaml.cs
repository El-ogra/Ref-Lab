using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RefLab.Presentation.Navigation;
using RefLab.Presentation.Views;

namespace RefLab.Presentation;

public partial class SignInWindow : Window
{
    private readonly IServiceProvider _services;
    private readonly INavigationService _navigation;
    private bool _transitioned;

    public SignInWindow(SignInView signInView, IServiceProvider services, INavigationService navigation)
    {
        ArgumentNullException.ThrowIfNull(signInView);
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(navigation);

        InitializeComponent();

        Content = signInView;
        _services = services;
        _navigation = navigation;

        _navigation.PropertyChanged += OnNavigationChanged;
        Closed += OnClosed;
    }

    private void OnNavigationChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_transitioned)
        {
            return;
        }

        if (!string.Equals(e.PropertyName, nameof(INavigationService.CurrentScreenKey), StringComparison.Ordinal))
        {
            return;
        }

        if (!string.Equals(_navigation.CurrentScreenKey, ScreenKeys.PatientWork, StringComparison.Ordinal))
        {
            return;
        }

        _transitioned = true;

        Dispatcher.Invoke(() =>
        {
            var mainWindow = _services.GetRequiredService<MainWindow>();
            System.Windows.Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            Close();
        });
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _navigation.PropertyChanged -= OnNavigationChanged;
    }
}