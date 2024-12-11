using Microsoft.AspNetCore.Mvc;
using Models;
using Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;  // Не забудьте добавить

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListStorage _todoListStorage;

    public TodoListController(ITodoListStorage todoListStorage)
    {
        _todoListStorage = todoListStorage ?? throw new ArgumentNullException(nameof(todoListStorage));
    }
    

    // Получение списков задач конкретного пользователя
    [HttpGet("user/{userId}")]
    [Authorize]  // Требуется авторизация
    public async Task<IActionResult> GetUserTodoLists(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("UserId не может быть пустым."); // HTTP 400

        var todoLists = await _todoListStorage.GetUserTodoListsAsync(userId, cancellationToken);
        return Ok(todoLists);
    }

    // Получение конкретного списка задач по ID
    [HttpGet("{id:guid}")]
    [Authorize]  // Требуется авторизация
    public async Task<IActionResult> GetTodoListById(Guid id, CancellationToken cancellationToken)
    {
        var todoList = await _todoListStorage.GetTodoListByIdAsync(id, cancellationToken);
        if (todoList == null)
            return NotFound(); // HTTP 404

        return Ok(todoList);
    }

    // Добавление нового списка задач
    
    
    [HttpPost]
    [Authorize]  // Требуется авторизация
    public async Task<IActionResult> AddTodoList([FromBody] TodoList newTodoList, CancellationToken cancellationToken)
    {
        // Логируем данные для проверки
        Console.WriteLine($"Id: {newTodoList.Id}, Title: {newTodoList.Title}, OwnerId: {newTodoList.OwnerId}");

        try
        {
            await _todoListStorage.AddTodoListAsync(newTodoList, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Ошибка: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }


    // Обновление списка задач
    [HttpPut("{id:guid}")]
    [Authorize]  // Требуется авторизация
    public async Task<IActionResult> UpdateTodoList(Guid id, [FromBody] TodoList updatedTodoList, CancellationToken cancellationToken)
    {
        if (updatedTodoList == null)
            return BadRequest("Обновленный список задач не может быть пустым."); // HTTP 400

        await _todoListStorage.UpdateTodoListAsync(id, updatedTodoList, cancellationToken);
        return NoContent(); // HTTP 204
    }

    
    
    [HttpDelete("{id:guid}")]
    [Authorize]  // Требуется авторизация
    public async Task<IActionResult> DeleteTodoList(Guid id, CancellationToken cancellationToken)
    {
        // Проверяем, существует ли список задач
        var todoList = await _todoListStorage.GetTodoListByIdAsync(id, cancellationToken);
        if (todoList == null)
        {
            return NotFound(new { Message = "Todo list not found." });
        }

        // Проверяем наличие связанных задач
        var todos = await _todoListStorage.GetTodosByTodoListIdAsync(id, cancellationToken);
        if (todos.Any())
        {
            return Conflict(new { Message = "Cannot delete the todo list as it contains tasks." });
        }

        // Удаляем список задач
        await _todoListStorage.DeleteTodoListAsync(id, cancellationToken);

        return NoContent(); // HTTP 204
    }

    
    
    

    // Получение всех задач по ID списка задач
    [HttpGet("{id:guid}/todos")]
    public async Task<IActionResult> GetTodosByTodoListId(Guid id, CancellationToken cancellationToken)
    {
        var todos = await _todoListStorage.GetTodoListByIdAsync(id, cancellationToken);
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
