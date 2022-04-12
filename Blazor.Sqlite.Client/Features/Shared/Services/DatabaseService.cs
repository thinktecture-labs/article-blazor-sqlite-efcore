using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Blazor.Sqlite.Client.Features.Shared.Services
{
    public class DatabaseService<T> : IDisposable
        where T : DbContext
    {
        public event EventHandler<EventArgs> DatabaseChanged;

#if RELEASE
        public static string FileName = "/database/app.db";
        private readonly IDbContextFactory<T> _dbContextFactory;
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private DotNetObjectReference<DatabaseService<T>>? _reference;
#endif



#if DEBUG
        public DatabaseService()
        {
        }
#else
        public DatabaseService(IJSRuntime jsRuntime
            , IDbContextFactory<T> dbContextFactory)
        {
            if (jsRuntime == null) throw new ArgumentNullException(nameof(jsRuntime));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./js/file.js").AsTask());
        }
#endif

        public async Task InitDatabaseAsync()
        {
            try
            {
#if RELEASE
                if (_reference == null)
                {
                    _reference = DotNetObjectReference.Create(this);
                }
                var module = await _moduleTask.Value;
                await module.InvokeVoidAsync("mountAndInitializeDb");
                await module.InvokeVoidAsync("registerStorageChangedEvent", _reference);
                if (!File.Exists(FileName))
                {
                    File.Create(FileName).Close();
                }

                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                await dbContext.Database.EnsureCreatedAsync();     
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().Name, ex.Message);
            }
        }

        public async Task SyncDatabase(CancellationToken cancellationToken)
        {
            Console.WriteLine("Start saving database");
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("syncDatabase", false, cancellationToken);
            await module.InvokeVoidAsync("writeIndexedDbChange", cancellationToken);
            Console.WriteLine("Finish save database");
        }

        [JSInvokable]
        public async Task HandleStorageChanged()
        {
            Console.WriteLine("Local storage changed!");
#if RELEASE
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("syncDatabase", true);
            DatabaseChanged?.Invoke(this, new EventArgs());
#endif
        }

        public void Dispose() => _reference?.Dispose();
    }
}
