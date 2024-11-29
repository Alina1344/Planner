using System;
using System.Collections.Generic;

namespace Models
{
    public class TodoList
    {
        // Свойства
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string OwnerId { get; init; }
      

        // Основной конструктор
        public TodoList(Guid id, string title, string ownerId)
        {
            Id = id;
            Title = title;
            OwnerId = ownerId;
            
        }
        public TodoList()
        {
        }
    }

}
