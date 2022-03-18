using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Sqlite.Client.Features.Contributions.Models;

public class Contribution
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public string Language { get; set; } = string.Empty;
    public bool Billed { get; set; }
    public string Abstract { get; set; } = string.Empty;
    public string PreviewSrc { get; set; } = string.Empty;
    public string PrimaryTag { get; set; } = string.Empty;
    public bool ExternalSpeaker { get; set; }
    public int Conference { get; set; }

    public ICollection<ContributionSpeaker> ContributionSpeakers { get; set; } = new List<ContributionSpeaker>();
}
