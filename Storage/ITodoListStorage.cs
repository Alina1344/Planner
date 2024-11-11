using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Storage
{
    public interface ITodoListStorage
    {
        Task<IReadOnlyCollection<TodoList>> GetUserTodoListsAsync(string userId, CancellationToken token);
        Task AddTodoListAsync(TodoList todoList, CancellationToken token);
        Task DeleteTodoListAsync(Guid todoListId, CancellationToken token);
        Task UpdateTodoListAsync(TodoList todoList, CancellationToken token);
        Task<IReadOnlyCollection<Todo>> GetAllTodosAsync(CancellationToken token);
        Task<IReadOnlyCollection<Todo>> GetTodosByTagAsync(string tag, CancellationToken token);
        Task<IReadOnlyCollection<Todo>> GetTodosByDeadlineAsync(DateTime deadline, CancellationToken token);

    }
}
