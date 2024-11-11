using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models; 

namespace Presenter
{
    public interface ITodoListPresenter
    {
        Task<IReadOnlyCollection<TodoList>> LoadUserTodolistAsync(string userId, CancellationToken token);
        Task AddNewTodolistAsync(string title, string description, string ownerId, CancellationToken token);
        Task DeleteTodolistAsync(Guid todolistId, CancellationToken token);
        Task UpdateTodolistAsync(TodoList todoList, CancellationToken token);
        Task<IReadOnlyCollection<Todo>> FilterTodosByDeadlineAsync(DateTime deadline, CancellationToken token);
    }
}
