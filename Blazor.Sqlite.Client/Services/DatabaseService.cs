using Blazor.Sqlite.Client.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Blazor.Sqlite.Client.Services
{
    public class DatabaseService : IDisposable
    {
#if DEBUG
        private static string filename = "app.db";
#else
        private static string filename = "/database/app.db";
#endif
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private readonly DotNetObjectReference<DatabaseService> _selfReference;
        

        public event EventHandler<SyncFinishedEvent> SyncFinished;

#if DEBUG
        public DatabaseService()
        {
        }
#else
        public DatabaseService(IJSRuntime jsRuntime
            , IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            if (jsRuntime == null) throw new ArgumentNullException(nameof(jsRuntime));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./js/file.js").AsTask());
            _selfReference = DotNetObjectReference.Create(this);
        }
#endif

        [JSInvokable]
        public void FinishSync(bool successful)
        {
            SyncFinished?.Invoke(this, new SyncFinishedEvent
            {
                Successfull = successful
            });
        }

        public async Task InitDatabaseAsync()
        {
            try
            {
#if RELEASE
                var module = await _moduleTask.Value;
                await module.InvokeVoidAsync("mount", _selfReference);
                if (!File.Exists(filename))
                {
                    File.Create(filename).Close();
                }

                var dbContext = await _dbContextFactory.CreateDbContextAsync();
                await dbContext.Database.EnsureCreatedAsync();
#else
                SyncFinished?.Invoke(this, new SyncFinishedEvent
                {
                    Successfull = true
                });
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().Name, ex.Message);
            }
        }
        
        public void Dispose() => _selfReference?.Dispose();
    }

    public class SyncFinishedEvent : EventArgs
    {
        public bool Successfull { get; set; }
    }
}
