using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Contributions.Models;

public class ContributionDto : Contribution
{
    public string Date { get; set; } = string.Empty;
    public List<int> Speaker { get; set; } = new List<int>();
}
