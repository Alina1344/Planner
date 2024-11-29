using Models;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter
{
    public class TodoListPresenter : ITodoListPresenter
    {
        private readonly ITodoListStorage _wishlistRepository;
        private readonly IUserPresenter _userPresenter;

        public TodoListPresenter(ITodoListStorage wishlistRepository, IUserPresenter userPresenter)
        {
            _wishlistRepository = wishlistRepository;
            _userPresenter = userPresenter;
        }

        public TodoListPresenter()
        {
            _wishlistRepository = new TodoListStorage();
            _userPresenter = new UserPresenter();
        }

        // Метод для загрузки всех списков задач пользователя
        public async Task<IReadOnlyCollection<TodoList>> LoadUserTodolistAsync(string userId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            token.ThrowIfCancellationRequested();

            User user = await _userPresenter.LoadUserAsync(userId, token);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.", nameof(userId));
            }

            var todoLists = await _wishlistRepository.GetUserTodoListsAsync(userId, token);
            return todoLists;
        }

        // Метод для добавления нового списка задач
        public async Task AddNewTodolistAsync(string title, string description, string ownerId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(title));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }

            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentException("OwnerId cannot be null or empty.", nameof(ownerId));
            }

            var todoList = new TodoList(Guid.NewGuid(), title, ownerId);
            token.ThrowIfCancellationRequested();
            await _wishlistRepository.AddTodoListAsync(todoList, token);
        }

        // Метод удаления списка задач
        public async Task DeleteTodolistAsync(Guid todoListId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _wishlistRepository.DeleteTodoListAsync(todoListId, token);
        }

        
       // Метод обновления списка задач
       public async Task UpdateTodolistAsync(TodoList todoList, CancellationToken token)
       {
           if (todoList == null)
           {
               throw new ArgumentNullException(nameof(todoList), "Todo list cannot be null.");
           }
       
           token.ThrowIfCancellationRequested();
       
           // Извлекаем Id из объекта TodoList и передаём его в метод
           await _wishlistRepository.UpdateTodoListAsync(todoList.Id, todoList, token);
       }


        // Метод фильтрации задач по сроку
        public async Task<IReadOnlyCollection<Todo>> FilterTodosByDeadlineAsync(DateTime deadline, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var allTodos = await _wishlistRepository.GetAllTodosAsync(token) as IReadOnlyCollection<Todo>;

            var filteredTodos = allTodos.Where(todo => todo.Deadline <= deadline).ToList();

            return filteredTodos.AsReadOnly();
        }
    }
}
