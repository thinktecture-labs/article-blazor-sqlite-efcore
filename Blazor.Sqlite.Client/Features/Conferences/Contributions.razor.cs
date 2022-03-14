using Blazor.Sqlite.Client.Features.Conferences.Models;
using Blazor.Sqlite.Client.Features.Conferences.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Blazor.Sqlite.Client.Features.Conferences
{
    public partial class Contributions
    {
        [Inject] private ContributionsService ContributionsService { get; set; } = default!;

        private bool _isInitilazing = true;

        protected override async Task OnInitializedAsync()
        {
            await ContributionsService.InitAsync();
            _isInitilazing = false;
            await base.OnInitializedAsync();
        }

        private string SpeakerString(Contribution contribution)
        {
            var result = string.Empty;
            if (contribution.ContributionSpeakers.Count > 0)
            {
                result = String.Join(",",
                    contribution.ContributionSpeakers.Select(cs => $"{cs.Speaker.FirstName} {cs.Speaker.LastName}"));
            }
            return result;
        }

        private async ValueTask<ItemsProviderResult<Contribution>> LoadCollection(
        ItemsProviderRequest request)
        {
            try
            {
                var count = await ContributionsService.GetContributionCount(request.CancellationToken);
                var totalCount = Math.Min(request.Count, count - request.StartIndex);
                var result = await ContributionsService.GetContributions(request.StartIndex, totalCount, request.CancellationToken);
                return new ItemsProviderResult<Contribution>(result, count);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Current request was canceled.");
                return new ItemsProviderResult<Contribution>(new List<Contribution>(), 0);
            }
        }
    }
}