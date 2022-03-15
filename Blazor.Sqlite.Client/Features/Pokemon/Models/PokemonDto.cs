using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonDto :  PokemonBase 
    {
        [JsonPropertyName("forms")]
        public List<Form> Forms { get; set; }

        [JsonPropertyName("types")]
        public List<PokemonTypeDto> Types { get; set; }

        [JsonPropertyName("stats")]
        public List<PokemonStat> Stats { get; set; }
    }
}
