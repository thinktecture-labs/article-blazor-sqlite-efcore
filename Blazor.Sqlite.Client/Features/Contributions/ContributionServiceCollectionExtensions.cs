using Blazor.Sqlite.Client.Features.Contributions.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Blazor.Sqlite.Client.Features.Contributions
{
    public static class ContributionServiceCollectionExtensions
    {

        public static void AddContributionsFeature(this IServiceCollection services)
        {
            services.AddScoped<ContributionsService>();
            services.AddScoped<SpeakerService>();
        }

        public static async Task InitializeContributionsFeature(this WebAssemblyHost builder)
        {
            var speakerService = builder.Services.GetRequiredService<SpeakerService>();
            await speakerService.InitializeAsync();
            var contributionService = builder.Services.GetRequiredService<ContributionsService>();
            await contributionService.InitAsync();
        }
    }
}
