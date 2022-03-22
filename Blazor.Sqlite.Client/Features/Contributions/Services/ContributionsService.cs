using Blazor.Sqlite.Client.Features.Contributions.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;

namespace Blazor.Sqlite.Client.Features.Contributions.Services
{
    public class ContributionsService
    {
        private readonly IDbContextFactory<ContributionDbContext> _factory;
        private readonly HttpClient _httpClient;

        private bool _hasSynced = false;
        public ContributionsService(IDbContextFactory<ContributionDbContext> factory, HttpClient httpClient)
        {
            _factory = factory;
            _httpClient = httpClient;
        }

        public async Task InitAsync()
        {
            if (_hasSynced) return;

            await using var dbContext = await _factory.CreateDbContextAsync();
            if (dbContext.Contributions.Count() > 0) return;

            var result = await _httpClient.GetFromJsonAsync<Root<ContributionDto>>("/sample-data/contributions.json");
            if (result?.Items.Count > 0)
            {
                var index = 1;
                result.Items.ForEach(item =>
                {
                    item.Id = index++;
                    if (DateTime.TryParse(item.Date, out var startDate))
                    {
                        item.StartDate = startDate;
                    }

                    dbContext.Contributions.Add(item);
                    if (item.Speaker.Any())
                    {
                        item.Speaker.ForEach(speakerId =>
                        {
                            dbContext.ContributionSpeakers.Add(new ContributionSpeaker
                            {
                                ContributionId = item.Id,
                                SpeakerId = speakerId
                            });
                        });
                    }
                });
            }

            await dbContext.SaveChangesAsync();
            _hasSynced = true;
        }

        public async Task<List<Contribution>> GetContributions(int skip = 0, int take = Int32.MaxValue, string searchTerm = "", CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _factory.CreateDbContextAsync();

            // Was passiert wenn der searchTerm zeichen wie %"' beinhaltet
            var query = String.IsNullOrWhiteSpace(searchTerm)
                ? dbContext.Contributions.Include(c => c.ContributionSpeakers).ThenInclude(cs => cs.Speaker)
                : dbContext.Contributions.Include(c => c.ContributionSpeakers).ThenInclude(cs => cs.Speaker)
                                         .Where(c => EF.Functions.Like(c.Title, $"%{searchTerm}%"));

            return await query.OrderBy(c => c.Title).Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public async Task<int> GetContributionCount(CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _factory.CreateDbContextAsync();
            return await dbContext.Contributions.CountAsync(cancellationToken);
        }

        public async Task<bool> SaveContributionAsync(Contribution contribution, List<int> speakerIds)
        {
            try
            {

                await using var dbContext = await _factory.CreateDbContextAsync();
                if (contribution.Id != 0)
                {
                    var currentContribution = await dbContext.Contributions.Include(c => c.ContributionSpeakers).FirstOrDefaultAsync(c => c.Id == contribution.Id);
                    if (currentContribution == null)
                    {
                        return false;
                    }
                    currentContribution.Title = contribution.Title;
                    currentContribution.Abstract = contribution.Abstract;
                    currentContribution.Type = contribution.Type;
                    currentContribution.PrimaryTag = contribution.PrimaryTag;
                    currentContribution.ExternalSpeaker = contribution.ExternalSpeaker;
                    currentContribution.StartDate = contribution.StartDate;

                    foreach (var speakerId in speakerIds)
                    {
                        if (!currentContribution.ContributionSpeakers.Any(cs => cs.ContributionId == currentContribution.Id && cs.SpeakerId == speakerId))
                        {
                            dbContext.ContributionSpeakers.Add(new ContributionSpeaker
                            {
                                SpeakerId = speakerId,
                                ContributionId = currentContribution.Id
                            });
                        }

                        var diff = currentContribution.ContributionSpeakers.Select(cs => cs.SpeakerId).Except(speakerIds);
                        foreach (var removeItem in diff)
                        {
                            var relation = currentContribution.ContributionSpeakers.FirstOrDefault(cs => cs.ContributionId == currentContribution.Id && cs.SpeakerId == removeItem);
                            if (relation != null)
                            {
                                dbContext.ContributionSpeakers.Remove(relation);
                            }
                        }
                    }
                }
                else
                {
                    var entry = dbContext.Contributions.Add(contribution);
                    foreach (var speakerId in speakerIds)
                    {
                        dbContext.ContributionSpeakers.Add(new ContributionSpeaker
                        {
                            SpeakerId = speakerId,
                            ContributionId = entry.Entity.Id
                        });
                    }
                }
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        public async Task<bool> DeleteContributionAsync(int id)
        {
            try
            {
                await using var dbContext = await _factory.CreateDbContextAsync();
                var currentContribution = await dbContext.Contributions.FirstOrDefaultAsync(c => c.Id == id);
                if (currentContribution != null)
                {
                    dbContext.Contributions.Remove(currentContribution);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}
