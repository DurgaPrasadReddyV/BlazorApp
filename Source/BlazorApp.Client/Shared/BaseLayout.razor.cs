using BlazorApp.Client.Theme;
using MudBlazor;

namespace BlazorApp.Client.Shared;

public partial class BaseLayout
{
    private MudTheme _currentTheme = new LightTheme();

    protected override void OnInitialized()
    {
    }
}