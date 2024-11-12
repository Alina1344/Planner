using Models;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Tests
{
    [TestFixture]
    public class TodoStorageTests
    {
        private Mock<IFileStorage<Todo>> _mockFileStorage;
        private TodoStorage _todoStorage;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _mockFileStorage = new Mock<IFileStorage<Todo>>();
            _todoStorage = new TodoStorage(_mockFileStorage.Object);
            _cancellationToken = CancellationToken.None;
        }

        
        [Test]
        public async Task CompleteTodoAsync_ShouldMarkTodoAsCompleted_WhenTodoExists()
        {
            // Подготовка данных
            var todoId = Guid.NewGuid();
            var todos = new List<Todo>
            {
                new Todo(
                    todoId, 
                    "Test Todo", 
                    "Test Description", 
                    DateTime.Now.AddDays(1), 
                    new List<string> { "tag1" }, 
                    false, 
                    Guid.NewGuid(), 
                    "ownerId"
                )
            };

            // Настройка моков
            _mockFileStorage.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(todos);

            // Мок для UpdateAsync, который вызывает обновление задачи
            _mockFileStorage.Setup(x => x.UpdateAsync(It.IsAny<Predicate<Todo>>(), It.IsAny<Todo>(), _cancellationToken))
                .Returns(Task.CompletedTask)
                .Callback<Predicate<Todo>, Todo, CancellationToken>((predicate, updatedTodo, token) =>
                {
                    // Обновляем задачу в списке
                    var index = todos.FindIndex(predicate);
                    if (index != -1)
                    {
                        todos[index] = updatedTodo;
                    }

                    // Проверяем, что в обновленной задаче свойство IsCompleted установлено в true
                    Assert.That(updatedTodo.IsCompleted, Is.True);
                });

            // Выполнение действия
            await _todoStorage.CompleteTodoAsync(todoId, _cancellationToken);

            // Проверка результата: задача должна быть помечена как выполненная
            var updatedTodo = todos.First();
            Assert.That(updatedTodo.IsCompleted, Is.True);

            // Проверяем, что метод UpdateAsync был вызван один раз
            _mockFileStorage.Verify(x => x.UpdateAsync(It.IsAny<Predicate<Todo>>(), It.IsAny<Todo>(), _cancellationToken), Times.Once);
        }

    }
}
