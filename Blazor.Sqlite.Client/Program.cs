using Blazor.Sqlite.Client;
using Blazor.Sqlite.Client.Features.Contributions;
using Blazor.Sqlite.Client.Features.Contributions.Models;
using Blazor.Sqlite.Client.Features.Shared.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Diagnostics.CodeAnalysis;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddSingleton<DatabaseService<ContributionDbContext>>();
builder.Services.AddContributionsFeature();

var host = builder.Build();

await host.InitializeContributionsFeature();
await host.RunAsync();

public partial class Program
{
    /// <summary>
    /// https://github.com/dotnet/efcore/issues/26860
    /// https://github.com/dotnet/aspnetcore/issues/39825
    /// FIXME: This is required for EF Core 6.0 as it is not compatible with trimming.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    private static Type _keepDateOnly = typeof(DateOnly);
}
