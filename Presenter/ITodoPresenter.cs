using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter;
     
public interface ITodoPresenter
{
    Task<IReadOnlyCollection<Todo>> LoadTodoListTodosAsync(string wishlistId, CancellationToken token);

    Task<IReadOnlyCollection<Todo>> LoadReservedTodosAsync(string userId, CancellationToken token);
 
    Task DeleteTodoAsync(Guid presentId, CancellationToken token);
    Task ReserveTodoAsync(Guid presentId, string reserverId, CancellationToken token);

}