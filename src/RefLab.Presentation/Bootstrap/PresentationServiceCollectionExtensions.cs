using Microsoft.Extensions.DependencyInjection;
using RefLab.Presentation.Common;
using RefLab.Presentation.Navigation;
using RefLab.Presentation.ViewModels;
using RefLab.Presentation.ViewModels.Panels;
using RefLab.Presentation.Views;
using RefLab.Presentation.Views.Controls;
using RefLab.Presentation.Views.Panels;

namespace RefLab.Presentation.Bootstrap;

/// <summary>
/// Registers the presentation layer with the dependency-injection container.
/// </summary>
/// <remarks>
/// <para>
/// The presentation layer registers its own services only. The Application and
/// Infrastructure layers are registered by their own extension methods, which
/// the composition root calls; the presentation layer never reaches into
/// either of them.
/// </para>
/// <para>
/// Every view model and every view is registered as a transient service so
/// that a view model never outlives the screen it belongs to.
/// </para>
/// </remarks>
public static class PresentationServiceCollectionExtensions
{
    /// <summary>Registers the presentation layer.</summary>
    /// <param name="services">The container to register with.</param>
    public static IServiceCollection AddPresentationLayer(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IErrorPresenter, ErrorPresenter>();
        services.AddSingleton<RequestExecutor>();
        services.AddSingleton<OperatorSession>();
        services.AddSingleton<IViewLocator, ViewLocator>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<ShellViewModel>();
        services.AddTransient<MethodSheetPanelViewModel>();
        services.AddTransient<DerivationPanelViewModel>();
        services.AddTransient<SignInViewModel>();
        services.AddTransient<PatientWorkViewModel>();
        services.AddTransient<TestCatalogueViewModel>();
        services.AddTransient<CatalogueAdministrationViewModel>();
        services.AddTransient<SendOutQueueViewModel>();
        services.AddTransient<MicrobiologyViewModel>();
        services.AddTransient<AccountingViewModel>();
        services.AddTransient<PartiesPricingViewModel>();
        services.AddTransient<ReportSetupViewModel>();
        services.AddTransient<LaboratorySettingsViewModel>();
        services.AddTransient<OperatorAdministrationViewModel>();
        services.AddTransient<StatisticsViewModel>();
        services.AddTransient<AuditViewModel>();
        services.AddTransient<ReceptionViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<SignInWindow>();
        services.AddTransient<SignInView>();
        services.AddTransient<PatientWorkView>();
        services.AddTransient<TestCatalogueView>();
        services.AddTransient<CatalogueAdministrationView>();
        services.AddTransient<SendOutQueueView>();
        services.AddTransient<MicrobiologyView>();
        services.AddTransient<AccountingView>();
        services.AddTransient<PartiesPricingView>();
        services.AddTransient<ReportSetupView>();
        services.AddTransient<LaboratorySettingsView>();
        services.AddTransient<OperatorAdministrationView>();
        services.AddTransient<StatisticsView>();
        services.AddTransient<AuditView>();
        services.AddTransient<ReceptionView>();
        services.AddTransient<MethodSheetPanelView>();
        services.AddTransient<DerivationPanelView>();

        return services;
    }
}
