using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Storage
{
    
    public class UserStorage : IUserStorage
    {
        private readonly IDatabaseRepository<User> _databaseRepository;
        
            // Основной конструктор для приложения
            public UserStorage()
            {
                _databaseRepository = new DatabaseRepository<User>(
                    "Host=localhost;Port=1111;Username=postgres;Password=alina13122003;Database=postgres",
                    "users"
                );
            }
        
            // Конструктор для работы с кастомной строкой подключения (например, для тестов)
            public UserStorage(string connectionString)
            {
                _databaseRepository = new DatabaseRepository<User>(connectionString, "users");
            }
        
            // Новый конструктор для тестов (принимает реализацию IDatabaseRepository<User>)
            public UserStorage(IDatabaseRepository<User> databaseRepository)
            {
                _databaseRepository = databaseRepository;
            }
        

        public async Task<IReadOnlyCollection<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _databaseRepository.GetListAsync("1=1", null, cancellationToken);
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken)
        {
            await _databaseRepository.AddAsync(user, cancellationToken);
        }

        public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken)
        {
            await _databaseRepository.DeleteAsync("id = @Id", new { Id = userId }, cancellationToken);
        }

        public async Task UpdateUserAsync(string userId, User updatedUser, CancellationToken cancellationToken)
        {
            await _databaseRepository.UpdateAsync("id = @Id", new { Id = userId }, updatedUser, cancellationToken);
        }

        /// <summary>
        /// Получить пользователя по ID.
        /// </summary>
        public async Task<User?> GetUserAsync(string userId, CancellationToken cancellationToken)
        {
            return await _databaseRepository.GetSingleAsync("id = @Id", new { Id = userId }, cancellationToken);
        }

        /// <summary>
        /// Поиск пользователей по ключевому слову.
        /// </summary>
        public async Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword, CancellationToken cancellationToken)
        {
            var users = await _databaseRepository.GetListAsync("1=1", null, cancellationToken);

            var filteredUsers = users.Where(u =>
                u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            return filteredUsers;
        }

        /// <summary>
        /// Получить пользователя по Email.
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _databaseRepository.GetSingleAsync("email = @Email", new { Email = email }, cancellationToken);
        }
    }
}

