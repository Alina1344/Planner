using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Storage;
using Models;

namespace Tests
{
    [TestFixture]
    public class UserStorageTests
    {
        private Mock<IFileStorage<User>> _mockUserRepository;
        private Mock<IFileStorage<Todo>> _mockTaskRepository;
        private UserStorage _userStorage;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IFileStorage<User>>();
            _mockTaskRepository = new Mock<IFileStorage<Todo>>();
            _userStorage = new UserStorage(_mockUserRepository.Object, _mockTaskRepository.Object);
        }

        [Test]
        public async Task GetUserAsync_ReturnsCorrectUser()
        {
            var userId = "1";
            var expectedUser = new User { Id = userId, Name = "John Doe" };
            var users = new List<User> { expectedUser };

            _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(users);

            var result = await _userStorage.GetUserAsync(userId, CancellationToken.None);

            Assert.AreEqual(expectedUser, result);
        }

        [Test]
        public async Task SearchUsersByKeywordAsync_ReturnsFilteredUsers()
        {
            var keyword = "john";
            var users = new List<User>
            {
                new User { Id = "1", Name = "John Doe", Email = "john@example.com" },
                new User { Id = "2", Name = "Jane Smith", Email = "jane@example.com" }
            };

            _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(users);

            var result = await _userStorage.SearchUsersByKeywordAsync(keyword, CancellationToken.None);
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John Doe", result.First().Name);
        }

        [Test]
        public async Task GetUserByEmailAsync_ReturnsCorrectUser()
        {
            var email = "john@example.com";
            var expectedUser = new User { Id = "1", Name = "John Doe", Email = email };
            var users = new List<User> { expectedUser };

            _mockUserRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(users);

            var result = await _userStorage.GetUserByEmailAsync(email, CancellationToken.None);

            Assert.AreEqual(expectedUser, result);
        }

        [Test]
        public async Task AddUserAsync_AddsUserSuccessfully()
        {
            var newUser = new User { Id = "3", Name = "New User" };

            await _userStorage.AddUserAsync(newUser, CancellationToken.None);

            _mockUserRepository.Verify(repo => repo.AddAsync(newUser, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_DeletesUserSuccessfully()
        {
            var userId = "1";

            await _userStorage.DeleteUserAsync(userId, CancellationToken.None);

            _mockUserRepository.Verify(repo => repo.DeleteAsync(It.IsAny<System.Func<User, bool>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_UpdatesUserSuccessfully()
        {
            var updatedUser = new User { Id = "1", Name = "Updated User" };

            await _userStorage.UpdateUserAsync(updatedUser, CancellationToken.None);

            _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<System.Func<User, bool>>(), updatedUser, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetTasksForUserAsync_ReturnsUserTasks()
        {
            var userId = "1";
            var tasks = new List<Todo>
            {
                new Todo { Id = "101", OwnerId = userId, Title = "Task 1" },
                new Todo { Id = "102", OwnerId = "2", Title = "Task 2" } // Задача другого пользователя
            };

            _mockTaskRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(tasks);

            var result = await _userStorage.GetTasksForUserAsync(userId, CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Task 1", result.First().Title);
        }
    }
}
