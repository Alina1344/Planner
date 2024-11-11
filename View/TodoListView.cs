using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Presenter;

namespace View;

public class TodoListView : ITodoListView
{
    private TodoListPresenter _todoListPresenter;
    private TodoView _todoView;

    public TodoListView(TodoListPresenter todoListPresenter, TodoView todoView)
    {
        _todoListPresenter = todoListPresenter;
        _todoView = todoView;
    }

    public async Task StartTodolist(User user)
    {
        string userId = user.Id;
        bool continueRunning = true;

        while (continueRunning)
        {
            Console.Clear();
            Console.WriteLine($"\nПривет! {user.Name}\n");

            await ShowUserTodoListAsync(user);

            var menuActions = new Dictionary<int, Func<Task>>()
            {
                { 1, async () => await AddTodoListAsync(userId) },  
                { 2, async () => await UpdateTodoList(user, true) },     
                { 3, async () => await DeleteTodoList(user) }, // Удаление списка
                { 4, () => Task.FromResult(continueRunning = false) } // Назад
            };

            var menuLabels = new Dictionary<int, string>()
            {
                { 1, "Создать новый список" },
                { 2, "Посмотреть список" },
                { 3, "Удалить список" },
                { 4, "Назад" }
            };

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

    public async Task ShowUserTodoListAsync(User user)
    {
        try
        {
            // Создание CancellationTokenSource для получения CancellationToken
            using CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken cancellationToken = cts.Token;

            // Вызов метода с двумя параметрами
            IReadOnlyCollection<TodoList> todolists = await _todoListPresenter.LoadUserTodolistAsync(user.Id, cancellationToken);

            if (todolists == null || todolists.Count == 0)
            {
                Console.WriteLine("У вас нет доступных списков задач.");
                return;
            }

            Console.WriteLine("Списки задач:\n");
            int index = 1;
            foreach (var todolist in todolists)
            {
                Console.WriteLine($"{index}. Имя: {todolist.Title}");
                index++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка при загрузке списков задач: {e.Message}");
        }
    }

    public async Task DeleteTodoList(User user)
    {
        try
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken cancellationToken = cts.Token;
            
            // Загружаем списки задач
            IReadOnlyCollection<TodoList> todolists = await _todoListPresenter.LoadUserTodolistAsync(user.Id, cancellationToken);
           
           

            if (todolists == null || todolists.Count == 0)
            {
                Console.WriteLine("Нет списков задач для удаления.");
                return;
            }

            await ShowUserTodoListAsync(user);
            int selectedTodolistIndex;
            do
            {
                Console.Write("\nВведите номер списка, который хотите удалить: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out selectedTodolistIndex) || selectedTodolistIndex < 1 || selectedTodolistIndex > todolists.Count)
                {
                    Console.WriteLine("Неверный ввод. Пожалуйста, введите корректный номер.");
                }
            }
            while (selectedTodolistIndex < 1 || selectedTodolistIndex > todolists.Count);

            var selectedList = todolists.ElementAt(selectedTodolistIndex - 1);
            
            Guid todoListIdString = selectedList.Id; // строка, представляющая Guid
           
            // Создание CancellationTokenSource для получения CancellationToken
            using CancellationTokenSource cts1 = new CancellationTokenSource();
            CancellationToken cancellationToken1 = cts1.Token;

            // Вызов метода с преобразованным Guid
            await _todoListPresenter.DeleteTodolistAsync(todoListIdString, cancellationToken1);
            
            Console.WriteLine("Список задач успешно удалён.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при удалении списка: {e.Message}");
        }
    }
    
 public async Task AddTodoListAsync(string w_ownerId)
     {
         CancellationToken token = new CancellationToken();
         try
         {
             Console.WriteLine("Создание списка задач");
 
             string w_name;
             do
             {
                 Console.Write("Введите название списка: ");
                 w_name = Console.ReadLine();
 
                 if (string.IsNullOrWhiteSpace(w_name))
                 {
                     Console.WriteLine("Название списка не может быть пустым. Пожалуйста, введите корректное название.");
                 }
             }
             while (string.IsNullOrWhiteSpace(w_name));
 
             string w_description;
             do
             {
                 Console.Write("Введите комментарий: ");
                 w_description = Console.ReadLine();
 
                 if (string.IsNullOrWhiteSpace(w_description))
                 {
                     Console.WriteLine("Описание не может быть пустым. Пожалуйста, введите комментарий.");
                 }
             }
             while (string.IsNullOrWhiteSpace(w_description));
 
             await _todoListPresenter.AddNewTodolistAsync(w_name, w_description, w_ownerId,token);
             Console.WriteLine("Список задач успешно создан.");
         }
         catch (Exception e)
         {
             Console.WriteLine($"Произошла ошибка: {e.Message}");
         }
     }
 
     public async Task UpdateTodoList(User user, bool update)
     {
         CancellationToken token = new CancellationToken();
         try
         {
             // Загружаем вишлисты пользователя
             IReadOnlyCollection<TodoList> todolists = await _todoListPresenter.LoadUserTodolistAsync(user.Id,token);
 
             // Проверяем наличие вишлистов
             if (todolists == null || todolists.Count == 0)
             {
                 Console.WriteLine("У вас нет доступных списков задач для изменения.");
                 return;
             }
 
             // Выводим список вишлистов
             await ShowUserTodoListAsync(user);
 
             // Спрашиваем у пользователя, какой вишлист он хочет обновить
             int selectedTodolistIndex;
             do
             {
                 Console.Write("\nВведите номер списка задач, который хотите обновить: ");
                 string input = Console.ReadLine();
 
                 // Проверка ввода номера вишлиста
                 if (!int.TryParse(input, out selectedTodolistIndex) || selectedTodolistIndex < 1 || selectedTodolistIndex > todolists.Count)
                 {
                     Console.WriteLine("Неверный ввод. Пожалуйста, введите корректный номер списка задач.");
                 }
             }
             while (selectedTodolistIndex < 1 || selectedTodolistIndex > todolists.Count);
 
             // Получаем выбранный вишлист по индексу
             var selectedWishlist = todolists.ElementAt(selectedTodolistIndex - 1);
            
                 // Вызываем функцию для работы с подарками в выбранном вишлисте
                 await _todoView.StartTodos(user, selectedWishlist, update);
            
             
         }
         catch (Exception e)
         {
             Console.WriteLine($"Произошла ошибка при обновлении списка задач: {e.Message}");
         }
     }
     


}
    
    
    
    


   