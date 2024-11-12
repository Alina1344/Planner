using Models;
using NUnit.Framework;
using Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace Storage.Tests
{
    [TestFixture]
    public class TodoListStorageTests
    {
        private Mock<IFileStorage<TodoList>> _fileStorageMock;
        private TodoListStorage _todoListStorage;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _fileStorageMock = new Mock<IFileStorage<TodoList>>();
            _todoListStorage = new TodoListStorage(_fileStorageMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task GetUserTodoListsAsync_ReturnsTodoLists_ForGivenUserId()
        {
            // Arrange
            var userId = "test-user-id";
            var todoList = new TodoList(Guid.NewGuid(), "Test List", userId, new List<Todo>());
            _fileStorageMock.Setup(fs => fs.GetAllAsync(_cancellationToken)).ReturnsAsync(new List<TodoList> { todoList });

            // Act
            var result = await _todoListStorage.GetUserTodoListsAsync(userId, _cancellationToken);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().OwnerId, Is.EqualTo(userId));
        }

        // Дополнительные тесты могут быть написаны по аналогии
    }
}