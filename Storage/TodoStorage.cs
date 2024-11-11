using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Storage
{
    public class TodoStorage : ITodoStorage
    {
        private readonly IFileStorage<Todo> _repository;
        private ITodoStorage _todoStorageImplementation;

        public TodoStorage()
        {
            _repository = new FileStorage<Todo>("../../data/Todos.json", "todos");
        }

        public TodoStorage(IFileStorage<Todo> repository)
        {
            _repository = repository;
        }
   

        // Получение задач по ID списка
        public async Task<IReadOnlyCollection<Todo>> LoadTodosByWishlistIdAsync(string todoListIdString, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!Guid.TryParse(todoListIdString, out Guid todoListId))
            {
                throw new ArgumentException("Invalid TodoListId format.", nameof(todoListIdString));
            }

            var todos = await _repository.GetAllAsync(token);
            return todos.Where(t => t.TodoListId == todoListId).ToList();
        }

        // Добавление задачи
        public async Task AddTodoAsync(Todo todo, CancellationToken token)
        {
            if (todo == null)
            {
                throw new ArgumentNullException(nameof(todo));
            }
            token.ThrowIfCancellationRequested();
            await _repository.AddAsync(todo, token);
        }

        // Удаление задачи по ID
        public async Task DeleteTodoAsync(Guid todoId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.DeleteAsync(t => t.Id == todoId, token);
        }
        

        // Отметка задачи как выполненной
        public async Task CompleteTodoAsync(Guid todoId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Todo> todos = await _repository.GetAllAsync(token);

            var todo = todos.FirstOrDefault(t => t.Id == todoId);

            if (todo == null)
                throw new Exception("Task not found");

            var updatedTodo = todo with { IsCompleted = true };
            await _repository.UpdateAsync(t => t.Id == todoId, updatedTodo, token);
        }

        // Получение выполненных задач
        public async Task<IReadOnlyCollection<Todo>> GetCompletedTodosAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Todo> todos = await _repository.GetAllAsync(token);
            return todos.Where(t => t.IsCompleted).ToList();
        }

        // Поиск задач по тегу
        public async Task<IReadOnlyCollection<Todo>> SearchTodosByTagAsync(string tag, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Todo> todos = await _repository.GetAllAsync(token);
            return todos.Where(t => t.Tags.Contains(tag)).ToList();
        }
        
        // Поиск задач по ключевому слову
        public async Task<IReadOnlyCollection<Todo>> SearchTodosByKeywordAsync(string keyword, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Todo> todos = await _repository.GetAllAsync(token);

            return todos.Where(t =>
                    !t.IsCompleted &&
                    (t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        // Фильтрация задач по срокам
        public async Task<IReadOnlyCollection<Todo>> FilterTodosByDeadlineAsync(DateTime deadline, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Todo> todos = await _repository.GetAllAsync(token);
            return todos.Where(t => t.Deadline <= deadline).ToList();
        }

        // Резервирование задачи
        public async Task ReserveTodoAsync(Guid todoId, string reserverId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Todo> todos = await _repository.GetAllAsync(token);

            var todo = todos.FirstOrDefault(t => t.Id == todoId);

            if (todo == null)
                throw new Exception("Task not found");

            var updatedTodo = todo with { OwnerId = reserverId };
            await _repository.UpdateAsync(t => t.Id == todoId, updatedTodo, token);
        }

        // Загрузка зарезервированных задач пользователя
        public async Task<IReadOnlyCollection<Todo>> LoadReservedTodosByUserIdAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Todo> todos = await _repository.GetAllAsync(token);
            return todos.Where(t => t.OwnerId == userId && !t.IsCompleted).ToList();
        }
    }
}
