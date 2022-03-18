namespace Blazor.Sqlite.Client.Features.Contributions.Models;

public class Root<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int ItemCount { get; set; }
}
