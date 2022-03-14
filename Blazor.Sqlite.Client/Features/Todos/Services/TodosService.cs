using Blazor.Sqlite.Client.Data;
using Blazor.Sqlite.Client.Services;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Sqlite.Client.Features.Todos.Services
{
    public class TodosService
    {
        private readonly DatabaseContext _context;

        public TodosService(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<IEnumerable<TodoItem>?> GetTodosAsync()
        {
            if (_context != null)
            {
                return await _context.Todos.ToListAsync();
            }
            return null;
        }

        public async Task AddTodoItemAsync(TodoItem item)
        {
            if (_context != null)
            {
                _context.Todos.Add(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTodoItemAsync(int id)
        {
            if (_context != null)
            {
                var todo = _context.Todos.FirstOrDefault(t => t.Id == id);
                if (todo != null)
                {
                    _context.Todos.Remove(todo);
                    await _context.SaveChangesAsync();
                }
            }
        }

    }
}
