using BlazorApp.Client.ApiClient;
using BlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp.Client.Pages.Personal;

public partial class Security
{
    [Inject]
    public IIdentityClient IdentityClient { get; set; } = default!;

    private readonly ChangePasswordRequest _passwordModel = new();

    private CustomValidation? _customValidation;

    private async Task ChangePasswordAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => IdentityClient.ChangePasswordAsync(_passwordModel), _snackBar, _customValidation,"Password Changed")
            is Result result && result.Succeeded)
        {
            _passwordModel.Password = string.Empty;
            _passwordModel.NewPassword = string.Empty;
            _passwordModel.ConfirmNewPassword = string.Empty;
        }
    }

    private InputType _currentPasswordInput = InputType.Password;
    private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _newPasswordVisibility;
    private InputType _newPasswordInput = InputType.Password;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility(bool newPassword)
    {
        if (newPassword)
        {
            if (_newPasswordVisibility)
            {
                _newPasswordVisibility = false;
                _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _newPasswordInput = InputType.Password;
            }
            else
            {
                _newPasswordVisibility = true;
                _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                _newPasswordInput = InputType.Text;
            }
        }
    }
}