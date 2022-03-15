using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonName
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int PokemonEntityId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("language")]
        public PokemonLanguage Language { get; set; }

        public PokemonEntity PokemonEntity { get; set; }
    }
}
