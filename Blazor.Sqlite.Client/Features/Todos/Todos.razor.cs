using Blazor.Sqlite.Client.Data;
using Blazor.Sqlite.Client.Features.Todos.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Sqlite.Client.Features.Todos
{
    public partial class Todos
    {
        [Inject] private TodosService TodosService { get; set; } = default!;

        private IEnumerable<TodoItem> _todos = new List<TodoItem>();
        private bool _isLoading = true;
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            _isLoading = false;
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            if (!_todos.Any())
            {
                _todos = await TodosService.GetTodosAsync() ?? new List<TodoItem>();
            }
        }

        private async Task AddTaskAsync()
        {
            await TodosService.AddTodoItemAsync(new TodoItem
            {
                Title = $"Test {_todos.Count() + 1}",
                Description = "Lorem ipsum dolor"
            });
            _todos = await TodosService.GetTodosAsync() ?? new List<TodoItem>();

            StateHasChanged();
        }

        private async Task DeleteTaskAsync(int id)
        {
            await TodosService.DeleteTodoItemAsync(id);
            _todos = await TodosService.GetTodosAsync() ?? new List<TodoItem>();

            StateHasChanged();
        }
    }
}