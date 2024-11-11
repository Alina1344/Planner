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
    Task AddTodo(string userId, string todolistId);
 


}