using Blazor.Sqlite.Client.Data;
using Blazor.Sqlite.Client.Features.Conferences.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Blazor.Sqlite.Client.Features.Conferences.Services
{
    public class ContributionsService
    {
        private readonly DatabaseContext _dbContext;
        private readonly HttpClient _httpClient;

        private bool _hasSynced = false;
        public ContributionsService(DatabaseContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }

        public async Task InitAsync()
        {
            if (_hasSynced) return;

            if (_dbContext.Contributions.Count() > 0) return;

            await SyncSpeakers();

            var result = await _httpClient.GetFromJsonAsync<Root<ContributionDto>>("/sample-data/contributions.json");
            if (result?.Items.Count > 0)
            {
                var index = 1;
                result.Items.ForEach(item =>
                {
                    item.Id = index++;
                    _dbContext.Contributions.Add(item);
                    if (item.Speaker.Any())
                    {
                        item.Speaker.ForEach(speakerId =>
                        {
                            _dbContext.ContributionSpeakers.Add(new ContributionSpeaker
                            {
                                ContributionId = item.Id,
                                SpeakerId = speakerId
                            });
                        });
                    }
                });
            }

            await _dbContext.SaveChangesAsync();
            _hasSynced = true;
        }

        public Task<List<Contribution>> GetContributions(int skip = 0, int take = Int32.MaxValue, CancellationToken cancellationToken = default)
        {
            return _dbContext.Contributions.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public Task<int> GetContributionCount(CancellationToken cancellationToken = default)
        {
            return _dbContext.Contributions.CountAsync(cancellationToken);
        }

        private async Task SyncSpeakers()
        {
            var result = await _httpClient.GetFromJsonAsync<Root<Speaker>>("/sample-data/speakers.json");
            if (result != null)
            {
                await _dbContext.Speakers.AddRangeAsync(result.Items);
            }

        }
    }
}
