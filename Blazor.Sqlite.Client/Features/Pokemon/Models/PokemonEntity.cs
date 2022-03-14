using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Pokemon.Models
{

    public class PokemonEntity : PokemonBase
    {
        public int SpeciesId { get; set; }
        public int SpritesId { get; set; }

        public ICollection<PokemonForm> PokemonForms { get; set; }
        public ICollection<PokemonToType> PokemonToTypes { get; set; }
    }

    public class PokemonDto :  PokemonBase 
    {
        [JsonPropertyName("forms")]
        public List<Form> Forms { get; set; }

        [JsonPropertyName("types")]
        public List<PokemonTypeDto> Types { get; set; }
    }

    public class PokemonBase
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("is_default")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("weight")]
        public int Weight { get; set; }


        // Entities
        [JsonPropertyName("species")]
        public Species Species { get; set; }

        [JsonPropertyName("sprites")]
        public Sprites Sprites { get; set; }
    }

    public class Form
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
        public ICollection<PokemonForm> PokemonForms { get; set; }
    }

    public class PokemonType
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
        public ICollection<PokemonToType> PokemonToTypes { get; set; }
    }

    public class PokemonTypeDto
    {
        [JsonPropertyName("slot")]
        public int Slot { get; set; }
        [JsonPropertyName("type")]
        public PokemonType Type { get; set; }
    }

    public class PokemonToType
    {
        public int PokemonEntityId { get; set; }
        public int PokemonTypeId { get; set; }

        public PokemonType PokemonType { get; set; }
        public PokemonEntity PokemonEntity { get; set; }
    }

    public class PokemonForm
    {
        public int PokemonEntityId { get; set; }
        public int FormId { get; set; }

        public Form Form { get; set; }
        public PokemonEntity PokemonEntity { get; set; }
    }

    public class Species
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        public ICollection<PokemonEntity> PokemonEntities { get; set; }
    }

    public class Sprites
    {
        public int Id { get; set; }

        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }
        public ICollection<PokemonEntity> PokemonEntities { get; set; }
    }
}
