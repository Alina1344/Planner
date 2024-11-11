using System;
using System.Collections.Generic;

namespace Models
{
    public record TodoList(Guid Id, string Title, string OwnerId, List<Todo> Todos);
 
}
