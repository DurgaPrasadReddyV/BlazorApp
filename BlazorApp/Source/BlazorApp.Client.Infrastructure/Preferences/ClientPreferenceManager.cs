namespace BlazorApp.Client.Infrastructure.Preferences;

public class ClientPreferenceManager : IClientPreferenceManager
{
    private readonly ILocalStorageService _localStorageService;
    public static string Preference = "clientPreference";

    public ClientPreferenceManager(
        ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<bool> ToggleDrawerAsync()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.IsDrawerOpen = !preference.IsDrawerOpen;
            await SetPreference(preference);
            return preference.IsDrawerOpen;
        }

        return false;
    }

    public async Task<IPreference> GetPreference()
    {
        return await _localStorageService.GetItemAsync<ClientPreference>(Preference) ?? new ClientPreference();
    }

    public async Task SetPreference(IPreference preference)
    {
        await _localStorageService.SetItemAsync(Preference, preference as ClientPreference);
    }
}