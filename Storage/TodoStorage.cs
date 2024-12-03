using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Storage
{
    public class TodoStorage : ITodoStorage
    {
        private readonly DatabaseRepository<Todo> _repository;

        public TodoStorage(DatabaseRepository<Todo> repository)
        {
            _repository = repository;
        }
        
        // Конструктор с жестко заданной строкой подключения и именем таблицы
        public TodoStorage()
        {
            _repository = new DatabaseRepository<Todo>(
                "Host=localhost;Port=1111;Username=postgres;Password=alina13122003;Database=postgres",
                "todos");
        }

        // Получение задач по ID списка
        public async Task<IReadOnlyCollection<Todo>> LoadTodosByWishlistIdAsync(string todoListIdString, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!Guid.TryParse(todoListIdString, out Guid todoListId))
            {
                throw new ArgumentException("Invalid TodoListId format.", nameof(todoListIdString));
            }

            return await _repository.GetListAsync(
                "todolistid = @TodoListId",
                new { TodoListId = todoListId },
                token);
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
            await _repository.DeleteAsync("id = @Id", new { Id = todoId }, token);
        }

        // Отметка задачи как выполненной
        public async Task CompleteTodoAsync(Guid todoId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var todos = await _repository.GetListAsync("id = @Id", new { Id = todoId }, token);
            var todo = todos.FirstOrDefault();

            if (todo == null)
                throw new Exception("Task not found");

            var updatedTodo = todo with { IsCompleted = true };
            await _repository.UpdateAsync("id = @Id", new { Id = todoId }, updatedTodo, token);
        }

        // Получение выполненных задач
        public async Task<IReadOnlyCollection<Todo>> GetCompletedTodosAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return await _repository.GetListAsync("iscompleted = true", null, token);
        }

        // Поиск задач по тегу
        public async Task<IReadOnlyCollection<Todo>> SearchTodosByTagAsync(string tag, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return await _repository.GetListAsync(
                "tags LIKE @Tag",
                new { Tag = $"%{tag}%" },
                token);
        }

        // Поиск задач по ключевому слову
        public async Task<IReadOnlyCollection<Todo>> SearchTodosByKeywordAsync(string keyword, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return await _repository.GetListAsync(
                "(title LIKE @Keyword OR description LIKE @Keyword) AND is_completed = false",
                new { Keyword = $"%{keyword}%" },
                token);
        }

        // Получение всех задач, отсортированных по дедлайну
        public async Task<IReadOnlyCollection<Todo>> GetAllTodosAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return await _repository.GetListAsync("1=1 ORDER BY deadline ASC", null, token);
        }

        // Резервирование задачи
        public async Task ReserveTodoAsync(Guid todoId, string reserverId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var todos = await _repository.GetListAsync("id = @Id", new { Id = todoId }, token);
            var todo = todos.FirstOrDefault();

            if (todo == null)
                throw new Exception("Task not found");

            var updatedTodo = todo with { OwnerId = reserverId };
            await _repository.UpdateAsync("id = @Id", new { Id = todoId }, updatedTodo, token);
        }

        // Загрузка зарезервированных задач пользователя
        public async Task<IReadOnlyCollection<Todo>> LoadReservedTodosByUserIdAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return await _repository.GetListAsync(
                "owner_id = @UserId AND is_completed = false",
                new { UserId = userId },
                token);
        }

        public async Task<Todo?> GetTodoAsync(string toString, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Проверяем, можно ли преобразовать строку в Guid
            if (!Guid.TryParse(toString, out var todoId))
            {
                throw new ArgumentException("Invalid ID format.", nameof(toString));
            }

            // Ищем задачу по ID
            var todos = await _repository.GetListAsync("id = @Id", new { Id = todoId }, cancellationToken);

            // Возвращаем первую найденную задачу или null
            return todos.FirstOrDefault();
        }
        
    }
}
