using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Storage
{
    public class TodoListStorage : ITodoListStorage
    {
        private readonly IFileStorage<TodoList> _repository;

        public TodoListStorage(IFileStorage<TodoList> repository)
        {
            _repository = repository;
        }

        public TodoListStorage()
        {
            _repository = new FileStorage<TodoList>("../../data/TodoLists.json", "todolists");
        }

        public async Task<IReadOnlyCollection<TodoList>> GetUserTodoListsAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var todoLists = await _repository.GetAllAsync(token);
            return todoLists.Where(t => t.OwnerId == userId).ToList().AsReadOnly();
        }

        public async Task AddTodoListAsync(TodoList todoList, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.AddAsync(todoList, token);
        }

        public async Task DeleteTodoListAsync(Guid todoListId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.DeleteAsync(t => t.Id == todoListId, token);
        }

        public async Task UpdateTodoListAsync(TodoList todoList, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.UpdateAsync(t => t.Id == todoList.Id, todoList, token);
        }

        public async Task<IReadOnlyCollection<Todo>> GetAllTodosAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var todoLists = await _repository.GetAllAsync(token);
            return todoLists.SelectMany(t => t.Todos).ToList().AsReadOnly();
        }
        public async Task<IReadOnlyCollection<Todo>> GetTodosByTagAsync(string tag, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var todoLists = await _repository.GetAllAsync(token);
            return todoLists
                .SelectMany(t => t.Todos)
                .Where(todo => todo.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                .ToList()
                .AsReadOnly();
        }
    
        public async Task<IReadOnlyCollection<Todo>> GetTodosByDeadlineAsync(DateTime deadline, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var todoLists = await _repository.GetAllAsync(token);
            return todoLists
                .SelectMany(t => t.Todos)
                .Where(todo => todo.Deadline <= deadline)
                .ToList()
                .AsReadOnly();
        }
    }

}
