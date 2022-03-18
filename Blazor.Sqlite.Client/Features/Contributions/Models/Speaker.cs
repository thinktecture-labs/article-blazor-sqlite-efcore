using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Contributions.Models;

public class Speaker
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public ICollection<ContributionSpeaker> ContributionSpeakers { get; set; } = new List<ContributionSpeaker>();
}
