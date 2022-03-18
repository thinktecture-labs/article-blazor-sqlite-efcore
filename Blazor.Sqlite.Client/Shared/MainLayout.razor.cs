using MudBlazor;

namespace Blazor.Sqlite.Client.Shared
{
    public partial class MainLayout
    {
        private bool _open;
        private static readonly MudTheme DefaultTheme = new()
        {
            Palette = new Palette
            {
                Black = "#272c34",
                AppbarBackground = "#ffffff",
                AppbarText = "#ff584f",
                DrawerBackground = "#ff584f",
                DrawerText = "#ffffff",
                DrawerIcon = "#ffffff",
                Primary = "#ff584f",
                PrimaryContrastText = "#ffffff",
                Secondary = "#3d6fb4"
            }
        };

        private void ToggleMenu()
        {
            _open = !_open;
        }
    }
}