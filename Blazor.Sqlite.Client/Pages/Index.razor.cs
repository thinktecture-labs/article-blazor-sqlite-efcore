using Blazor.Sqlite.Client.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Sqlite.Client.Pages
{
    public partial class Index
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        
        private void NavigateToPage(string page)
        {            
            NavigationManager.NavigateTo($"/{page}");
        }
    }
}