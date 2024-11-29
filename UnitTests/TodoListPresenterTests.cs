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
    public class TodoListPresenterTests
    {
        private Mock<ITodoListStorage> _wishlistRepositoryMock;
        private Mock<IUserPresenter> _userPresenterMock;
        private TodoListPresenter _todoListPresenter;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _wishlistRepositoryMock = new Mock<ITodoListStorage>();
            _userPresenterMock = new Mock<IUserPresenter>();
            _todoListPresenter = new TodoListPresenter(_wishlistRepositoryMock.Object, _userPresenterMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task LoadUserTodolistAsync_ReturnsTodoLists_ForValidUserId()
        {
            // Arrange
            string userId = "user-id";
            var user = new User(userId, "Test User", "test@example.com", "UserRole");
            var todoLists = new List<TodoList>
            {
                new TodoList(Guid.NewGuid(), "TodoList1", userId)
            };

            _userPresenterMock
                .Setup(up => up.LoadUserAsync(userId, _cancellationToken))
                .ReturnsAsync(user);
            
            _wishlistRepositoryMock
                .Setup(repo => repo.GetUserTodoListsAsync(userId, _cancellationToken))
                .ReturnsAsync(todoLists);

            // Act
            var result = await _todoListPresenter.LoadUserTodolistAsync(userId, _cancellationToken);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("TodoList1"));
        }

        [Test]
        public void LoadUserTodolistAsync_ThrowsException_WhenUserIdIsNullOrEmpty()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _todoListPresenter.LoadUserTodolistAsync("", _cancellationToken));
        }

        [Test]
        public async Task AddNewTodolistAsync_CallsRepository_WithValidData()
        {
            // Arrange
            string title = "New TodoList";
            string description = "Description";
            string ownerId = "owner-id";

            // Act
            await _todoListPresenter.AddNewTodolistAsync(title, description, ownerId, _cancellationToken);

            // Assert
            _wishlistRepositoryMock.Verify(repo => repo.AddTodoListAsync(It.Is<TodoList>(tl =>
                tl.Title == title && tl.OwnerId == ownerId), _cancellationToken), Times.Once);
        }

        [Test]
        public void AddNewTodolistAsync_ThrowsException_WhenTitleIsNullOrEmpty()
        {
            // Arrange
            string description = "Description";
            string ownerId = "owner-id";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _todoListPresenter.AddNewTodolistAsync("", description, ownerId, _cancellationToken));
        }

        [Test]
        public async Task DeleteTodolistAsync_CallsRepository_WithValidTodoListId()
        {
            // Arrange
            Guid todoListId = Guid.NewGuid();

            // Act
            await _todoListPresenter.DeleteTodolistAsync(todoListId, _cancellationToken);

            // Assert
            _wishlistRepositoryMock.Verify(repo => repo.DeleteTodoListAsync(todoListId, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task FilterTodosByDeadlineAsync_ReturnsFilteredTodos()
        {
            // Arrange
            DateTime deadline = DateTime.Today;
            var todos = new List<Todo>
            {
                new Todo(Guid.NewGuid(), "Todo1", "Description1", DateTime.Today.AddDays(-1),  false, Guid.NewGuid(), "owner-id"),
                new Todo(Guid.NewGuid(), "Todo2", "Description2", DateTime.Today.AddDays(1),  false, Guid.NewGuid(), "owner-id")
            };

            _wishlistRepositoryMock
                .Setup(repo => repo.GetAllTodosAsync(_cancellationToken))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoListPresenter.FilterTodosByDeadlineAsync(deadline, _cancellationToken);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Todo1"));
        }
    }
}
