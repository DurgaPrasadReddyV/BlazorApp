using BlazorApp.Client.ApiClient;
using BlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Pages.Login;

public partial class ForgotPassword
{
    private readonly ForgotPasswordRequest _forgotPasswordRequest = new();
    private CustomValidation? _customValidation;
    private bool BusySubmitting { get; set; }

    [Inject]
    private IIdentityClient _identityClient { get; set; } = default!;

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteCallGuardedAsync(
            () => _identityClient.ForgotPasswordAsync(_forgotPasswordRequest),
            _snackBar,
            _customValidation);

        BusySubmitting = false;
    }
}