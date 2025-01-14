﻿using BlazorApp.Client.ApiClient;
using BlazorApp.Client.Authentication;
using BlazorApp.Client.Authorization;
using BlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorApp.Client.Pages.Personal;
public partial class Profile
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthenticationService AuthService { get; set; } = default!;
    [Inject]
    protected IIdentityClient IdentityClient { get; set; } = default!;

    private readonly UpdateProfileRequest _profileModel = new();

    private string? _imageUrl;
    private string? _userId;
    private char _firstLetterOfName;

    private CustomValidation? _customValidation;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        var user = state.User;
        if (user is not null)
        {
            _profileModel.Email = user.GetEmail() ?? string.Empty;
            _profileModel.FirstName = user.GetFirstName() ?? string.Empty;
            _profileModel.LastName = user.GetSurname() ?? string.Empty;
            _profileModel.PhoneNumber = user.GetPhoneNumber();
            _imageUrl = user?.GetImageUrl()?.Replace("{server_url}/", _hostEnvironment.BaseAddress);
            _userId = user?.GetUserId();
        }

        if (_profileModel.FirstName?.Length > 0)
        {
            _firstLetterOfName = _profileModel.FirstName[0];
        }
    }

    private async Task UpdateProfileAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => IdentityClient.UpdateProfileAsync(_profileModel), _snackBar, _customValidation)
            is Result result && result.Succeeded)
        {
            _snackBar.Add("Your Profile has been updated. Please Login again to Continue.", Severity.Success);
            await AuthService.ReLoginAsync(_navigationManager.Uri);
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file is not null)
        {
            var supportedFormats = new List<string> { ".jpeg", ".jpg", ".png" };
            string? extension = Path.GetExtension(file.Name);
            if (!supportedFormats.Contains(extension.ToLower()))
            {
                _snackBar.Add("File Format Not Supported.", Severity.Error);
                return;
            }

            string? fileName = $"{_userId}-{Guid.NewGuid().ToString("N")}";
            fileName = fileName.Substring(0, Math.Min(fileName.Length, 90));
            string? format = "image/png";
            var imageFile = await file.RequestImageFileAsync(format, 400, 400);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            string? base64String = $"data:image/png;base64,{Convert.ToBase64String(buffer)}";
            _profileModel.Image = new FileUploadRequest() { Name = fileName, Data = base64String, Extension = extension };

            await UpdateProfileAsync();
        }
    }
}