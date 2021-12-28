using MudBlazor;

namespace BlazorApp.Client.Theme;

public class LightTheme : MudTheme
{
    public LightTheme()
    {
        Palette = new Palette()
        {
            Primary = CustomColors.Light.Primary,
            Secondary = CustomColors.Light.Secondary,
            Tertiary = CustomColors.Light.Tertiary,
            Background = CustomColors.Light.Background,
            AppbarBackground = CustomColors.Light.AppbarBackground,
            DrawerBackground = CustomColors.Light.DrawerBackground,
            AppbarText = CustomColors.Light.AppbarText,
            DrawerText = CustomColors.Light.DrawerText,
            TableLines = "#e0e0e029",
            OverlayDark = "hsl(0deg 0% 0% / 75%)"
        };
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "5px"
        };

        Typography = CustomTypography.FSHTypography;
        Shadows = new Shadow();
        ZIndex = new ZIndex() { Drawer = 1300 };
    }
}