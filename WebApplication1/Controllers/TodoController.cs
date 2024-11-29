using Microsoft.AspNetCore.Mvc;
using Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoStorage _todoStorage;

        public TodosController(ITodoStorage todoStorage)
        {
            _todoStorage = todoStorage;
        }

        // Получить все задачи
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<Todo>>> GetAllTodos(CancellationToken cancellationToken)
        {
            var todos = await _todoStorage.GetAllTodosAsync(cancellationToken);
            return Ok(todos);
        }

        // Добавить задачу
        [HttpPost]
        public async Task<IActionResult> AddTodo([FromBody] Todo todo, CancellationToken cancellationToken)
        {
            await _todoStorage.AddTodoAsync(todo, cancellationToken);
            return CreatedAtAction(nameof(GetAllTodos), new { id = todo.Id }, todo);
        }

        // Отметить задачу выполненной
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTodo(Guid id, CancellationToken cancellationToken)
        {
            await _todoStorage.CompleteTodoAsync(id, cancellationToken);
            return NoContent();
        }

        // Поиск задач по тегу
        [HttpGet("search-by-tag")]
        public async Task<ActionResult<IReadOnlyCollection<Todo>>> SearchTodosByTag([FromQuery] string tag, CancellationToken cancellationToken)
        {
            var todos = await _todoStorage.SearchTodosByTagAsync(tag, cancellationToken);
            return Ok(todos);
        }

        // Удаление задачи
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id, CancellationToken cancellationToken)
        {
            await _todoStorage.DeleteTodoAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
