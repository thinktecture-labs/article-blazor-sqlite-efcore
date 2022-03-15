using Blazor.Sqlite.Client.Features.Pokemon.Models;
using Blazor.Sqlite.Client.Features.Pokemon.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Sqlite.Client.Features.Pokemon
{
    public partial class PokemonDetails
    {
        private const string ImageBaseUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{0}.png";

        [Inject] private PokemonService _pokemonService { get; set; } = default!;
        [Parameter] public int Id { get; set; }
        [Parameter] public string Name { get; set; } = string.Empty;

        private PokemonEntity? _pokemon;
        private string? _name;
        private string? _flavorText;
        private string _pokemonImage => String.Format(ImageBaseUrl, Id);
        private bool _isInitialized = false;

        protected override async Task OnInitializedAsync()
        {
            _pokemon = await _pokemonService.GetPokemonAsync(Id);
            _name =  _pokemon?.PokemonNames?.FirstOrDefault(n => n.Language.Name.Contains("de"))?.Name;
            _flavorText =  _pokemon?.FlavorTextEntries?.FirstOrDefault(f => f.Language.Name.Contains("de"))?.FlavorText;
            _isInitialized = true;
            await base.OnInitializedAsync();
        }

        public string GetColor(string type) 
        {
            switch(type)
            {
                case "bug":
                    return "#28963C";
                case "dark":
                    return "#5B5A77";
                case "dragon":
                    return "#72CAD6";
                case "electric":
                    return "#FAFB75";
                case "fairy":
                    return "#DF1E6C";
                case "fighting":
                    return "#E5633F";
                case "fire":
                    return "#F24F5E";
                case "flying":
                    return "#97B2C7";
                case "ghost":
                    return "#33336B";
                case "grass":
                    return "#4CCA4E";
                case "ground":
                    return "#A27030";
                case "ice":
                    return "#91D2F4";
                case "normal":
                    return "#74515B";
                case "poison":
                    return "#996AD9";
                case "psychic":
                    return "#ED2693";
                case "rock":
                    return "#853F25";
                case "steel":
                    return "#61756D";
                case "water":
                    return "#2C53E1";
                default:
                    return "#74515B";
            }
        }

        public MudBlazor.Color GetStatColor()
        {
            if (_pokemon?.PokemonColor == null)
            {
                return MudBlazor.Color.Default;
            }
            switch(_pokemon.PokemonColor.Name)
            {
                case "green":
                    return MudBlazor.Color.Success;
                case "blue":
                    return MudBlazor.Color.Info;
                case "red":
                    return MudBlazor.Color.Error;
                default:
                    return MudBlazor.Color.Default;
            }
        }
    }
}