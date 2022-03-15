using Blazor.Sqlite.Client.Data;
using Blazor.Sqlite.Client.Features.Pokemon.Models;
using Blazor.Sqlite.Client.Services;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
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
#if DEBUG
                    if (_context.Pokemons.Count() > 151)
                    {
                        hasData = false;
                    }
                    else
                    {
                        hasData = !string.IsNullOrWhiteSpace(url);
                    }
#else
                    hasData = !string.IsNullOrWhiteSpace(url);
#endif
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
                    var pokemonSpeciesResult = await _httpClient.GetFromJsonAsync<PokemonSpecies>($"https://pokeapi.co/api/v2/pokemon-species/{id}");
                    if (pokemonResult != null && pokemonSpeciesResult != null)
                    {
                        await PersistPokemonAsync(pokemonResult, pokemonSpeciesResult);
                        var result = await _context.PokemonData
                            .Include(p => p.PokemonColor)
                            .Include(p => p.PokemonNames)
                                .ThenInclude(pn => pn.Language)
                            .Include(p => p.FlavorTextEntries)
                                .ThenInclude(pn => pn.Language)
                            .FirstOrDefaultAsync(p => p.Id == id);

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<PokemonEntity?> GetPokemonWithGraphQLAsync(string name)
        {
            try
            {
                var query = GetQuery(name);
                var pokemonDetails = await _httpClient.PostAsJsonAsync("https://beta.pokeapi.co/graphql/v1beta", query);
                var result = await pokemonDetails.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<Models.PokemonOverview>> GetPokemonCollectionAsync(int skip = 0, int take = Int32.MaxValue, CancellationToken cancellationToken = default)
        {
            return _context != null ?
                await _context.Pokemons.OrderBy(pokemon => pokemon.Id).Skip(skip).Take(take).ToListAsync(cancellationToken)
                : new List<Models.PokemonOverview>();
        }

        public async Task<int> GetPokemonCountAsync(CancellationToken cancellationToken = default)
        {
            return _context != null
                ? await _context.Pokemons.CountAsync(cancellationToken)
                : 0;
        }

        private async Task PersistPokemonAsync(PokemonDto dto, PokemonSpecies species)
        {
            if (!_context.Sprites.Any(sprite => sprite.Id == dto.Sprites.Id))
            {
                dto.Sprites.Id = dto.Id;
                _context.Sprites.Add(dto.Sprites);
            }

            var colorId = CutIdFromUrl(species.Color.Url);
            if (!_context.PokemonColors.Any(c => c.Id == colorId))
            {
                species.Color.Id = colorId;
                _context.PokemonColors.Add(species.Color);
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
                    SpritesId = dto.Id,
                    IsBaby = species.IsBaby,
                    IsMythical = species.IsMythical,
                    IsLegendary = species.IsLegendary,
                    PokemonColorId = colorId,
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

            dto.Stats.ForEach((pokemonStat) =>
            {
                var id = CutIdFromUrl(pokemonStat.Stat.Url);
                if (!_context.Stats.Any(s => s.Id == id))
                {
                    pokemonStat.Stat.Id = id;
                    _context.Stats.Add(pokemonStat.Stat);
                    _context.PokemonStats.Add(new PokemonStat
                    {
                        BaseStat = pokemonStat.BaseStat,
                        Effort = pokemonStat.Effort,
                        StatId = id,
                        PokemonEntityId = dto.Id,
                    });
                }
                else if (!_context.PokemonStats.Any(ps => ps.StatId == id && ps.PokemonEntityId == dto.Id))
                {
                    _context.PokemonStats.Add(new PokemonStat
                    {
                        BaseStat = pokemonStat.BaseStat,
                        Effort = pokemonStat.Effort,
                        StatId = id,
                        PokemonEntityId = dto.Id,
                    });
                }
            });

            var nameId = _context.PokemonNames.Count() + 1;
            species.Names.ForEach(name =>
            {
                if (!_context.PokemonNames.Any(n => n.Name == name.Name))
                {
                    var languageId = CutIdFromUrl(name.Language.Url);
                    if (!_context.PokemonLanguages.Any(l => l.Id == languageId))
                    {
                        name.Language.Id = languageId;
                        _context.PokemonLanguages.Add(name.Language);
                    }
                    name.Id = nameId;
                    name.LanguageId = languageId;
                    name.PokemonEntityId = dto.Id;
                    _context.PokemonNames.Add(name);
                }
                nameId++;
            });
            await _context.SaveChangesAsync();

            var flavorTextEntryId = _context.FlavorTextEntries.Count() + 1;
            species.FlavorTextEntries.ForEach(flavorTextEntry =>
            {
                if (!_context.FlavorTextEntries.Any(n => n.FlavorText == flavorTextEntry.FlavorText))
                {
                    var languageId = CutIdFromUrl(flavorTextEntry.Language.Url);
                    if (!_context.PokemonLanguages.Any(l => l.Id == languageId))
                    {
                        flavorTextEntry.Language.Id = languageId;
                        _context.PokemonLanguages.Add(flavorTextEntry.Language);
                    }
                    flavorTextEntry.Id = flavorTextEntryId;
                    flavorTextEntry.LanguageId = languageId;
                    flavorTextEntry.PokemonEntityId = dto.Id;
                    _context.FlavorTextEntries.Add(flavorTextEntry);
                }
                flavorTextEntryId++;
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

        private GraphQLRequest GetQuery(string name)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                  query pokemon_details($name: String) {
                      species: pokemon_v2_pokemonspecies(where: {name: {_eq: $name}}) {
                        name
                        base_happiness
                        is_legendary
                        is_mythical
                        generation: pokemon_v2_generation {
                          name
                        }
                        habitat: pokemon_v2_pokemonhabitat {
                          name
                        }
                        pokemon: pokemon_v2_pokemons_aggregate(limit: 1) {
                          nodes {
                            height
                            name
                            id
                            weight
                            abilities: pokemon_v2_pokemonabilities_aggregate {
                              nodes {
                                ability: pokemon_v2_ability {
                                  name
                                }
                              }
                            }
                            stats: pokemon_v2_pokemonstats {
                              base_stat
                              stat: pokemon_v2_stat {
                                name
                              }
                            }
                            types: pokemon_v2_pokemontypes {
                              slot
                              type: pokemon_v2_type {
                                name
                              }
                            }
                            levelUpMoves: pokemon_v2_pokemonmoves_aggregate(where: {pokemon_v2_movelearnmethod: {name: {_eq: ""level - up""}}}, distinct_on: move_id) {
                              nodes {
                                    move: pokemon_v2_move {
                                name
                                    }
                            level
                              }
                    }
                    foundInAsManyPlaces: pokemon_v2_encounters_aggregate {
                              aggregate {
                                count
                }
                            }
                            fireRedItems: pokemon_v2_pokemonitems(where: { pokemon_v2_version: { name: { _eq: ""firered""} } }) {
                    pokemon_v2_item {
                        name
                        cost
                              }
                    rarity
                            }
                          }
                        }
                        flavorText: pokemon_v2_pokemonspeciesflavortexts(where: { pokemon_v2_language: { name: { _eq: ""en""} }, pokemon_v2_version: { name: { _eq: ""firered""} } }) {
                    flavor_text
                        }
                      }
                    }
                ",
                Variables = new { name = name },
            };

            return query;
        }
    }
}
