using Blazor.Sqlite.Client.Features.Contributions.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Blazor.Sqlite.Client.Features.Contributions.Services
{
    public class SpeakerService
    {
        private readonly IDbContextFactory<ContributionDbContext> _factory;
        private readonly HttpClient _httpClient;

        private bool _hasSynced = false;
        public SpeakerService(IDbContextFactory<ContributionDbContext> factory, HttpClient httpClient)
        {
            _factory = factory;
            _httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            if (_hasSynced) return;

            await using var dbContext = await _factory.CreateDbContextAsync();
            if (dbContext.Speakers.Count() > 0) return;

            var result = await _httpClient.GetFromJsonAsync<Root<Speaker>>("/sample-data/speakers.json");
            if (result != null)
            {
                await dbContext.Speakers.AddRangeAsync(result.Items);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Speaker>> GetSpeakersAsync()
        {
            await using var dbContext = await _factory.CreateDbContextAsync();
            return await dbContext.Speakers.ToListAsync();
        }

    }
}
