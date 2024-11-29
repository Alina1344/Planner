using System;
using System.Collections.Generic;

namespace Models
{
    public record class Todo
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime Deadline { get; init; }
        public bool IsCompleted { get; init; }
        public Guid TodoListId { get; init; }
        public string OwnerId { get; init; }

        // Конструктор по умолчанию (параметры можно будет задать позже)
        public Todo()
        {
        }

        // Основной пользовательский конструктор
        public Todo(Guid id, string title, string description, DateTime deadline, bool isCompleted, Guid todoListId, string ownerId)
        {
            Id = id;
            Title = title;
            Description = description;
            Deadline = deadline;
            IsCompleted = isCompleted;
            TodoListId = todoListId;
            OwnerId = ownerId;
        }
    }
}