using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Storage
{
    public class UserStorage : IUserStorage
    {
        private readonly IFileStorage<User> _userRepository;
        private readonly IFileStorage<Todo> _taskRepository; // Зависимость для задач

        public UserStorage()
        {
            _userRepository = new FileStorage<User>("../../data/Users.json", "users");
            _taskRepository = new FileStorage<Todo>("../../data/Todos.json", "todos"); // Путь для задач
        }

        // Дополнительный конструктор для тестов (или для гибкой зависимости)
        public UserStorage(IFileStorage<User> userRepository, IFileStorage<Todo> taskRepository)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var users = await _userRepository.GetAllAsync(token);
            return users.FirstOrDefault(u => u.Id == userId);
        }

        public async Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var users = await _userRepository.GetAllAsync(token);
            var filteredUsers = users.Where(u => 
                u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return filteredUsers;
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var users = await _userRepository.GetAllAsync(token);
            return users.FirstOrDefault(u => u.Email == email);
        }

        public async Task AddUserAsync(User user, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _userRepository.AddAsync(user, token);
        }

        public async Task DeleteUserAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _userRepository.DeleteAsync(u => u.Id == userId, token);
        }

        public async Task UpdateUserAsync(User updatedUser, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _userRepository.UpdateAsync(u => u.Id == updatedUser.Id, updatedUser, token);
        }

        // Метод для получения всех задач пользователя
        public async Task<IReadOnlyCollection<Todo>> GetTasksForUserAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var allTasks = await _taskRepository.GetAllAsync(token);
            return allTasks.Where(task => task.OwnerId == userId).ToList().AsReadOnly();
        }
    }
}
