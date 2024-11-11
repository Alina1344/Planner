using Models;
using Presenter;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace View
{
    public class TodoView : ITodoView
    {
        private TodoPresenter _todoPresenter;
        private TodoListPresenter _todolistPresenter;

        public TodoView()
        {
            _todoPresenter = new TodoPresenter();
        }

        public async Task StartTodos(User user, TodoList todolist, bool update)
        {
            string userId = user.Id;
            string todolistId = todolist.Id.ToString();  // Приведение Guid к строке
            string todolistName = todolist.Title;
            bool continueRunning = true;

            while (continueRunning)
            {
                Console.Clear();
                Console.WriteLine($"Задачи в списке задач: {todolistName}");

                await ShowUserTodos(todolistId);

                // Базовые действия: выполнить задачу и выйти
                var menuActions = new Dictionary<int, Func<Task>>()
                {
                    { 1, async () => await MarkTodoAsCompleted(todolistId) }, // Выполнить задачу
                    { 2, () => Task.FromResult(continueRunning = false) }      // Назад
                };

                var menuLabels = new Dictionary<int, string>()
                {
                    { 1, "Выполнить задачу" },
                    { 2, "Назад" }
                };

                // Если update == true, добавляем опции "Добавить задачу" и "Удалить задачу"
                if (update)
                {
                    menuActions.Add(3, async () => await AddTodo(userId, todolistId));
                    menuActions.Add(4, async () => await DeleteTodo(todolistId));
            
                    menuLabels.Add(3, "Добавить задачу");
                    menuLabels.Add(4, "Удалить задачу");
                }

                var menuView = new MenuView(menuActions, menuLabels);
                await menuView.ExecuteMenuChoice();

                if (continueRunning)
                {
                    Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Загрузка...");
        }

        
        public async Task ShowUserTodos(string wishlistId)
        {
            CancellationToken token = new CancellationToken();
            try
            {
                IReadOnlyCollection<Todo> todos = await _todoPresenter.LoadTodoListTodosAsync(wishlistId, token);

                if (todos == null || todos.Count == 0)
                {
                    Console.WriteLine("В этом списке задач пока пусто.");
                    return;
                }

                foreach (var present in todos)
                {
                    Console.WriteLine($"Имя: {present.Title}, Описание: {present.Description}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке задачи: {ex.Message}");
            }
        }

       

        public async Task AddTodo(string userId, string wishlistId)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                Console.WriteLine("Создание задачи");

                // Ввод названия задачи
                string p_name;
                do
                {
                    Console.Write("Введите название задачи: ");
                    p_name = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(p_name))
                    {
                        Console.WriteLine("Название задачи не может быть пустым. Пожалуйста, введите корректное название.");
                    }
                }
                while (string.IsNullOrWhiteSpace(p_name));

                // Ввод описания задачи
                string p_description;
                do
                {
                    Console.Write("Введите комментарий: ");
                    p_description = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(p_description))
                    {
                        Console.WriteLine("Описание не может быть пустым. Пожалуйста, введите комментарий.");
                    }
                }
                while (string.IsNullOrWhiteSpace(p_description));

                // Ввод даты дедлайна
                DateTime p_deadline;
                while (true)
                {
                    Console.Write("Введите дедлайн задачи (в формате ГГГГ-ММ-ДД): ");
                    string deadlineInput = Console.ReadLine();

                    if (DateTime.TryParse(deadlineInput, out p_deadline))
                    {
                        break; // Успешный ввод
                    }
                    Console.WriteLine("Некорректная дата. Пожалуйста, введите дату в правильном формате.");
                }

                // Ввод тегов
                List<string> tags = new List<string>();
                Console.WriteLine("Введите теги для задачи (нажмите Enter для завершения ввода):");
                while (true)
                {
                    string tag = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(tag))
                    {
                        break; // Завершить ввод тегов
                    }
                    tags.Add(tag);
                }

                // Добавление новой задачи
                await _todoPresenter.AddNewTodoAsync(p_name, p_description, userId, wishlistId, p_deadline, tags, token);
                Console.WriteLine("Задача успешно создана.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }



        public async Task DeleteTodo(string wishlistId)
        {
            CancellationToken token = new CancellationToken();
            try
            {
                // Получаем все задачи для отображения пользователю
                IReadOnlyCollection<Todo> todos = await _todoPresenter.LoadTodoListTodosAsync(wishlistId, token);

                if (todos == null || todos.Count == 0)
                {
                    Console.WriteLine("Нет задач для удаления.");
                    return;
                }

                // Отображаем список задач с их номерами
                Console.WriteLine("Выберите задачу для удаления:");
                int index = 1;
                List<Guid> todoIds = new List<Guid>(); // Список для хранения идентификаторов задач
                foreach (var todo in todos)
                {
                    todoIds.Add(todo.Id);
                    Console.WriteLine($"{index}. Имя: {todo.Title}");
                    index++;
                }

                Console.Write("Введите номер задачи для удаления: ");
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= todos.Count)
                {
                    // Получаем ID задачи по номеру
                    Guid todoId = todoIds[taskNumber - 1]; // Номера начинаются с 1, индексы с 0

                    // Вызываем метод для удаления задачи
                    await _todoPresenter.DeleteTodoAsync(todoId, token);
                    Console.WriteLine("Задача успешно удалена.");
                }
                else
                {
                    Console.WriteLine("Неверный номер задачи. Пожалуйста, введите корректный номер.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при удалении задачи: {e.Message}");
            }
        }
       

        public async Task MarkTodoAsCompleted(string wishlistId)
        {
            CancellationToken token = new CancellationToken();
            try
            {
                // Получаем все задачи для отображения пользователю
                IReadOnlyCollection<Todo> todos = await _todoPresenter.LoadTodoListTodosAsync(wishlistId, token);

                if (todos == null || todos.Count == 0)
                {
                    Console.WriteLine("Нет задач для выполнения.");
                    return;
                }

                // Отображаем список задач с их номерами
                Console.WriteLine("Выберите задачу для пометки как выполненную:");
                int index = 1;
                List<Guid> todoIds = new List<Guid>(); // Список для хранения идентификаторов задач
                foreach (var todo in todos)
                {
                    todoIds.Add(todo.Id);
                    Console.WriteLine($"{index}. Имя: {todo.Title}");
                    index++;
                }

                Console.Write("Введите номер задачи для выполнения: ");
                if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= todos.Count)
                {
                    // Получаем ID задачи по номеру
                    Guid todoId = todoIds[taskNumber - 1]; // Номера начинаются с 1, индексы с 0

                    // Вызываем метод для пометки задачи как выполненной
                    await _todoPresenter.CompleteTodoAsync(todoId, token);
                    Console.WriteLine("Задача успешно помечена как выполненная.");
                }
                else
                {
                    Console.WriteLine("Неверный номер задачи. Пожалуйста, введите корректный номер.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при пометке задачи как выполненной: {e.Message}");
            }
        }
        
        public async Task ShowSearchedTodoAsync(CancellationToken token, User user)
        {
            try
            {
                Console.Write("Введите тег для поиска задач: ");
                string tag = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(tag))
                {
                    Console.WriteLine("Тег не может быть пустым.");
                    return;
                }

                // Используем метод поиска задач по тегу из TodoPresenter
                IReadOnlyCollection<Todo> searchedTodos = await _todoPresenter.SearchTodosByTagAsync(tag, token);

                if (searchedTodos == null || searchedTodos.Count == 0)
                {
                    Console.WriteLine("Задачи с данным тегом не найдены.");
                    return;
                }

                Console.WriteLine("Найденные задачи:");
                foreach (var todo in searchedTodos)
                {
                    Console.WriteLine($"Имя: {todo.Title}, Описание: {todo.Description}, Дедлайн: {todo.Deadline}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске задач: {ex.Message}");
            }
        }


        public async Task ShowCompletTodo(CancellationToken token, User user)
        {
            try
            {
                // Получаем выполненные задачи пользователя
                var completedTodos = await _todoPresenter.GetCompletedTodosAsync(token);
        
                if (completedTodos == null || completedTodos.Count == 0)
                {
                    Console.WriteLine("Выполненные задачи отсутствуют.");
                    return;
                }

                // Выводим выполненные задачи
                for (int i = 0; i < completedTodos.Count; i++)
                {
                    var todo = completedTodos.ElementAt(i);
                    Console.WriteLine($"{i + 1}. Имя: {todo.Title}, Описание: {todo.Description}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении выполненных задач: {ex.Message}");
            }
        }
        
    }
}