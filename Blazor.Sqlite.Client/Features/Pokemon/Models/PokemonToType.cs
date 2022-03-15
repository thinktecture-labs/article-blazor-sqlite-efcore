namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonToType
    {
        public int PokemonEntityId { get; set; }
        public int PokemonTypeId { get; set; }

        public PokemonType PokemonType { get; set; }
        public PokemonEntity PokemonEntity { get; set; }
    }
}
