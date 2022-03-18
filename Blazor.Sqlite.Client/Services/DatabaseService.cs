using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Blazor.Sqlite.Client.Services
{
    public class DatabaseService<T>
        where T : DbContext
    {
#if DEBUG
        private static string filename = "app.db";
#else
        private static string filename = "/database/app.db";
#endif
        private readonly IDbContextFactory<T> _dbContextFactory;

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;


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
                var module = await _moduleTask.Value;
                await module.InvokeVoidAsync("mountAndInitializeDb");
                if (!File.Exists(filename))
                {
                    File.Create(filename).Close();
                }

                var dbContext = await _dbContextFactory.CreateDbContextAsync();
                await dbContext.Database.EnsureCreatedAsync();     
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().Name, ex.Message);
            }
        }
    }
}
