using Models;
using Moq;
using NUnit.Framework;
using Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
    
namespace Storage.Tests
{   
    [TestFixture]
    public class UserStorageTests
    {
        private Mock<IDatabaseRepository<User>> _mockRepository;
        private UserStorage _userStorage;
        private CancellationToken _cancellationToken;
        
        [SetUp]
        public void SetUp()
        {
            // Создаем мок для IDatabaseRepository<User>
            _mockRepository = new Mock<IDatabaseRepository<User>>();
        
            // Передаем мок в UserStorage
            _userStorage = new UserStorage(_mockRepository.Object);
        
            _cancellationToken = CancellationToken.None;
        
            // Проверяем, что _userStorage не равен null
            Assert.That(_userStorage, Is.Not.Null);
        }
    
        [Test]
        public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User("1", "User1", "user1@example.com", "User"),
                new User("2", "User2", "user2@example.com", "User")
            };
    
            _mockRepository.Setup(r => r.GetListAsync(It.IsAny<string>(), It.IsAny<object>(), _cancellationToken))
                           .ReturnsAsync(mockUsers);
    
            // Act
            var result = await _userStorage.GetAllUsersAsync(_cancellationToken);
    
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("User1"));
            Assert.That(result.Last().Name, Is.EqualTo("User2"));
        }
    
        
        [Test]
        public async Task AddUserAsync_ShouldAddUser_WhenCalled()
        {
            // Arrange
            var user = new User("3", "User3", "user3@example.com", "User");
        
            // Настройка мока
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), _cancellationToken))
                           .Returns(Task.CompletedTask);
        
            // Act
            await _userStorage.AddUserAsync(user, _cancellationToken);
        
            // Assert
            Assert.That(_mockRepository.Object, Is.Not.Null);  // Проверка, что объект не равен null
            _mockRepository.Verify(r => r.AddAsync(user, _cancellationToken), Times.Once);
        }
    
    
    
    
    
        [Test]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenCalled()
        {
            // Arrange
            var userId = "1";
    
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<object>(), _cancellationToken))
                           .Returns(Task.CompletedTask);
    
            // Act
            await _userStorage.DeleteUserAsync(userId, _cancellationToken);
    
            // Assert
            _mockRepository.Verify(r => r.DeleteAsync("id = @Id", It.IsAny<object>(), _cancellationToken), Times.Once);
        }
    
        [Test]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenCalled()
        {
            // Arrange
            var userId = "1";
            var updatedUser = new User("1", "Updated User", "updated@example.com", "User");
    
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<User>(), _cancellationToken))
                           .Returns(Task.CompletedTask);
    
            // Act
            await _userStorage.UpdateUserAsync(userId, updatedUser, _cancellationToken);
    
            // Assert
            _mockRepository.Verify(r => r.UpdateAsync("id = @Id", It.IsAny<object>(), updatedUser, _cancellationToken), Times.Once);
        }
    
        [Test]
        public async Task GetUserAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var mockUser = new User("1", "User1", "user1@example.com", "User");
    
            _mockRepository.Setup(r => r.GetSingleAsync("id = @Id", It.IsAny<object>(), _cancellationToken))
                           .ReturnsAsync(mockUser);
    
            // Act
            var result = await _userStorage.GetUserAsync(userId, _cancellationToken);
    
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo("1"));
            Assert.That(result?.Name, Is.EqualTo("User1"));
        }
    
        [Test]
        public async Task SearchUsersByKeywordAsync_ShouldReturnFilteredUsers_WhenKeywordMatches()
        {
            // Arrange
            var keyword = "user1";
            var mockUsers = new List<User>
            {
                new User("1", "User1", "user1@example.com", "User"),
                new User("2", "User2", "user2@example.com", "User")
            };
        
            _mockRepository.Setup(r => r.GetListAsync(It.IsAny<string>(), It.IsAny<object>(), _cancellationToken))
                           .ReturnsAsync(mockUsers);
        
            // Act
            var result = await _userStorage.SearchUsersByKeywordAsync(keyword, _cancellationToken);
        
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.ElementAt(0).Name, Is.EqualTo("User1"));  // Используем ElementAt() для индексации
        }
    
    
        [Test]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExistsByEmail()
        {
            // Arrange
            var email = "user1@example.com";
            var mockUser = new User("1", "User1", email, "User");
    
            _mockRepository.Setup(r => r.GetSingleAsync("email = @Email", It.IsAny<object>(), _cancellationToken))
                           .ReturnsAsync(mockUser);
    
            // Act
            var result = await _userStorage.GetUserByEmailAsync(email, _cancellationToken);
    
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Email, Is.EqualTo(email));
        }
    }
}