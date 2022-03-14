using Blazor.Sqlite.Client.Features.Pokemon.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Blazor.Sqlite.Client.Features.Pokemon
{
    public partial class Pokemons
    {
        //private const string ImageBaseUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{0}.png";
        private const string ImageBaseUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{0}.png";

        [Inject] private PokemonService PokemonService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private bool _isInitilazing = true;
        protected override async Task OnInitializedAsync()
        {
            await PokemonService.InitAsync();
            _isInitilazing = false;
            await base.OnInitializedAsync();
        }

        private string GetPokemonImage(int id)
        {
            return String.Format(ImageBaseUrl, id);
        }

        private void OpenPokemonDetails(int id)
        {
            NavigationManager.NavigateTo($"/pokemons/{id}");
        }

        private async ValueTask<ItemsProviderResult<Models.Pokemon>> LoadCollection(
        ItemsProviderRequest request)
        {
            try
            {
                var count = await PokemonService.GetPokemonCountAsync(request.CancellationToken);
                var totalCount = Math.Min(request.Count, count - request.StartIndex);
                var result = await PokemonService.GetPokemonCollectionAsync(request.StartIndex, totalCount, request.CancellationToken);
                return new ItemsProviderResult<Models.Pokemon>(result, count);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Current request was canceled.");
                return new ItemsProviderResult<Models.Pokemon>(new List<Models.Pokemon>(), 0);
            }
        }
    }
}