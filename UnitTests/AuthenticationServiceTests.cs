/*using Models;
using Moq;
using NUnit.Framework;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter.Tests;

[TestFixture]
public class AuthenticationServiceTests
{
    private Mock<UserStorage> _mockUserStorage;
    private AuthenticationService _authenticationService;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void SetUp()
    {
        // Создаем мок для UserStorage
        _mockUserStorage = new Mock<UserStorage>();
        _mockUserStorage.SetupAllProperties(); // Мокируем все свойства и методы

        // Создаем экземпляр AuthenticationService с мокированным UserStorage
        _authenticationService = new AuthenticationService(_mockUserStorage.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task AuthenticateUserAsync_ShouldAuthenticateUser_WhenValidCredentials()
    {
        // Arrange
        var email = "test@example.com";
        var password = "validPassword";
        var user = new User("1", "Test User", email, "hashedPassword");

        // Настроим мок для метода GetUserByEmailAsync
        _mockUserStorage
            .Setup(x => x.GetUserByEmailAsync(email, _cancellationToken))
            .ReturnsAsync(user);

        // Act
        await _authenticationService.AuthenticateUserAsync(email, password, _cancellationToken);

        // Assert
        Assert.That(await _authenticationService.GetAuthenticatedUserAsync(), Is.EqualTo(user));
    }
}*/
