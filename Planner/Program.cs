using System;
using System.Threading.Tasks;
using Models;
using Storage;
using Presenter;
using View;
using System;

namespace Planner;

class Program    {
    public static async Task Main(string[] args)       
    {
        UserView userView = new UserView(new UserPresenter(),new TodoListView(new TodoListPresenter(),new TodoView()),new TodoView());           
        await userView.Start();
    }
}