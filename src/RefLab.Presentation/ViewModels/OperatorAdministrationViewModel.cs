using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RefLab.Application.Features.Identity;
using RefLab.Presentation.Common;

namespace RefLab.Presentation.ViewModels;

/// <summary>
/// The view model of the operator administration screen.
/// </summary>
public sealed partial class OperatorAdministrationViewModel : ViewModelBase
{
    /// <summary>Initialises a new view model.</summary>
    public OperatorAdministrationViewModel(RequestExecutor executor)
        : base(executor)
    {
    }

    /// <inheritdoc />
    public override string Title => "Operator administration";

    /// <summary>The operators.</summary>
    public ObservableCollection<OperatorDto> Operators { get; } = new();

    /// <summary>The permissions the laboratory defines.</summary>
    public IReadOnlyList<string> AvailablePermissions { get; } = Enum
        .GetNames<RefLab.Domain.Identity.Permission>()
        .OrderBy(name => name, StringComparer.Ordinal)
        .ToArray();

    /// <summary>The operator being maintained.</summary>
    [ObservableProperty]
    private OperatorDto? _selectedOperator;

    /// <summary>The name of the operator being created.</summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>The password of the operator being created.</summary>
    [ObservableProperty]
    private string _password = string.Empty;

    /// <summary>The current password of the operator changing the password.</summary>
    [ObservableProperty]
    private string _currentPassword = string.Empty;

    /// <summary>The new password of the operator changing the password.</summary>
    [ObservableProperty]
    private string _newPassword = string.Empty;

    /// <summary>The role caption of the operator being created.</summary>
    [ObservableProperty]
    private string? _roleCaption;

    /// <summary>Whether the operator being created is an administrator.</summary>
    [ObservableProperty]
    private bool _isAdministrator;

    /// <summary>Whether the operator account is active.</summary>
    [ObservableProperty]
    private bool _isActive = true;

    /// <summary>The permissions granted to the operator.</summary>
    public ObservableCollection<string> Permissions { get; } = new();

    /// <summary>Reloads the operators.</summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            var operators = await _executor.SendAsync(new GetOperatorsQuery());

            Operators.Clear();

            foreach (var @operator in operators)
            {
                Operators.Add(@operator);
            }

            StatusMessage = $"{operators.Count} operator(s).";
        }).ConfigureAwait(true);
    }

    /// <summary>Creates an operator.</summary>
    [RelayCommand]
    private async Task CreateAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "A name and a password are required.";
            return;
        }

        await RunAsync(async () =>
        {
            var created = await _executor.SendAsync(new CreateOperatorCommand(
                Name,
                Password,
                RoleCaption,
                IsAdministrator,
                Permissions.ToArray()));

            Operators.Add(created);

            StatusMessage = $"The operator {created.Name} was created.";
        }).ConfigureAwait(true);
    }

    /// <summary>Updates the selected operator.</summary>
    [RelayCommand]
    private async Task UpdateAsync()
    {
        if (SelectedOperator is null)
        {
            ErrorMessage = "Select an operator first.";
            return;
        }

        await RunAsync(async () =>
        {
            var updated = await _executor.SendAsync(new UpdateOperatorCommand(
                SelectedOperator.Id,
                Name.Length == 0 ? SelectedOperator.Name : Name,
                RoleCaption,
                IsActive));

            Replace(updated);

            StatusMessage = $"The operator {updated.Name} was updated.";
        }).ConfigureAwait(true);
    }

    /// <summary>Assigns the selected permissions to the selected operator.</summary>
    [RelayCommand]
    private async Task AssignPermissionsAsync()
    {
        if (SelectedOperator is null)
        {
            ErrorMessage = "Select an operator first.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new AssignOperatorPermissionsCommand(
                SelectedOperator.Id,
                Permissions.ToArray()));

            await LoadAsync().ConfigureAwait(true);

            StatusMessage = "The permissions were assigned.";
        }).ConfigureAwait(true);
    }

    /// <summary>Changes the password of the selected operator.</summary>
    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (SelectedOperator is null)
        {
            ErrorMessage = "Select an operator first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "The current password and the new password are required.";
            return;
        }

        await RunAsync(async () =>
        {
            await _executor.SendAsync(new ChangePasswordCommand(
                SelectedOperator.Id,
                CurrentPassword,
                NewPassword));

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;

            StatusMessage = "The password was changed.";
        }).ConfigureAwait(true);
    }

    private void Replace(OperatorDto updated)
    {
        for (var index = 0; index < Operators.Count; index++)
        {
            if (Operators[index].Id == updated.Id)
            {
                Operators[index] = updated;
                return;
            }
        }
    }
}
