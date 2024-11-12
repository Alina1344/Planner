using Moq; 
using NUnit.Framework;
using Storage;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Tests
{
    [TestFixture]
    public class UserStorageTests
    {
        private Mock<IFileStorage<User>> _userRepositoryMock;
        private Mock<IFileStorage<Todo>> _taskRepositoryMock;
        private UserStorage _userStorage;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IFileStorage<User>>();
            _taskRepositoryMock = new Mock<IFileStorage<Todo>>();
            _userStorage = new UserStorage(_userRepositoryMock.Object, _taskRepositoryMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task GetUserAsync_ReturnsCorrectUser_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var expectedUser = new User("1", "Test User", "test@example.com", "hashedpassword");

            _userRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User> { expectedUser });

            // Act
            var actualUser = await _userStorage.GetUserAsync(userId, _cancellationToken);

            // Assert
            Assert.That(actualUser, Is.EqualTo(expectedUser));
        }

        [Test]
        public async Task SearchUsersByKeywordAsync_FiltersCorrectly()
        {
            // Arrange
            var keyword = "Test User";
            var users = new List<User>
            {
                new User("1", "Test User", "test@example.com", "hashedpassword"),
                new User("2", "Another User", "another@example.com", "hashedpassword")
            };

            _userRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _userStorage.SearchUsersByKeywordAsync(keyword, _cancellationToken);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Test User"));
        }

        [Test]
        public async Task AddUserAsync_CallsRepositoryAddAsync()
        {
            // Arrange
            var newUser = new User("123", "New User", "newuser@example.com", "hashedpassword");

            // Act
            await _userStorage.AddUserAsync(newUser, _cancellationToken);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(newUser, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_CallsRepositoryDeleteAsync()
        {
            // Arrange
            var userId = "123";

            // Act
            await _userStorage.DeleteUserAsync(userId, _cancellationToken);

            // Assert
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Func<User, bool>>(), _cancellationToken), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var updatedUser = new User("123", "Updated User", "updateduser@example.com", "newhashedpassword");
        
            // Настройка мока с конкретным предикатом
            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.Is<Predicate<User>>(predicate => predicate(updatedUser)), updatedUser, _cancellationToken))
                .Returns(Task.CompletedTask);
        
            // Act
            await _userStorage.UpdateUserAsync(updatedUser, _cancellationToken);
        
            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Predicate<User>>(), updatedUser, _cancellationToken), Times.Once);
        }


        [Test]
       
        public async Task GetTasksForUserAsync_ReturnsCorrectTasks_WhenUserHasTasks()
        {
            // Arrange
            var userId = "1";
            var expectedTask = new Todo(
                Guid.NewGuid(),                // Id
                "Test Task",                    // Title
                "Task Description",             // Description
                DateTime.Now.AddDays(1),        // Deadline
                new List<string> { "tag1" },    // Tags
                false,                          // IsCompleted
                Guid.NewGuid(),                 // TodoListId
                userId                          // OwnerId
            );
        
            _taskRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Todo> { expectedTask });
        
            // Act
            var tasks = await _userStorage.GetTasksForUserAsync(userId, _cancellationToken);
        
            // Assert
            Assert.That(tasks.Count, Is.EqualTo(1));
            Assert.That(tasks.First(), Is.EqualTo(expectedTask));
        }

    }
}
