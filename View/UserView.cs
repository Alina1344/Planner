using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presenter;

namespace View
{
    //fffffffffffffff
    public class UserView(UserPresenter userPresenter, TodoListView todolistView, TodoView todoView)
        : IUserView
    {
        private bool _isProgramRunning = true;
        private bool _isLoggedIn;

        public async Task Start()
        {
            CancellationToken token = new CancellationToken();
            while (_isProgramRunning)
            {
                await AuthUser();
                if (_isProgramRunning && _isLoggedIn)
                {
                    var authenticatedUser = await userPresenter.GetAuthenticatedUserAsync(token);

                    if (authenticatedUser == null)
                    {
                        Console.WriteLine("Нет аутентифицированного пользователя. Попробуйте снова.");
                        continue;
                    }
                    else
                    {
                        await ShowUser(authenticatedUser);
                    }
                }
            }
        }

        public async Task ShowUser(User user)
        {
            try
            {
                var username = user.Name;
                _isLoggedIn = true;

                var menuActions = new Dictionary<int, Func<Task>>()
                {
                    { 1, async () => await todolistView.StartTodolist(user) },
                    { 2, async () => await ShowCompletedTodos() }, 
                    { 3, async () => await SearchTodos() },
                    { 4, async () => await ExitProgram() }
                };

                var menuLabels = new Dictionary<int, string>()
                {
                    { 1, "Посмотреть списки задач" },
                    { 2, "Посмотреть выполненные задачи" }, 
                    { 3, "Найти задачу" },
                    { 4, "Выход" }
                };

                var menuView = new MenuView(menuActions, menuLabels);

                while (_isLoggedIn)
                {
                    Console.WriteLine($"Привет, {username}!");
                    await menuView.ExecuteMenuChoice();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }
        private async Task ShowCompletedTodos()
        {
            try
            {
                CancellationToken token = new CancellationToken();
                User user = await userPresenter.GetAuthenticatedUserAsync(token);
                await todoView.ShowSearchedTodo(token, user);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при отображении выполненных задач: {e.Message}");
            }
        }
        
        private async Task SearchTodos()
        {
            try
            {
                CancellationToken token = new CancellationToken();
                User user = await userPresenter.GetAuthenticatedUserAsync(token);
                await todoView.ShowSearchedTodo(token, user); // Здесь вы вызываете метод для отображения выполненных задач
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при поиске задач: {e.Message}");
            }
        }


        private async Task ExitProgram()
        {
            Console.WriteLine("Выход из программы...");
            await userPresenter.LogoutAsync();
            _isLoggedIn = false;
            _isProgramRunning = false; // Остановка программы
        }

        


        public async Task AuthUser()
        {
            var menuActions = new Dictionary<int, Func<Task>>()
            {
                { 1, async () => await RegisterUser() },
                { 2, async () => await LoginUser() },
                { 3, () => Task.Run(() => System.Environment.Exit(0)) }
            };

            var menuLabels = new Dictionary<int, string>()
            {
                { 1, "Зарегистрироваться" },
                { 2, "Войти" },
                { 3, "Выход" }
            };

            var menuView = new MenuView(menuActions, menuLabels);

            while (!_isLoggedIn)
            {
                Console.WriteLine("Выберите действие:");
                await menuView.ExecuteMenuChoice();
            }
        }

        private async Task RegisterUser()
        {
            CancellationToken token = new CancellationToken();
            while (true)
            {
                Console.WriteLine("Регистрация нового пользователя (нажмите 'Esc' для отмены)...");

                string name = await ReadInputWithEsc("Введите имя: ");
                if (name == null) return;

                string email = await ReadInputWithEsc("Введите электронную почту: ");
                if (email == null) return;

                string password = await ReadInputWithEsc("Введите пароль: ");
                if (password == null) return;

                try
                {
                    await userPresenter.CreateUserAsync(name, email, password, token);
                    Console.WriteLine("Пользователь успешно зарегистрирован.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при регистрации: {ex.Message}. Попробуйте снова.");
                }
            }
        }

        private async Task LoginUser()
        {
            CancellationToken token = new CancellationToken();
            while (true)
            {
                Console.WriteLine("Аутентификация пользователя (нажмите 'Esc' для отмены)...");

                string email = await ReadInputWithEsc("Введите электронную почту: ");
                if (email == null) return;

                string password = await ReadInputWithEsc("Введите пароль: ");
                if (password == null) return;

                try
                {
                    await userPresenter.AuthenticateUserAsync(email, password, token);
                    Console.WriteLine("Пользователь успешно аутентифицирован.");
                    _isLoggedIn = true;
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при аутентификации: {ex.Message}. Попробуйте снова.");
                }
            }
        }

        private async Task<string> ReadInputWithEsc(string prompt)
        {
            Console.Write(prompt);
            StringBuilder input = new StringBuilder();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Tab)
                    {
                        Console.WriteLine("\nВвод отменен, возвращение в меню...");
                        return null;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input.Remove(input.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        input.Append(key.KeyChar);
                        Console.Write(key.KeyChar);
                    }
                }

                await Task.Delay(50);
            }

            return input.ToString();
        }

        
    }
}

