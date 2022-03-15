namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonEntity : PokemonBase
    {
        // Species Props
        public int BaseHappiness { get; set; }
        public int CaptureRate { get; set; }
        public bool IsBaby { get; set; }
        public bool IsLegendary { get; set; }
        public bool IsMythical { get; set; }
        public int Order { get; set; }

        public int SpeciesId { get; set; }
        public int SpritesId { get; set; }
        public int PokemonColorId { get; set; }

        public PokemonColor PokemonColor { get; set; }
        public ICollection<FlavorTextEntry> FlavorTextEntries { get; set; }
        public ICollection<PokemonName> PokemonNames { get; set; }
        public ICollection<PokemonForm> PokemonForms { get; set; }
        public ICollection<PokemonToType> PokemonToTypes { get; set; }
        public ICollection<PokemonStat> PokemonStats { get; set; }
    }
}
