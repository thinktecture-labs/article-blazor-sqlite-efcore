using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonStat
    {
        [JsonPropertyName("base_stat")]
        public int BaseStat { get; set; }

        [JsonPropertyName("effort")]
        public int Effort { get; set; }

        [JsonPropertyName("stat")]
        public Stat Stat { get; set; }

        public int StatId { get; set; }
        public int PokemonEntityId { get; set; }
        public PokemonEntity PokemonEntity { get; set; }
    }
}
