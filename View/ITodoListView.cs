using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Presenter;
using Storage;
using View;

namespace View;
public interface ITodoListView
{
    Task ShowUserTodoListAsync(User user);
    Task AddTodoListAsync(string owner_id);
    Task UpdateTodoList(User user, bool update);
}