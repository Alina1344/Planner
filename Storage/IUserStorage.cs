using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Storage
{
    public interface IUserStorage
    {
        // Получение всех пользователей
        Task<IReadOnlyCollection<User>> GetAllUsersAsync(CancellationToken cancellationToken);

        // Получение пользователя по ID
        Task<User?> GetUserAsync(string userId, CancellationToken cancellationToken);

        // Поиск пользователей по ключевому слову
        Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword, CancellationToken cancellationToken);

        // Добавление нового пользователя
        Task AddUserAsync(User user, CancellationToken cancellationToken);

        // Удаление пользователя по ID
        Task DeleteUserAsync(string userId, CancellationToken cancellationToken);

        // Обновление данных пользователя
        Task UpdateUserAsync(string userId, User updatedUser, CancellationToken cancellationToken);

        // Получение пользователя по Email
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    }
}