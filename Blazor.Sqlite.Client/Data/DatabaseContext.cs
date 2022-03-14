using Blazor.Sqlite.Client.Features.Conferences.Models;
using Blazor.Sqlite.Client.Features.Pokemon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;

namespace Blazor.Sqlite.Client.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<TodoItem> Todos { get; set; }
        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<PokemonEntity> PokemonData { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Sprites> Sprites { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<PokemonType> PokemonTypes { get; set; }
        public DbSet<PokemonForm> PokemonForms { get; set; }
        public DbSet<PokemonToType> PokemonToTypes { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<ContributionSpeaker> ContributionSpeakers { get; set; }

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private readonly ISnackbar _snackbar;

        public DatabaseContext(DbContextOptions<DatabaseContext> options
            , IJSRuntime jsRuntime
            , ISnackbar snackbar)
        : base(options)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./js/file.js").AsTask());
            _snackbar = snackbar;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PokemonForm>()
                .HasKey(pf => new {pf.PokemonEntityId, pf.FormId});

            modelBuilder.Entity<PokemonForm>()
                .HasOne(pt => pt.PokemonEntity)
                .WithMany(p => p.PokemonForms)
                .HasForeignKey(pt => pt.PokemonEntityId);

            modelBuilder.Entity<PokemonForm>()
                .HasOne(pt => pt.Form)
                .WithMany(p => p.PokemonForms)
                .HasForeignKey(pt => pt.FormId);

            modelBuilder.Entity<PokemonToType>()
                .HasKey(pf => new { pf.PokemonEntityId, pf.PokemonTypeId });

            modelBuilder.Entity<PokemonToType>()
                .HasOne(pt => pt.PokemonEntity)
                .WithMany(p => p.PokemonToTypes)
                .HasForeignKey(pt => pt.PokemonEntityId);

            modelBuilder.Entity<PokemonToType>()
                .HasOne(pt => pt.PokemonType)
                .WithMany(p => p.PokemonToTypes)
                .HasForeignKey(pt => pt.PokemonTypeId);

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
            options.LogTo(Console.WriteLine, LogLevel.Warning)
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging(true);
        }
#endif

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
#if RELEASE
            Console.WriteLine("Start saving database");
            var result = await base.SaveChangesAsync(cancellationToken);
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("sync", cancellationToken);
            return result;
#else
            return await base.SaveChangesAsync(cancellationToken);
#endif
        }
    }

    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
