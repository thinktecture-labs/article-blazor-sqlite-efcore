using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonApiResult
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }

        [JsonPropertyName("previous")]
        public object Previous { get; set; }

        [JsonPropertyName("results")]
        public List<PokemonOverview> Results { get; set; }
    }
}
