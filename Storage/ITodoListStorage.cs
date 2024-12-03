using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Storage
{
    public interface ITodoListStorage
    {
        Task<List<TodoList>> GetAllTodoListsAsync(CancellationToken cancellationToken);
        Task<List<TodoList>> GetUserTodoListsAsync(string userId, CancellationToken cancellationToken);
        Task AddTodoListAsync(TodoList todoList, CancellationToken cancellationToken);
        Task UpdateTodoListAsync(Guid todoListId, TodoList updatedTodoList, CancellationToken cancellationToken);
        Task DeleteTodoListAsync(Guid todoListId, CancellationToken cancellationToken);
        Task<TodoList?> GetTodoListByIdAsync(Guid todoListId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Todo>> GetAllTodosAsync(CancellationToken cancellationToken);
        Task<List<Todo>> GetTodosByTodoListIdAsync(Guid todoListId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Todo>> GetTodosByDeadlineAsync(DateTime deadline, CancellationToken cancellationToken);
    }

}