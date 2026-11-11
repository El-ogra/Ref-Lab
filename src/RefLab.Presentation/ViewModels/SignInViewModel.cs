using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Identity;
using RefLab.Presentation.Common;
using RefLab.Presentation.Navigation;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the start-up and sign-in screen.
/// </summary>
/// <remarks>
/// The screen carries the operator name and the password, sends the sign-in
/// use case and, on success, establishes the session before moving on to the
/// patient work screen.
/// </remarks>
public sealed partial class SignInViewModel : ViewModelBase
{
    private readonly OperatorSession _session;
    private readonly INavigationService _navigation;
    private readonly ShellViewModel _shell;

    /// <summary>Initialises a new sign-in view model.</summary>
    public SignInViewModel(
        RequestExecutor executor,
        OperatorSession session,
        INavigationService navigation,
        ShellViewModel shell)
        : base(executor)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _shell = shell ?? throw new ArgumentNullException(nameof(shell));
    }

    /// <inheritdoc />
    public override string Title => "Sign-in";

    /// <summary>The operator name.</summary>
    [ObservableProperty]
    private string _operatorName = string.Empty;

    /// <summary>The password.</summary>
    [ObservableProperty]
    private string _password = string.Empty;

    /// <summary>Whether the password is currently revealed.</summary>
    [ObservableProperty]
    private bool _isPasswordVisible;

    /// <summary>The glyph shown on the password visibility toggle.</summary>
    public string PasswordVisibilityGlyph => IsPasswordVisible ? "🙈" : "👁";

    partial void OnIsPasswordVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(PasswordVisibilityGlyph));
    }

    /// <summary>Toggles password visibility.</summary>
    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    /// <summary>Signs the operator in.</summary>
    [RelayCommand]
    private async Task SignInAsync()
    {
        if (string.IsNullOrWhiteSpace(OperatorName))
        {
            ErrorMessage = "An operator name is required.";
            return;
        }

        var succeeded = await RunAsync(async () =>
        {
            var result = await _executor.SendAsync(new SignInCommand(OperatorName, Password));

            _session.Establish(result.Operator, result.SignedInAt);

            _shell.OnSessionEstablished();

            _navigation.NavigateTo(ScreenKeys.PatientWork);
        }).ConfigureAwait(true);

        if (succeeded)
        {
            Password = string.Empty;
            StatusMessage = "Signed in.";
        }
    }
}
