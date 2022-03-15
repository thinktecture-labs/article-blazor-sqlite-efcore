using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonColor
    {
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        public ICollection<PokemonEntity> PokemonEntities { get; set; }
    }
}
