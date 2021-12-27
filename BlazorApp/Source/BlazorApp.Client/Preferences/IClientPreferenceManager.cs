namespace BlazorApp.Client.Preferences;

public interface IClientPreferenceManager : IPreferenceManager
{
    Task<bool> ToggleDrawerAsync();
}