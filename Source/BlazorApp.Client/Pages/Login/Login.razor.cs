using BlazorApp.Client.ApiClient;
using BlazorApp.Client.Authentication;
using BlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp.Client.Pages.Login;

public partial class Login
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    public IAuthenticationService AuthService { get; set; } = default!;

    private CustomValidation? _customValidation;

    public bool BusySubmitting { get; set; } = false;

    private readonly TokenRequest _tokenRequest = new();
    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        if (authState.User.Identity?.IsAuthenticated is true)
        {
            _navigationManager.NavigateTo("/dashboard");
        }
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteCallGuardedAsync(
            () => AuthService.LoginAsync(_tokenRequest),
            _snackBar,
            _customValidation);

        BusySubmitting = false;
    }
}