namespace RefLab.Presentation.Navigation;

/// <summary>
/// The table that maps a screen key onto the type of its view.
/// </summary>
/// <remarks>
/// The table is the single navigation map of the application. A screen is
/// added by registering its view here and in the dependency-injection
/// container; nothing else has to change.
/// </remarks>
public static class ScreenRegistry
{
    /// <summary>The view type of each screen.</summary>
    public static IReadOnlyDictionary<string, Type> ViewTypes { get; } =
        new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            [ScreenKeys.SignIn] = typeof(Views.SignInView),
            [ScreenKeys.PatientWork] = typeof(Views.PatientWorkView),
            [ScreenKeys.TestCatalogue] = typeof(Views.TestCatalogueView),
            [ScreenKeys.CatalogueAdministration] = typeof(Views.CatalogueAdministrationView),
            [ScreenKeys.SendOut] = typeof(Views.SendOutQueueView),
            [ScreenKeys.Microbiology] = typeof(Views.MicrobiologyView),
            [ScreenKeys.Accounting] = typeof(Views.AccountingView),
            [ScreenKeys.PartiesPricing] = typeof(Views.PartiesPricingView),
            [ScreenKeys.ReportSetup] = typeof(Views.ReportSetupView),
            [ScreenKeys.LaboratorySettings] = typeof(Views.LaboratorySettingsView),
            [ScreenKeys.OperatorAdministration] = typeof(Views.OperatorAdministrationView),
            [ScreenKeys.Statistics] = typeof(Views.StatisticsView),
            [ScreenKeys.Audit] = typeof(Views.AuditView),
        };
}
