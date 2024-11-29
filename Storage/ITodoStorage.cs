using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Storage
{
    public interface ITodoStorage
    {
        Task<IReadOnlyCollection<Todo>> LoadTodosByWishlistIdAsync(string todoListIdString, CancellationToken token);
        Task AddTodoAsync(Todo todo, CancellationToken token);
        Task DeleteTodoAsync(Guid todoId, CancellationToken token);
        Task CompleteTodoAsync(Guid todoId, CancellationToken token);
        Task<IReadOnlyCollection<Todo>> GetCompletedTodosAsync(CancellationToken token);
        Task<IReadOnlyCollection<Todo>> SearchTodosByTagAsync(string tag, CancellationToken token);
        Task<IReadOnlyCollection<Todo>> SearchTodosByKeywordAsync(string keyword, CancellationToken token);
        Task<IReadOnlyCollection<Todo>> GetAllTodosAsync(CancellationToken token);
        Task ReserveTodoAsync(Guid todoId, string reserverId, CancellationToken token);
        Task<IReadOnlyCollection<Todo>> LoadReservedTodosByUserIdAsync(string userId, CancellationToken token);
    }
}
