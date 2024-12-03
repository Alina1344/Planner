using Microsoft.AspNetCore.Mvc;
using Storage;
using Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserStorage _userStorage;

        public UsersController(IUserStorage userStorage)
        {
            _userStorage = userStorage;
        }

        // Получить всех пользователей
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<User>>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userStorage.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        // Получить пользователя по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id, CancellationToken cancellationToken)
        {
            var user = await _userStorage.GetUserAsync(id, cancellationToken);
            if (user == null)
                return NotFound($"User with ID {id} not found.");
            return Ok(user);
        }

        // Добавить нового пользователя
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user, CancellationToken cancellationToken)
        {
            await _userStorage.AddUserAsync(user, cancellationToken);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        

        // Обновить данные пользователя
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser, CancellationToken cancellationToken)
        {
            await _userStorage.UpdateUserAsync(id, updatedUser, cancellationToken);
            return NoContent();
        }

        // Поиск пользователя по Email
        [HttpGet("search-by-email")]
        public async Task<ActionResult<User>> GetUserByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var user = await _userStorage.GetUserByEmailAsync(email, cancellationToken);
            if (user == null)
                return NotFound($"User with email {email} not found.");
            return Ok(user);
        }
    }
}
