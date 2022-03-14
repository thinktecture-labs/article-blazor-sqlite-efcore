using Blazor.Sqlite.Client.Data;
using Blazor.Sqlite.Client.Features.Pokemon.Models;
using Blazor.Sqlite.Client.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Blazor.Sqlite.Client.Features.Pokemon.Services
{
    public class PokemonService
    {
        private readonly DatabaseContext _context;
        private readonly HttpClient _httpClient;

        private bool _hasSynced = false;
        public PokemonService(DatabaseContext context, HttpClient httpClient)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClient = httpClient;
        }

        public async Task InitAsync()
        {
            if (_hasSynced) return;

            if (_context != null && _context.Pokemons.Any()) return;

            if (_context != null)
            {
                var hasData = true;
                var url = string.Empty;
                var count = 1;
                while (hasData)
                {
                    var result = await _httpClient.GetFromJsonAsync<PokemonApiResult>(String.IsNullOrEmpty(url)
                        ? "https://pokeapi.co/api/v2/pokemon"
                        : url);

                    if (result == null) continue;

                    foreach (var pokemon in result.Results)
                    {
                        pokemon.Id = count++;
                    }

                    await _context.Pokemons.AddRangeAsync(result.Results);
                    url = result.Next;
                    hasData = !string.IsNullOrWhiteSpace(url);
                }

                await _context.SaveChangesAsync();
                _hasSynced = true;
            }
        }

        public async Task<PokemonEntity?> GetPokemonAsync(int id)
        {
            try
            {
                var pokemon = await _context.PokemonData.Include(p => p.PokemonToTypes).ThenInclude(pt => pt.PokemonType).FirstOrDefaultAsync(pokemon => pokemon.Id == id);
                if (pokemon != null)
                {
                    return pokemon;
                }
                else
                {
                    var pokemonResult = await _httpClient.GetFromJsonAsync<PokemonDto>($"https://pokeapi.co/api/v2/pokemon/{id}");
                    if (pokemonResult != null)
                    {
                        await PersistPokemonAsync(pokemonResult);
                        return await _context.PokemonData.Include(p => p.PokemonToTypes).ThenInclude(pt => pt.PokemonType).FirstOrDefaultAsync(p => p.Id == id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<Models.Pokemon>> GetPokemonCollectionAsync(int skip = 0, int take = Int32.MaxValue, CancellationToken cancellationToken = default)
        {
            return _context != null ?
                await _context.Pokemons.OrderBy(pokemon => pokemon.Id).Skip(skip).Take(take).ToListAsync(cancellationToken)
                : new List<Models.Pokemon>();
        }

        public async Task<int> GetPokemonCountAsync(CancellationToken cancellationToken = default)
        {
            return _context != null
                ? await _context.Pokemons.CountAsync(cancellationToken)
                : 0;
        }

        private async Task PersistPokemonAsync(PokemonDto dto)
        {
            if (!_context.Sprites.Any(sprite => sprite.Id == dto.Sprites.Id))
            {
                dto.Sprites.Id = dto.Id;
                _context.Sprites.Add(dto.Sprites);
            }

            var speciesId = CutIdFromUrl(dto.Species.Url);
            if (!_context.Species.Any(sprite => sprite.Id == dto.Species.Id))
            {
                dto.Species.Id = speciesId;
                _context.Species.Add(dto.Species);
            }

            if (!_context.PokemonData.Any(pokemon => pokemon.Id == dto.Id))
            {
                var pokemon = new PokemonEntity
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Height = dto.Height,
                    Order = dto.Order,
                    Weight = dto.Weight,
                    SpeciesId = speciesId,
                    SpritesId = dto.Id
                };
                _context.PokemonData.Add(pokemon);
            }

            dto.Forms.ForEach((form) =>
            {
                var id = CutIdFromUrl(form.Url);
                if (!_context.Forms.Any(f => f.Id == id))
                {
                    form.Id = id;
                    _context.Forms.Add(form);
                    _context.PokemonForms.Add(new PokemonForm
                    {
                        FormId = id,
                        PokemonEntityId = dto.Id,
                    });
                } 
                else if (!_context.PokemonForms.Any(pf => pf.FormId == id && pf.PokemonEntityId == dto.Id))
                {
                    _context.PokemonForms.Add(new PokemonForm
                    {
                        FormId = id,
                        PokemonEntityId = dto.Id,
                    });
                }
            });

            dto.Types.ForEach((pokemonType) =>
            {
                var id = CutIdFromUrl(pokemonType.Type.Url);
                if (!_context.PokemonTypes.Any(f => f.Id == id))
                {
                    pokemonType.Type.Id = id;
                    _context.PokemonTypes.Add(pokemonType.Type);
                    _context.PokemonToTypes.Add(new PokemonToType
                    {
                        PokemonTypeId = id,
                        PokemonEntityId = dto.Id,
                    });
                }
                else if (!_context.PokemonToTypes.Any(pf => pf.PokemonTypeId == id && pf.PokemonEntityId == dto.Id))
                {
                    _context.PokemonToTypes.Add(new PokemonToType
                    {
                        PokemonTypeId = id,
                        PokemonEntityId = dto.Id,
                    });
                }
            });
            await _context.SaveChangesAsync();
        }

        private int CutIdFromUrl(string url)
        {
            var currentUrl = url.Trim('/');
            var lastIndex = currentUrl.LastIndexOf('/');
            var idString = currentUrl.Substring(lastIndex + 1, (currentUrl.Length - lastIndex) - 1);
            return Int32.Parse(idString);
        }
    }
}
