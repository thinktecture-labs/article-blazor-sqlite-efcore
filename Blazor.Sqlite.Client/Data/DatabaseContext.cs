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
        public DbSet<PokemonOverview> Pokemons { get; set; }
        public DbSet<PokemonEntity> PokemonData { get; set; }
        public DbSet<Sprites> Sprites { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<PokemonLanguage> PokemonLanguages { get; set; }
        public DbSet<PokemonName> PokemonNames { get; set; }
        public DbSet<FlavorTextEntry> FlavorTextEntries { get; set; }
        public DbSet<PokemonColor> PokemonColors { get; set; }
        public DbSet<PokemonType> PokemonTypes { get; set; }
        public DbSet<PokemonForm> PokemonForms { get; set; }
        public DbSet<PokemonToType> PokemonToTypes { get; set; }
        public DbSet<Stat> Stats { get; set; }
        public DbSet<PokemonStat> PokemonStats { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<ContributionSpeaker> ContributionSpeakers { get; set; }

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public DatabaseContext(DbContextOptions<DatabaseContext> options
            , IJSRuntime jsRuntime)
        : base(options)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./js/file.js").AsTask());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PokemonColor>()
               .HasMany(c => c.PokemonEntities)
               .WithOne(e => e.PokemonColor)
               .HasForeignKey(c => c.PokemonColorId);

            modelBuilder.Entity<PokemonLanguage>()
               .HasMany(l => l.PokemonNames)
               .WithOne(e => e.Language)
               .HasForeignKey(l => l.LanguageId);

            modelBuilder.Entity<PokemonLanguage>()
               .HasMany(c => c.FlavorTextEntries)
               .WithOne(e => e.Language)
               .HasForeignKey(c => c.LanguageId);


            modelBuilder.Entity<FlavorTextEntry>()
                .HasOne(c => c.PokemonEntity)
                .WithMany(e => e.FlavorTextEntries)
                .HasForeignKey(e => e.PokemonEntityId);

            modelBuilder.Entity<PokemonEntity>()
               .HasMany(c => c.PokemonNames)
               .WithOne(e => e.PokemonEntity)
               .HasForeignKey(e => e.PokemonEntityId);

            modelBuilder.Entity<PokemonStat>()
                .HasKey(ps => new { ps.PokemonEntityId, ps.StatId });

            modelBuilder.Entity<PokemonStat>()
                .HasOne(pt => pt.PokemonEntity)
                .WithMany(p => p.PokemonStats)
                .HasForeignKey(pt => pt.PokemonEntityId);

            modelBuilder.Entity<PokemonStat>()
                .HasOne(pt => pt.Stat)
                .WithMany(p => p.PokemonStats)
                .HasForeignKey(pt => pt.StatId);

            modelBuilder.Entity<PokemonForm>()
                .HasKey(pf => new { pf.PokemonEntityId, pf.FormId });

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
            await module.InvokeVoidAsync("syncDatabase", cancellationToken);
            Console.WriteLine("Finish save database");
        }
    }

    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
