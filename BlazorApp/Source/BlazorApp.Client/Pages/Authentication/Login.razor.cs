using BlazorApp.Client.Infrastructure.ApiClient;
using BlazorApp.Client.Infrastructure.Authentication;
using BlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BlazorApp.Client.Pages.Authentication;

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

    private void FillAdministratorCredentials()
    {
        _tokenRequest.Email = "admin@root.com";
        _tokenRequest.Password = "123Pa$$word!";
    }

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ExecuteApiCallAsync(() => AuthService.LoginAsync(_tokenRequest));

        BusySubmitting = false;
    }

    private async Task<T?> ExecuteApiCallAsync<T>(Func<Task<T>> call)
    where T : Result
    {
        _customValidation?.ClearErrors();
        try
        {
            var result = await call();

            if (result.Succeeded)
            {
                return result;
            }

            if (result.Messages is not null)
            {
                foreach (string message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            else
            {
                _snackBar.Add("Something went wrong!", Severity.Error);
            }
        }
        catch (ApiException<HttpValidationProblemDetails> ex)
        {
            _customValidation?.DisplayErrors(ex.Result.Errors);
        }
        catch (ApiException<ErrorResultOfString> ex)
        {
            _snackBar.Add(ex.Result.Exception, Severity.Error);
        }

        return default;
    }
}