using Blazor.Sqlite.Client;
using Blazor.Sqlite.Client.Data;
using Blazor.Sqlite.Client.Features.Conferences.Services;
using Blazor.Sqlite.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Diagnostics.CodeAnalysis;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if DEBUG
builder.Services.AddDbContext<DatabaseContext>(
            options => options.UseInMemoryDatabase("BlazorSqlite"));
#else
builder.Services.AddDbContextFactory<DatabaseContext>(
            options => options.UseSqlite($"Filename=/database/app.db"));
#endif

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<ContributionsService>();

var host = builder.Build();

var dbService = host.Services.GetRequiredService<DatabaseService>();
await dbService.InitDatabaseAsync();
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
