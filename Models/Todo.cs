using System;
using System.Collections.Generic;

namespace Models
{
    public record Todo(Guid Id, string Title, string Description, DateTime Deadline, List<string> Tags, bool IsCompleted, Guid TodoListId, string OwnerId);
}