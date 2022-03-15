namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{
    public class PokemonForm
    {
        public int PokemonEntityId { get; set; }
        public int FormId { get; set; }

        public Form Form { get; set; }
        public PokemonEntity PokemonEntity { get; set; }
    }
}
