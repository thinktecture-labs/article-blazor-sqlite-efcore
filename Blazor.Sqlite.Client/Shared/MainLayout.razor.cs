using Blazor.Sqlite.Client.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.Sqlite.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] private DatabaseService DatabaseService { get; set; } = default!;

        private bool _isInitialized;

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

        protected override void OnInitialized()
        {
            DatabaseService.SyncFinished += DatabaseService_SyncFinished;
            base.OnInitialized();
        }

        private void DatabaseService_SyncFinished(object? sender, SyncFinishedEvent e)
        {
            _isInitialized = true;
            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("App | OnAfterRenderAsync | Start initialize database");
                await DatabaseService.InitDatabaseAsync();
                await InvokeAsync(StateHasChanged);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private void ToggleMenu()
        {
            _open = !_open;
        }

        public void Dispose()
        {
            DatabaseService.SyncFinished -= DatabaseService_SyncFinished;
        }
    }
}