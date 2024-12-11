using Microsoft.AspNetCore.Mvc;
using Storage;
using Models;
using Microsoft.AspNetCore.Authorization;  // Не забудьте добавить

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoStorage _todoStorage;

        public TodoController(ITodoStorage todoStorage)
        {
            _todoStorage = todoStorage;
        }

        // Получить все задачи
        [HttpGet]
        [Authorize]  // Требуется авторизация
        public async Task<ActionResult<IReadOnlyCollection<Todo>>> GetAllTodos(CancellationToken cancellationToken)
        {
            var todos = await _todoStorage.GetAllTodosAsync(cancellationToken);
            return Ok(todos);
        }

        // Добавить задачу
        [HttpPost]
        [Authorize]  // Требуется авторизация
        public async Task<IActionResult> AddTodo([FromBody] Todo todo, CancellationToken cancellationToken)
        {
            await _todoStorage.AddTodoAsync(todo, cancellationToken);
            return CreatedAtAction(nameof(GetAllTodos), new { id = todo.Id }, todo);
        }

        // Отметить задачу выполненной
        [HttpPost("{id}/complete")]
        [Authorize]  // Требуется авторизация
        public async Task<IActionResult> CompleteTodo(Guid id, CancellationToken cancellationToken)
        {
            await _todoStorage.CompleteTodoAsync(id, cancellationToken);
            return NoContent();
        }

        // Удаление задачи
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Требуется роль администратора
        public async Task<IActionResult> DeleteTodo(Guid id, CancellationToken cancellationToken)
        {
            var todo = await _todoStorage.GetTodoAsync(id.ToString(), cancellationToken);

            if (todo == null)
            {
                return NotFound(new { Message = "Todo not found or already deleted." });
            }
            await _todoStorage.DeleteTodoAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
