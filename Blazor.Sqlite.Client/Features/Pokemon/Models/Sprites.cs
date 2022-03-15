using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class Sprites
    {
        public int Id { get; set; }

        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }
        public ICollection<PokemonEntity> PokemonEntities { get; set; }
    }
}
