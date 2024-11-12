using Models;
using Moq;
using NUnit.Framework;
using Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter.Tests
{
    [TestFixture]
    public class UserPresenterTests
    {
        private Mock<IUserStorage> _userRepositoryMock;
        private Mock<IAuthenticationService> _authServiceMock;
        private UserPresenter _userPresenter;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserStorage>();
            _authServiceMock = new Mock<IAuthenticationService>();
            _userPresenter = new UserPresenter(_userRepositoryMock.Object, _authServiceMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task CreateUserAsync_ThrowsException_WhenParametersAreNullOrEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _userPresenter.CreateUserAsync("", "email@test.com", "password", _cancellationToken));
            Assert.ThrowsAsync<ArgumentNullException>(() => _userPresenter.CreateUserAsync("name", "", "password", _cancellationToken));
            Assert.ThrowsAsync<ArgumentNullException>(() => _userPresenter.CreateUserAsync("name", "email@test.com", "", _cancellationToken));
        }

        [Test]
        public async Task CreateUserAsync_CallsAuthServiceRegisterUserAsync_WithValidParameters()
        {
            string name = "Test User";
            string email = "test@example.com";
            string password = "password";

            await _userPresenter.CreateUserAsync(name, email, password, _cancellationToken);

            _authServiceMock.Verify(auth => auth.RegisterUserAsync(name, email, password, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task AuthenticateUserAsync_CallsAuthServiceAuthenticateUserAsync_AndReturnsAuthenticatedUser()
        {
            string email = "test@example.com";
            string password = "password";
            var user = new User("user-id", "Test User", email, "UserRole");

            _authServiceMock.Setup(auth => auth.AuthenticateUserAsync(email, password, _cancellationToken)).Returns(Task.CompletedTask);
            _authServiceMock.Setup(auth => auth.GetAuthenticatedUserAsync()).ReturnsAsync(user);

            var result = await _userPresenter.AuthenticateUserAsync(email, password, _cancellationToken);

            Assert.That(result, Is.EqualTo(user));
            _authServiceMock.Verify(auth => auth.AuthenticateUserAsync(email, password, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task LoadUserAsync_ReturnsUser_WhenUserExists()
        {
            string userId = "user-id";
            var user = new User(userId, "Test User", "test@example.com", "UserRole");

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(userId, _cancellationToken)).ReturnsAsync(user);

            var result = await _userPresenter.LoadUserAsync(userId, _cancellationToken);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void LoadUserAsync_ThrowsException_WhenUserDoesNotExist()
        {
            string userId = "invalid-user-id";

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(userId, _cancellationToken)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<Exception>(() => _userPresenter.LoadUserAsync(userId, _cancellationToken));
        }

        [Test]
        public async Task DeleteUserAsync_CallsRepositoryDeleteUserAsync_WhenUserExists()
        {
            string userId = "user-id";
            var user = new User(userId, "Test User", "test@example.com", "UserRole");

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(userId, _cancellationToken)).ReturnsAsync(user);

            await _userPresenter.DeleteUserAsync(userId, _cancellationToken);

            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(userId, _cancellationToken), Times.Once);
        }

        [Test]
        public void DeleteUserAsync_ThrowsException_WhenUserDoesNotExist()
        {
            string userId = "invalid-user-id";

            _userRepositoryMock.Setup(repo => repo.GetUserAsync(userId, _cancellationToken)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<Exception>(() => _userPresenter.DeleteUserAsync(userId, _cancellationToken));
        }

        [Test]
        public async Task SearchUsersByKeywordAsync_ReturnsEmptyList_WhenKeywordIsNullOrEmpty()
        {
            var result = await _userPresenter.SearchUsersByKeywordAsync("", _cancellationToken);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task SearchUsersByKeywordAsync_CallsRepositorySearchUsersByKeywordAsync_WithValidKeyword()
        {
            string keyword = "Test";
            var users = new List<User>
            {
                new User("user-id", "Test User", "test@example.com", "UserRole")
            };

            _userRepositoryMock.Setup(repo => repo.SearchUsersByKeywordAsync(keyword, _cancellationToken)).ReturnsAsync(users);

            var result = await _userPresenter.SearchUsersByKeywordAsync(keyword, _cancellationToken);

            Assert.That(result, Is.EqualTo(users));
        }

        [Test]
        public async Task GetUserByEmailAsync_ReturnsUser_WhenEmailIsValid()
        {
            string email = "test@example.com";
            var user = new User("user-id", "Test User", email, "UserRole");

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email, _cancellationToken)).ReturnsAsync(user);

            var result = await _userPresenter.GetUserByEmailAsync(email, _cancellationToken);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public async Task GetUserByEmailAsync_ReturnsNull_WhenEmailIsEmpty()
        {
            var result = await _userPresenter.GetUserByEmailAsync("", _cancellationToken);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task LogoutAsync_CallsAuthServiceLogoutAsync()
        {
            await _userPresenter.LogoutAsync();

            _authServiceMock.Verify(auth => auth.LogoutAsync(), Times.Once);
        }
    }
}
