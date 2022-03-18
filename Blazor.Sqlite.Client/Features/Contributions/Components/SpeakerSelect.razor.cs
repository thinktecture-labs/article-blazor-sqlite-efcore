using Blazor.Sqlite.Client.Features.Contributions.Models;
using Blazor.Sqlite.Client.Features.Contributions.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Sqlite.Client.Features.Contributions.Components
{
    public partial class SpeakerSelect
    {
        [Inject] private SpeakerService _speakersService { get; set; } = default!;
        [Parameter] public string Label { get; set; }
        [Parameter] public List<int> SelectedSpeakers { get; set; } = new List<int>();
        [Parameter] public EventCallback<List<int>> SelectedSpeakersChanged { get; set; }

        private List<Speaker> _speakers = new List<Speaker>();
        private List<int> _selectedSpeakers = new List<int>();
        protected override async Task OnInitializedAsync()
        {
            if (SelectedSpeakers?.Count > 0)
            {
                _selectedSpeakers.AddRange(SelectedSpeakers);
            }
            _speakers = await _speakersService.GetSpeakersAsync();
            await base.OnInitializedAsync();
        }

        private async Task OnSpeakerSelectionChanged(IEnumerable<int> selection)
        {
            _selectedSpeakers = new List<int>(selection);
            await SelectedSpeakersChanged.InvokeAsync(_selectedSpeakers);
        }

        private string GetSpeakerString(int speakerId)
        {
            var speaker = _speakers.FirstOrDefault(s => s.Id == speakerId);
            return $"{speaker.FirstName} {speaker.LastName}";
        }
    }
}