using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter;
     
public interface ITodoPresenter
{
    Task<IReadOnlyCollection<Todo>> LoadTodoListTodosAsync(string wishlistId, CancellationToken token);
    Task<IReadOnlyCollection<Todo>> LoadReservedTodosAsync(string userId, CancellationToken token);
    Task<IReadOnlyCollection<Todo>> GetCompletedTodosAsync(CancellationToken token);
    Task<IReadOnlyCollection<Todo>> SearchTodosByTagAsync(string tag, CancellationToken token);
    Task<IReadOnlyCollection<Todo>> GetAllTodosSortedByDeadlineAsync(CancellationToken token);

    Task AddNewTodoAsync(string title, string description, string ownerId, string todoListId, DateTime deadline, List<string> tags, CancellationToken token);
    Task DeleteTodoAsync(Guid todoId, CancellationToken token);
    Task ReserveTodoAsync(Guid todoId, string reserverId, CancellationToken token);
    Task CompleteTodoAsync(Guid todoId, CancellationToken token);

}