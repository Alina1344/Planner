using Models;
using Moq;
using NUnit.Framework;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter.Tests
{
    [TestFixture]
    public class TodoPresenterTests
    {
        private Mock<ITodoStorage> _todoStorageMock;
        private TodoPresenter _todoPresenter;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _todoStorageMock = new Mock<ITodoStorage>();
            _todoPresenter = new TodoPresenter(_todoStorageMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task CompleteTodoAsync_ThrowsException_WhenTodoNotFound()
        {
            // Arrange
            var todoId = Guid.NewGuid();

            // Настройка возвращаемого результата GetCompletedTodosAsync
            _todoStorageMock
                .Setup(s => s.GetCompletedTodosAsync(_cancellationToken))
                .ReturnsAsync(new List<Todo>()); // Возвращаем пустой список, как будто задача не найдена

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(() => _todoPresenter.CompleteTodoAsync(todoId, _cancellationToken));
            Assert.That(exception.Message, Is.EqualTo("Задача не найдена или уже выполнена."));
        }

        [Test]
        public async Task AddNewTodoAsync_ThrowsArgumentNullException_WhenParametersAreNullOrEmpty()
        {
            // Arrange
            string title = string.Empty;
            string description = string.Empty;
            string ownerId = string.Empty;
            string todoListId = string.Empty;
            DateTime deadline = DateTime.Now;
            List<string> tags = new List<string>();

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _todoPresenter.AddNewTodoAsync(title, description, ownerId, todoListId, deadline, tags, _cancellationToken));
            Assert.That(exception.Message, Does.Contain("Parameters cannot be null or empty"));
        }
        
        [Test]
        public async Task AddNewTodoAsync_CallsAddTodoAsync_WhenParametersAreValid()
        {
            // Arrange
            var todoListId = Guid.NewGuid().ToString(); // Генерируем корректный GUID
            var title = "New Task";
            var description = "Task description";
            var ownerId = "owner-id";
            var deadline = DateTime.Now.AddDays(1);
            var tags = new List<string> { "tag1", "tag2" };

            _todoStorageMock.Setup(s => s.AddTodoAsync(It.IsAny<Todo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask); // Мокаем вызов

            // Act
            await _todoPresenter.AddNewTodoAsync(title, description, ownerId, todoListId, deadline, tags, _cancellationToken);

            // Assert
            _todoStorageMock.Verify(s => s.AddTodoAsync(It.IsAny<Todo>(), _cancellationToken), Times.Once);
        }

        [Test]
        public async Task DeleteTodoAsync_CallsDeleteTodoAsync_WhenValidTodoId()
        {
            // Arrange
            var todoId = Guid.NewGuid();

            // Act
            await _todoPresenter.DeleteTodoAsync(todoId, _cancellationToken);

            // Assert
            _todoStorageMock.Verify(x => x.DeleteTodoAsync(todoId, _cancellationToken), Times.Once);
        }

        [Test]
        public void ReserveTodoAsync_ThrowsArgumentNullException_WhenOwnerIdIsNullOrEmpty()
        {
            // Arrange
            var todoId = Guid.NewGuid();
            string ownerId = string.Empty;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _todoPresenter.ReserveTodoAsync(todoId, ownerId, _cancellationToken));
            Assert.That(exception.ParamName, Is.EqualTo("ownerId"));
        }

        [Test]
        public async Task GetCompletedTodosAsync_ReturnsCompletedTodos()
        {
            // Arrange
            var completedTodos = new List<Todo>
            {
                new Todo(Guid.NewGuid(), "Completed Todo 1", "Description 1", DateTime.Now, new List<string>(), true, Guid.NewGuid(), "OwnerId")
            };

            _todoStorageMock
                .Setup(s => s.GetCompletedTodosAsync(_cancellationToken))
                .ReturnsAsync(completedTodos);

            // Act
            var result = await _todoPresenter.GetCompletedTodosAsync(_cancellationToken);

            // Assert
            Assert.That(result, Is.EqualTo(completedTodos));
        }

        [Test]
        public async Task SearchTodosByTagAsync_ReturnsMatchingTodos()
        {
            // Arrange
            var tag = "TestTag";
            var matchingTodos = new List<Todo>
            {
                new Todo(Guid.NewGuid(), "Test Todo 1", "Description 1", DateTime.Now, new List<string> { tag }, false, Guid.NewGuid(), "OwnerId"),
                new Todo(Guid.NewGuid(), "Test Todo 2", "Description 2", DateTime.Now, new List<string> { tag }, false, Guid.NewGuid(), "OwnerId")
            };

            _todoStorageMock
                .Setup(s => s.SearchTodosByTagAsync(tag, _cancellationToken))
                .ReturnsAsync(matchingTodos);

            // Act
            var result = await _todoPresenter.SearchTodosByTagAsync(tag, _cancellationToken);

            // Assert
            Assert.That(result, Is.EqualTo(matchingTodos));
        }

        [Test]
        public async Task GetAllTodosSortedByDeadlineAsync_ReturnsTodosSortedByDeadline()
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo(Guid.NewGuid(), "Todo 1", "Description 1", DateTime.Now.AddDays(2), new List<string>(), false, Guid.NewGuid(), "OwnerId"),
                new Todo(Guid.NewGuid(), "Todo 2", "Description 2", DateTime.Now.AddDays(1), new List<string>(), false, Guid.NewGuid(), "OwnerId")
            };

            _todoStorageMock
                .Setup(s => s.GetAllTodosAsync(_cancellationToken))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoPresenter.GetAllTodosSortedByDeadlineAsync(_cancellationToken);

            // Assert
            Assert.That(result.First().Title, Is.EqualTo("Todo 2"));
            Assert.That(result.Last().Title, Is.EqualTo("Todo 1"));
        }
    }
}
