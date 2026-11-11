using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Abstractions;
using RefLab.Presentation.Common;
using RefLab.Presentation.Navigation;
using RefLab.Infrastructure.Session;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the application shell.
/// </summary>
/// <remarks>
/// <para>
/// The shell owns the module toolbar, the operator identity strip and the
/// navigation region. It holds no business logic: it forwards navigation
/// requests to the navigation service and reports the state of the signed-in
/// operator.
/// </para>
/// <para>
/// The session is cleared through <see cref="SessionCurrentUser"/>, the
/// concrete singleton that implements <see cref="ICurrentUser"/>. The
/// Application layer's abstraction exposes no sign-out operation, and the
/// Application layer is a completed layer that Phase 5 does not modify, so
/// the concrete type is used for exactly this one call.
/// </para>
/// </remarks>
public sealed partial class ShellViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private readonly SessionCurrentUser _session;
    private readonly ICurrentUser _currentUser;

    /// <summary>Initialises a new shell view model.</summary>
    public ShellViewModel(
        RequestExecutor executor,
        INavigationService navigation,
        SessionCurrentUser session,
        ICurrentUser currentUser)
        : base(executor)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

        _navigation.PropertyChanged += OnNavigationChanged;
    }

    /// <inheritdoc />
    public override string Title => "Ref Lab";

    /// <summary>The view of the current screen.</summary>
    public object? CurrentView => _navigation.CurrentView;

    /// <summary>The name of the signed-in operator, when there is one.</summary>
    public string OperatorName => _currentUser.OperatorName ?? string.Empty;

    /// <summary>Whether an operator is signed in.</summary>
    public bool IsAuthenticated => _currentUser.IsAuthenticated;

    /// <summary>
    /// The moment the operator signed in, as it is shown in the status strip.
    /// </summary>
    public string LastSignInText => _session.SignedInAt is null
        ? string.Empty
        : _session.SignedInAt.Value.ToLocalTime().ToString("ddd, dd MMM yyyy HH:mm", CultureInfo.CurrentCulture);

    /// <summary>
    /// The workstation's current date, as it is shown in the status strip.
    /// </summary>
    /// <remarks>
    /// This is the date on the operator's own workstation, which is what the
    /// laboratory's status strip shows. It is a display value rather than a
    /// recorded fact, so it is read from the workstation clock rather than
    /// from the laboratory's own clock.
    /// </remarks>
    public string TodayText => DateTime.Now.ToString("ddd, dd-MMM-yyyy HH:mm", CultureInfo.CurrentCulture);

    /// <summary>The key of the current screen.</summary>
    public string? CurrentScreenKey => _navigation.CurrentScreenKey;

    /// <summary>Navigates to a screen.</summary>
    /// <param name="screenKey">The key of the target screen.</param>
    [RelayCommand]
    private void Navigate(string? screenKey)
    {
        if (string.IsNullOrWhiteSpace(screenKey))
        {
            return;
        }

        _navigation.NavigateTo(screenKey);
    }

    /// <summary>Returns to the previously visited screen.</summary>
    [RelayCommand]
    private void Back() => _navigation.GoBack();

    /// <summary>Ends the session and returns to the sign-in screen.</summary>
    [RelayCommand]
    private void SignOut()
    {
        _session.SignOut();

        _navigation.NavigateTo(ScreenKeys.SignIn);

        OnPropertyChanged(nameof(OperatorName));
        OnPropertyChanged(nameof(IsAuthenticated));
    }

    /// <summary>Called by the sign-in screen once the session has been established.</summary>
    public void OnSessionEstablished()
    {
        OnPropertyChanged(nameof(OperatorName));
        OnPropertyChanged(nameof(IsAuthenticated));
    }

    private void OnNavigationChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CurrentView));
        OnPropertyChanged(nameof(CurrentScreenKey));
    }
}
