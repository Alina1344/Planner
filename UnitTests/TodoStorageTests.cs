using Moq;
using NUnit.Framework;
using Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Tests
{
    [TestFixture]
    public class TodoStorageTests
    {
        private Mock<DatabaseRepository<Todo>> _mockRepository;
        private TodoStorage _todoStorage;

        [SetUp]
        public void SetUp()
        {
            var connectionString = "your_connection_string";  
            var tableName = "Todos";  
        
            // Мокируем зависимость DatabaseRepository с реальными значениями для конструктора
            _mockRepository = new Mock<DatabaseRepository<Todo>>(connectionString, tableName);
        
            // Создайте экземпляр TodoStorage с мокированным репозиторием
            _todoStorage = new TodoStorage(_mockRepository.Object);
        }


       
        [Test]
        public void AddTodoAsync_ShouldThrowArgumentNullException_WhenTodoIsNull()
        {
            // Arrange
            Todo? todo = null;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _todoStorage.AddTodoAsync(todo, cancellationToken));
        }

       
       
        [Test]
        public void LoadTodosByWishlistIdAsync_ShouldThrowArgumentException_WhenInvalidTodoListId()
        {
            // Arrange
            var invalidTodoListId = "InvalidGuid";
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _todoStorage.LoadTodosByWishlistIdAsync(invalidTodoListId, cancellationToken));
        }

       }
}

