using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Storage;
using Models;

namespace Storage.Tests
{
    /*[TestFixture]
    public class TodoListStorageTests
    {
        private Mock<DatabaseRepository<TodoList>> _mockTodoListRepository;
        private Mock<DatabaseRepository<Todo>> _mockTodoRepository;
        private TodoListStorage _todoListStorage;

        [SetUp]
        public void SetUp()
        {
            _mockTodoListRepository = new Mock<DatabaseRepository<TodoList>>(MockBehavior.Strict, "connectionString", "tableName");
            _mockTodoRepository = new Mock<DatabaseRepository<Todo>>(MockBehavior.Strict, "connectionString", "tableName");
            _todoListStorage = new TodoListStorage(_mockTodoListRepository.Object, _mockTodoRepository.Object);
        }

        [Test]
        public async Task GetAllTodoListsAsync_ReturnsListOfTodoLists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var todoLists = new List<TodoList>
            {
                new TodoList(Guid.NewGuid(), "Test List 1", "user1"),
                new TodoList(Guid.NewGuid(), "Test List 2", "user2")
            };

            _mockTodoListRepository
                .Setup(repo => repo.GetListAsync(null, null, cancellationToken))
                .ReturnsAsync(todoLists);

            // Act
            var result = await _todoListStorage.GetAllTodoListsAsync(cancellationToken);

            // Assert
            Assert.That(result, Is.EqualTo(todoLists));
            _mockTodoListRepository.Verify(repo => repo.GetListAsync(null, null, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetUserTodoListsAsync_ValidUserId_ReturnsUserTodoLists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "user1";
            var todoLists = new List<TodoList>
            {
                new TodoList(Guid.NewGuid(), "User List 1", userId)
            };

            _mockTodoListRepository
                .Setup(repo => repo.GetListAsync("ownerid = @OwnerId", It.IsAny<object>(), cancellationToken))
                .ReturnsAsync(todoLists);

            // Act
            var result = await _todoListStorage.GetUserTodoListsAsync(userId, cancellationToken);

            // Assert
            Assert.That(result, Is.EqualTo(todoLists));
            _mockTodoListRepository.Verify(repo => repo.GetListAsync("ownerid = @OwnerId", It.IsAny<object>(), cancellationToken), Times.Once);
        }*/
    
    [TestFixture]
    public class TodoListStorageTests
    {
        private Mock<IDatabaseRepository<TodoList>> _mockTodoListRepository;
        private Mock<IDatabaseRepository<Todo>> _mockTodoRepository;
        private TodoListStorage _todoListStorage;
        
        [SetUp]
        public void SetUp()
        {
            // Мокируем интерфейсы
            _mockTodoListRepository = new Mock<IDatabaseRepository<TodoList>>(MockBehavior.Strict);
            _mockTodoRepository = new Mock<IDatabaseRepository<Todo>>(MockBehavior.Strict);
            
            // Передаем моки в конструктор
            _todoListStorage = new TodoListStorage(_mockTodoListRepository.Object, _mockTodoRepository.Object);
        }
    
        [Test]
        public async Task GetAllTodoListsAsync_ReturnsListOfTodoLists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var todoLists = new List<TodoList>
            {
                new TodoList(Guid.NewGuid(), "Test List 1", "user1"),
                new TodoList(Guid.NewGuid(), "Test List 2", "user2")
            };
    
            _mockTodoListRepository
                .Setup(repo => repo.GetListAsync(null, null, cancellationToken))
                .ReturnsAsync(todoLists);
    
            // Act
            var result = await _todoListStorage.GetAllTodoListsAsync(cancellationToken);
    
            // Assert
            Assert.That(result, Is.EqualTo(todoLists));
            _mockTodoListRepository.Verify(repo => repo.GetListAsync(null, null, cancellationToken), Times.Once);
        }
    
        [Test]
        public async Task GetUserTodoListsAsync_ValidUserId_ReturnsUserTodoLists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "user1";
            var todoLists = new List<TodoList>
            {
                new TodoList(Guid.NewGuid(), "User List 1", userId)
            };
    
            _mockTodoListRepository
                .Setup(repo => repo.GetListAsync("ownerid = @OwnerId", It.IsAny<object>(), cancellationToken))
                .ReturnsAsync(todoLists);
    
            // Act
            var result = await _todoListStorage.GetUserTodoListsAsync(userId, cancellationToken);
    
            // Assert
            Assert.That(result, Is.EqualTo(todoLists));
            _mockTodoListRepository.Verify(repo => repo.GetListAsync("ownerid = @OwnerId", It.IsAny<object>(), cancellationToken), Times.Once);
        }
    


        [Test]
        public void GetUserTodoListsAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _todoListStorage.GetUserTodoListsAsync("", cancellationToken));
            Assert.That(ex.Message, Does.Contain("UserId не может быть пустым."));
        }

        [Test]
        public async Task AddTodoListAsync_ValidTodoList_AddsTodoList()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var todoList = new TodoList(Guid.NewGuid(), "Test List", "user1");

            _mockTodoListRepository
                .Setup(repo => repo.AddAsync(todoList, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _todoListStorage.AddTodoListAsync(todoList, cancellationToken);

            // Assert
            _mockTodoListRepository.Verify(repo => repo.AddAsync(todoList, cancellationToken), Times.Once);
        }

        [Test]
        public void AddTodoListAsync_NullTodoList_ThrowsArgumentNullException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() =>
                _todoListStorage.AddTodoListAsync(null, cancellationToken));
            Assert.That(ex.ParamName, Is.EqualTo("todoList"));
        }

        [Test]
        public async Task DeleteTodoListAsync_ValidId_DeletesTodoListAndTasks()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var todoListId = Guid.NewGuid();

            _mockTodoRepository
                .Setup(repo => repo.DeleteAsync("todolist_id = @TodoListId", It.IsAny<object>(), cancellationToken))
                .Returns(Task.CompletedTask);

            _mockTodoListRepository
                .Setup(repo => repo.DeleteAsync("id = @Id", It.IsAny<object>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _todoListStorage.DeleteTodoListAsync(todoListId, cancellationToken);

            // Assert
            _mockTodoRepository.Verify(repo => repo.DeleteAsync("todolist_id = @TodoListId", It.IsAny<object>(), cancellationToken), Times.Once);
            _mockTodoListRepository.Verify(repo => repo.DeleteAsync("id = @Id", It.IsAny<object>(), cancellationToken), Times.Once);
        }
    }
}
