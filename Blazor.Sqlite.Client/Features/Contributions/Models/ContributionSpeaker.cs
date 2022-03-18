namespace Blazor.Sqlite.Client.Features.Contributions.Models;

public class ContributionSpeaker
{
    public int ContributionId { get; set; }
    public int SpeakerId { get; set; }

    public Contribution Contribution { get; set; } = default!;
    public Speaker Speaker { get; set; } = default!;
}
