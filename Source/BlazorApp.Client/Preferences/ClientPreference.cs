namespace BlazorApp.Client.Preferences;

public class ClientPreference : IPreference
{
    public bool IsDrawerOpen { get; set; } = false;
}