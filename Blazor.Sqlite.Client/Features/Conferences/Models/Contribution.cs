using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Conferences.Models;
public class Root<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int ItemCount { get; set; }
}

public class Contribution
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; }
    public string Language { get; set; } = string.Empty;
    public bool Billed { get; set; }
    public string Abstract { get; set; } = string.Empty;
    public string PreviewSrc { get; set; } = string.Empty;
    public string PrimaryTag { get; set; } = string.Empty;
    public bool ExternalSpeaker { get; set; }
    public int Conference { get; set; }

    public ICollection<ContributionSpeaker> ContributionSpeakers { get; set; } = new List<ContributionSpeaker>();
}

public class ContributionDto : Contribution
{
    [JsonPropertyName("speaker")]
    public List<int> Speaker { get; set; } = new List<int>();
}

public class ContributionSpeaker
{
    public int ContributionId { get; set; }
    public int SpeakerId { get; set; }

    public Contribution Contribution { get; set; } = default!;
    public Speaker Speaker { get; set; } = default!;
}

public class Speaker
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    public ICollection<ContributionSpeaker> ContributionSpeakers { get; set; } = new List<ContributionSpeaker>();
}
