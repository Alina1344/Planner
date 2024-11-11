using Models;
using Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter
{
    public class TodoPresenter : ITodoPresenter
    {
        private readonly TodoStorage _todoStorage;
        
        public TodoPresenter()
        {
            _todoStorage = new TodoStorage();
        }
        

        public Task<IReadOnlyCollection<Todo>> LoadTodoListTodosAsync(string wishlistId, CancellationToken token)
        {
            return _todoStorage.LoadTodosByWishlistIdAsync(wishlistId, token);
        }

       public async Task<IReadOnlyCollection<Todo>> SearchTodosByKeywordAsync(string keyword, CancellationToken token)
               {
                   if (string.IsNullOrWhiteSpace(keyword))
                   {
                       return Array.Empty<Todo>();
                   }
                   return await _todoStorage.SearchTodosByKeywordAsync(keyword, token);
               }


        public async Task<IReadOnlyCollection<Todo>> LoadReservedTodosAsync(string userId, CancellationToken token)
        {
            return await _todoStorage.LoadReservedTodosByUserIdAsync(userId, token);
        }

        public async Task AddNewTodoAsync(string title, string description, string ownerId, string todoListId, DateTime deadline, List<string> tags, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(ownerId) || string.IsNullOrWhiteSpace(todoListId))
            {
                throw new ArgumentNullException("Parameters cannot be null or empty.");
            }

            var todo = new Todo(
                Guid.NewGuid(),
                title,
                description,
                deadline,
                tags, // Добавление тегов
                false,
                Guid.Parse(todoListId),
                ownerId
            );

            token.ThrowIfCancellationRequested();
            await _todoStorage.AddTodoAsync(todo, token);
        }


        public async Task DeleteTodoAsync(Guid todoId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _todoStorage.DeleteTodoAsync(todoId, token);
        }

        public async Task ReserveTodoAsync(Guid todoId, string ownerId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentNullException(nameof(ownerId));
            }
            token.ThrowIfCancellationRequested();
            await _todoStorage.ReserveTodoAsync(todoId, ownerId, token);
        }
        
        public async Task CompleteTodoAsync(Guid todoId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // Попробуем отметить задачу как выполненную, используя метод хранилища
            await _todoStorage.CompleteTodoAsync(todoId, token);

            // Загружаем выполненные задачи для обновления UI или списка задач, если это требуется
            IReadOnlyCollection<Todo> completedTodos = await _todoStorage.GetCompletedTodosAsync(token);

            if (completedTodos.All(t => t.Id != todoId))
            {
                throw new Exception("Задача не найдена или уже выполнена.");
            }
        }
        public async Task<IReadOnlyCollection<Todo>> GetCompletedTodosAsync(CancellationToken token)
        {
            return await _todoStorage.GetCompletedTodosAsync(token);
        }

        public async Task<IReadOnlyCollection<Todo>> SearchTodosByTagAsync(string tag, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }
            token.ThrowIfCancellationRequested();
            return await _todoStorage.SearchTodosByTagAsync(tag, token);
        }

        public async Task<IReadOnlyCollection<Todo>> FilterTodosByDeadlineAsync(DateTime deadline, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return await _todoStorage.FilterTodosByDeadlineAsync(deadline, token);
        }
    }
}