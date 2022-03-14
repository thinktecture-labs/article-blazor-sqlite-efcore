using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blazor.Sqlite.Client.Features.Conferences.Models;
public class Root<T>
{
    public List<T> Items { get; set; }
    public int ItemCount { get; set; }
}

public class Contribution
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Date { get; set; }
    public string Language { get; set; }
    public bool Billed { get; set; }
    public string Abstract { get; set; }
    public string PreviewSrc { get; set; }
    public string PrimaryTag { get; set; } = string.Empty;
    public bool ExternalSpeaker { get; set; }
    public int Conference { get; set; }

    public virtual ICollection<ContributionSpeaker> ContributionSpeakers { get; set; }
}

public class ContributionDto : Contribution
{
    [JsonPropertyName("speaker")]
    public List<int> Speaker { get; set; }
}

public class ContributionSpeaker
{
    public int ContributionId { get; set; }
    public int SpeakerId { get; set; }

    public Contribution Contribution { get; set; }
    public Speaker Speaker { get; set; }
}

public class Speaker
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    public virtual ICollection<ContributionSpeaker> ContributionSpeakers { get; set; }
}
