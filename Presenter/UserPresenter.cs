using Models;
using Presenter;
using Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter
{
    public class UserPresenter : IUserPresenter
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserStorage _userRepository;

        public UserPresenter(IUserStorage userRepositoryMock, IAuthenticationService authServiceMock)
        {
            _userRepository = userRepositoryMock;
            _authService = authServiceMock;
        }

        public UserPresenter()
        {
            _userRepository = new UserStorage();
            _authService = new AuthenticationService( new UserStorage());
        }

        public async Task CreateUserAsync(string name, string email, string password, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Parameters cannot be null or empty.");
            }

            await _authService.RegisterUserAsync(name, email, password, token);
        }
        

        public async Task<User> AuthenticateUserAsync(string email, string password, CancellationToken token)
        {
            await _authService.AuthenticateUserAsync(email, password, token);
            return await _authService.GetAuthenticatedUserAsync();
        }

        public async Task<User> LoadUserAsync(string userId, CancellationToken token)
        {
            var user = await _userRepository.GetUserAsync(userId, token);
            return user ?? throw new Exception("User not found.");
        }

        public async Task DeleteUserAsync(string userId, CancellationToken token)
        {
            var user = await _userRepository.GetUserAsync(userId, token);
            if (user != null)
            {
                await _userRepository.DeleteUserAsync(userId, token);
            }
            else
            {
                throw new Exception("User not found.");
            }
        }

        public async Task<User> GetAuthenticatedUserAsync(CancellationToken token)
        {
            return await _authService.GetAuthenticatedUserAsync();
        }

        public async Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Array.Empty<User>(); // Возвращаем пустой массив, если ключевое слово пустое
            }

            return await _userRepository.SearchUsersByKeywordAsync(keyword, token);
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null; // Возвращаем null, если email пустой
            }
            return await _userRepository.GetUserByEmailAsync(email, token);
        }

        public async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
        }
    }
}
