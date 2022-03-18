using Blazor.Sqlite.Client.Features.Contributions.Models;
using Blazor.Sqlite.Client.Features.Contributions.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.Sqlite.Client.Features.Contributions.Components
{
    public partial class ContributionEditor
    {
        [Inject] private ContributionsService _contributionsService { get; set; } = default!;
        [CascadingParameter] public MudDialogInstance DialogInstance { get; set; } = default!;
        [Parameter] public Contribution? Contribution { get; set; }

        private Contribution? _editModel;
        private List<int> _selectedSpeakers = new List<int>();

        protected override void OnInitialized()
        {
            _editModel = new Contribution();
            if (Contribution != null)
            {

                _editModel.Id = Contribution.Id;
                _editModel.Title = Contribution.Title;
                _editModel.Type = Contribution.Type;
                _editModel.StartDate = Contribution.StartDate;
                _editModel.Abstract = Contribution.Abstract;
                _editModel.Billed = Contribution.Billed;
                _editModel.PreviewSrc = Contribution.PreviewSrc;
                _editModel.PrimaryTag = Contribution.PrimaryTag;
                _editModel.ExternalSpeaker = Contribution.ExternalSpeaker;
                _editModel.Conference = Contribution.Conference;
                _editModel.ContributionSpeakers = Contribution.ContributionSpeakers;
            }

            if (_editModel.ContributionSpeakers.Count > 0)
            {
                _selectedSpeakers = _editModel.ContributionSpeakers.Select(cs => cs.SpeakerId).ToList() ?? new List<int>();
            }
            base.OnInitialized();
        }

        private void OnSpeakerSelectionChanged(List<int> selection)
        {
            _selectedSpeakers = selection;
        }

        private async Task SaveContribution()
        {            
            if (await _contributionsService.SaveContributionAsync(_editModel, _selectedSpeakers))
            {
                DialogInstance.Close(true);
            }
            else
            {
                DialogInstance.Close(false);
            }
        }

        private void Cancel() => DialogInstance.Cancel();
    }
}