using Blazor.Sqlite.Client.Features.Contributions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;

namespace Blazor.Sqlite.Client.Features.Contributions.Models
{
    public class ContributionDbContext : DbContext
    {
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<ContributionSpeaker> ContributionSpeakers { get; set; }

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public ContributionDbContext(DbContextOptions<ContributionDbContext> options
            , IJSRuntime jsRuntime)
        : base(options)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./js/file.js").AsTask());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContributionSpeaker>()
                .HasKey(t => new { t.ContributionId, t.SpeakerId });

            modelBuilder.Entity<ContributionSpeaker>()
                .HasOne(pt => pt.Contribution)
                .WithMany(p => p.ContributionSpeakers)
                .HasForeignKey(pt => pt.ContributionId);

            modelBuilder.Entity<ContributionSpeaker>()
                .HasOne(pt => pt.Speaker)
                .WithMany(t => t.ContributionSpeakers)
                .HasForeignKey(pt => pt.SpeakerId);

            base.OnModelCreating(modelBuilder);
        }

#if RELEASE
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.LogTo(Console.WriteLine, LogLevel.Error)
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging(false);
        }
#endif

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
#if RELEASE
            await PersistDatabaseAsync(cancellationToken);       
#endif
            return result;
        }

        private async Task PersistDatabaseAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Start saving database");
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("syncDatabase", false, cancellationToken);
            Console.WriteLine("Finish save database");
        }
    }
}
