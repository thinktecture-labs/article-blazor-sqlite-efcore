using Blazor.Sqlite.Client.Features.Contributions.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Blazor.Sqlite.Client.Features.Contributions.Services
{
    public class SpeakerService
    {
        private readonly ContributionDbContext _dbContext;
        private readonly HttpClient _httpClient;

        private bool _hasSynced = false;
        public SpeakerService(ContributionDbContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            if (_hasSynced) return;

            if (_dbContext.Speakers.Count() > 0) return;

            var result = await _httpClient.GetFromJsonAsync<Root<Speaker>>("/sample-data/speakers.json");
            if (result != null)
            {
                await _dbContext.Speakers.AddRangeAsync(result.Items);
            }

        }

        public Task<List<Speaker>> GetSpeakersAsync()
        {
            return _dbContext.Speakers.ToListAsync();
        }

    }
}
