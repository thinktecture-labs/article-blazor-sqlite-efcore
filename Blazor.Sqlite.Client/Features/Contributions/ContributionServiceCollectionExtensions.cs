using Blazor.Sqlite.Client.Features.Contributions.Models;
using Blazor.Sqlite.Client.Features.Contributions.Services;
using Blazor.Sqlite.Client.Features.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blazor.Sqlite.Client.Features.Contributions
{
    public static class ContributionServiceCollectionExtensions
    {

        public static void AddContributionsFeature(this IServiceCollection services)
        {
#if DEBUG
            services.AddDbContextFactory<ContributionDbContext>(
                    options =>
                    {
                        options.UseInMemoryDatabase("BlazorSqlite");
                        options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    });
#else
            services.AddDbContextFactory<ContributionDbContext>(
                    options => options.UseSqlite($"Filename={DatabaseService<ContributionDbContext>.FileName}"));
#endif

            services.AddScoped<ContributionsService>();
            services.AddScoped<SpeakerService>();
        }

        public static async Task InitializeContributionsFeature(this WebAssemblyHost host)
        {
            // Initialize DatabaseContext and sync with IndexedDb Files
            var dbService = host.Services.GetRequiredService<DatabaseService<ContributionDbContext>>();
            await dbService.InitDatabaseAsync();

            // Sync Speakers
            var speakerService = host.Services.GetRequiredService<SpeakerService>();
            await speakerService.InitializeAsync();

            // Sync Contributions
            var contributionService = host.Services.GetRequiredService<ContributionsService>();
            await contributionService.InitAsync();
        }
    }
}
