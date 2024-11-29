using Microsoft.AspNetCore.Mvc;
using Models;
using Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListStorage _todoListStorage;

    public TodoListController(ITodoListStorage todoListStorage)
    {
        _todoListStorage = todoListStorage ?? throw new ArgumentNullException(nameof(todoListStorage));
    }

    // Получение всех списков задач
    [HttpGet]
    public async Task<IActionResult> GetAllTodoLists(CancellationToken cancellationToken)
    {
        var todoLists = await _todoListStorage.GetAllTodoListsAsync(cancellationToken);
        return Ok(todoLists); // Возвращает HTTP 200 с телом ответа
    }

    // Получение списков задач конкретного пользователя
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserTodoLists(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("UserId не может быть пустым."); // HTTP 400

        var todoLists = await _todoListStorage.GetUserTodoListsAsync(userId, cancellationToken);
        return Ok(todoLists);
    }

    // Получение конкретного списка задач по ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTodoListById(Guid id, CancellationToken cancellationToken)
    {
        var todoList = await _todoListStorage.GetTodoListByIdAsync(id, cancellationToken);
        if (todoList == null)
            return NotFound(); // HTTP 404

        return Ok(todoList);
    }

    // Добавление нового списка задач
    [HttpPost]
    public async Task<IActionResult> AddTodoList([FromBody] TodoList newTodoList, CancellationToken cancellationToken)
    {
        if (newTodoList == null)
            return BadRequest("Список задач не может быть пустым."); // HTTP 400

        await _todoListStorage.AddTodoListAsync(newTodoList, cancellationToken);
        return CreatedAtAction(nameof(GetTodoListById), new { id = newTodoList.Id }, newTodoList); // HTTP 201
    }

    // Обновление списка задач
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTodoList(Guid id, [FromBody] TodoList updatedTodoList, CancellationToken cancellationToken)
    {
        if (updatedTodoList == null)
            return BadRequest("Обновленный список задач не может быть пустым."); // HTTP 400

        await _todoListStorage.UpdateTodoListAsync(id, updatedTodoList, cancellationToken);
        return NoContent(); // HTTP 204
    }

    // Удаление списка задач
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTodoList(Guid id, CancellationToken cancellationToken)
    {
        await _todoListStorage.DeleteTodoListAsync(id, cancellationToken);
        return NoContent(); // HTTP 204
    }

    // Получение всех задач по ID списка задач
    [HttpGet("{id:guid}/todos")]
    public async Task<IActionResult> GetTodosByTodoListId(Guid id, CancellationToken cancellationToken)
    {
        var todos = await _todoListStorage.GetTodosByTodoListIdAsync(id, cancellationToken);
        return Ok(todos);
    }

    // Получение задач по тегу
    [HttpGet("todos/tag")]
    public async Task<IActionResult> GetTodosByTag([FromQuery] string tag, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return BadRequest("Tag не может быть пустым."); // HTTP 400

        var todos = await _todoListStorage.GetTodosByTagAsync(tag, cancellationToken);
        return Ok(todos);
    }

    // Получение задач с крайним сроком
    [HttpGet("todos/deadline")]
    public async Task<IActionResult> GetTodosByDeadline([FromQuery] DateTime deadline, CancellationToken cancellationToken)
    {
        var todos = await _todoListStorage.GetTodosByDeadlineAsync(deadline, cancellationToken);
        return Ok(todos);
    }
}
