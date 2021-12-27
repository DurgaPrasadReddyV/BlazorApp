using MudBlazor;

namespace BlazorApp.Client.Infrastructure.Preferences;

public interface IClientPreferenceManager : IPreferenceManager
{
    Task<bool> ToggleDrawerAsync();
}