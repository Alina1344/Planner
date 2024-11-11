using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace View;

public interface ITodoView
{
    Task StartTodos(User user, TodoList todolist, bool update); 
    Task ShowUserTodos(string wishlistId); 
    Task AddTodo(string userId, string wishlistId); 
    Task DeleteTodo(string wishlistId); 
    Task MarkTodoAsCompleted(string wishlistId); 
    Task ShowSortedTodosByDeadlineAsync(CancellationToken token, User user); 
    Task ShowSearchedTodoAsync(CancellationToken token, User user); 
    Task ShowCompletTodo(CancellationToken token, User user); 

}