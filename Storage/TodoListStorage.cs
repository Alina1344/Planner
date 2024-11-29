using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Storage;

namespace Storage
{
    public class TodoListStorage : ITodoListStorage
    {
        private readonly IDatabaseRepository<TodoList> _todoListRepository;
        private readonly IDatabaseRepository<Todo> _todoRepository;

        // Конструктор для инициализации репозиториев с интерфейсами
        public TodoListStorage(IDatabaseRepository<TodoList> todoListRepository, IDatabaseRepository<Todo> todoRepository)
        {
            _todoListRepository = todoListRepository ?? throw new ArgumentNullException(nameof(todoListRepository));
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
        }

        // Конструктор с жестко заданной строкой подключения и именем таблицы
        // Используется для создания репозиториев с конкретными реализациями
        public TodoListStorage()
        {
            _todoListRepository = new DatabaseRepository<TodoList>(
                "Host=localhost;Port=1111;Username=postgres;Password=alina13122003;Database=postgres",
                "todolists");

            _todoRepository = new DatabaseRepository<Todo>(
                "Host=localhost;Port=1111;Username=postgres;Password=alina13122003;Database=postgres",
                "todos");
        }

        // Получение списка всех списков задач
        public async Task<List<TodoList>> GetAllTodoListsAsync(CancellationToken cancellationToken)
        {
            var todoLists = await _todoListRepository.GetListAsync(null, null, cancellationToken);
            return todoLists.ToList();
        }

        // Получение списка задач по владельцу (пользователю)
        public async Task<List<TodoList>> GetUserTodoListsAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId не может быть пустым.", nameof(userId));

            var todoLists = await _todoListRepository.GetListAsync("ownerid = @OwnerId", new { OwnerId = userId }, cancellationToken);
            return todoLists.ToList();
        }

        // Добавление нового списка задач
        public async Task AddTodoListAsync(TodoList todoList, CancellationToken cancellationToken)
        {
            if (todoList == null)
                throw new ArgumentNullException(nameof(todoList));

            await _todoListRepository.AddAsync(todoList, cancellationToken);
        }

        // Обновление списка задач
        public async Task UpdateTodoListAsync(Guid todoListId, TodoList updatedTodoList, CancellationToken cancellationToken)
        {
            if (updatedTodoList == null)
                throw new ArgumentNullException(nameof(updatedTodoList));

            await _todoListRepository.UpdateAsync("id = @Id", new { Id = todoListId }, updatedTodoList, cancellationToken);
        }

        // Удаление списка задач
        public async Task DeleteTodoListAsync(Guid todoListId, CancellationToken cancellationToken)
        {
            // Удаляем все связанные задачи перед удалением списка задач
            await _todoRepository.DeleteAsync("todolist_id = @TodoListId", new { TodoListId = todoListId }, cancellationToken);

            await _todoListRepository.DeleteAsync("id = @Id", new { Id = todoListId }, cancellationToken);
        }

        // Получение конкретного списка задач по ID
        public async Task<TodoList?> GetTodoListByIdAsync(Guid todoListId, CancellationToken cancellationToken)
        {
            return await _todoListRepository.GetSingleAsync("id = @Id", new { Id = todoListId }, cancellationToken);
        }

        // Получение всех задач по идентификатору списка задач
        public async Task<List<Todo>> GetTodosByTodoListIdAsync(Guid todoListId, CancellationToken cancellationToken)
        {
            return await _todoRepository.GetListAsync("todolist_id = @TodoListId", new { TodoListId = todoListId }, cancellationToken);
        }

        // Получение всех задач
        public async Task<IReadOnlyCollection<Todo>> GetAllTodosAsync(CancellationToken cancellationToken)
        {
            var todos = await _todoRepository.GetListAsync(null, null, cancellationToken);
            return todos.ToList().AsReadOnly();
        }

        // Получение задач по тегу
        public async Task<IReadOnlyCollection<Todo>> GetTodosByTagAsync(string tag, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Tag не может быть пустым.", nameof(tag));

            var todos = await _todoRepository.GetListAsync("tags ILIKE @Tag", new { Tag = "%" + tag + "%" }, cancellationToken);
            return todos.ToList().AsReadOnly();
        }

        // Получение задач по крайнему сроку выполнения
        public async Task<IReadOnlyCollection<Todo>> GetTodosByDeadlineAsync(DateTime deadline, CancellationToken cancellationToken)
        {
            var todos = await _todoRepository.GetListAsync("deadline <= @Deadline", new { Deadline = deadline }, cancellationToken);
            return todos.ToList().AsReadOnly();
        }
    }
}
