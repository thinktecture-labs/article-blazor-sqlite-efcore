using Blazor.Sqlite.Client.Features.Conferences.Components;
using Blazor.Sqlite.Client.Features.Conferences.Models;
using Blazor.Sqlite.Client.Features.Conferences.Services;
using Blazor.Sqlite.Client.Features.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MudBlazor;

namespace Blazor.Sqlite.Client.Features.Conferences
{
    public partial class Contributions
    {
        [Inject] private ContributionsService _contributionsService { get; set; } = default!;
        [Inject] private IDialogService _dialogService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;

        private bool _isInitilazing = true;
        private Virtualize<Contribution>? _virtualize;

        protected override async Task OnInitializedAsync()
        {
            await _contributionsService.InitAsync();
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
                var count = await _contributionsService.GetContributionCount(request.CancellationToken);
                var totalCount = Math.Min(request.Count, count - request.StartIndex);
                var result = await _contributionsService.GetContributions(request.StartIndex, totalCount, request.CancellationToken);
                return new ItemsProviderResult<Contribution>(result, count);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Current request was canceled.");
                return new ItemsProviderResult<Contribution>(new List<Contribution>(), 0);
            }
        }

        private void CreateOrUpdateContribution(Contribution? contribution = null)
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(ContributionEditor.Contribution), contribution);

            var options = new DialogOptions
            {
                CloseButton = true,
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true,
                Position = DialogPosition.TopCenter,
            };

            _dialogService.Show<ContributionEditor>(contribution == null ? "Add contribution" : "Edit contribution", parameters, options);
        }

        private async Task DeleteContribution(int id)
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", "Do you really want to delete these record? This process cannot be undone.");
            parameters.Add("ButtonText", "Delete");
            parameters.Add("Color", Color.Error);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var result = await _dialogService.Show<ConfirmDialog>("Delete", parameters, options)!.Result;

            if (!result.Cancelled)
            {
                if (await _contributionsService.DeleteContributionAsync(id))
                {
                    await _virtualize!.RefreshDataAsync();
                }
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/");
        }
    }
}