using BlazorApp.Client.Infrastructure.Notifications;
using BlazorApp.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp.Client.Shared;

public partial class MainLayout
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Inject]
    public NotificationClient Notifications { get; set; } = default!;

    private bool _drawerOpen = false;

    protected override async Task OnInitializedAsync()
    {
        Notifications.TryConnectAsync();

        if (await _clientPreferenceManager.GetPreference() is ClientPreference preference)
        {
            _drawerOpen = preference.IsDrawerOpen;
        }
    }

    private async Task DrawerToggle()
    {
        _drawerOpen = await _clientPreferenceManager.ToggleDrawerAsync();
    }

    private void Logout()
    {
        var parameters = new DialogParameters
            {
                { nameof(Dialogs.Logout.ContentText), $"Logout Confirmation"},
                { nameof(Dialogs.Logout.ButtonText), $"Logout"},
                { nameof(Dialogs.Logout.Color), Color.Error}
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        _dialogService.Show<Dialogs.Logout>("Logout", parameters, options);
    }

    private void Profile()
    {
        _navigationManager.NavigateTo("/account");
    }
}