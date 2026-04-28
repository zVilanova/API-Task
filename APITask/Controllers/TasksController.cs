using APITask.Data;
using APITask.DTOs;
using APITask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APITask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // Convertendo entidade <TaskItem> para DTO de resposta
    private TaskResponseDto ToResponseDto(TaskItem task)
    {
        return new TaskResponseDto
        {
            TaskItemId = task.TaskItemId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            CreatedAt = task.CreatedAt.ToString("o")
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksAsync()
    {
        // AsNoTracking() — não rastreia mudanças, melhor performance em consultas GET
        // ToListAsync() — executa o SELECT e armazena em lista de forma assíncrona
        var tasks = await _context.Tasks.AsNoTracking().ToListAsync();
        var response = tasks.Select(task => ToResponseDto(task));
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdTaskAsync(int id)
    {
        var task = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(taskId => taskId.TaskItemId == id);

        if (task is null)
            return NotFound("Task não encontrada...");
       
        return Ok(ToResponseDto(task));
    }

    [HttpPost]
    public async Task<IActionResult> PostTaskAsync([FromBody] CreateTaskDto createTaskDto)
    {
        if (createTaskDto is null)
            return BadRequest("Informações inválidas...");

        // Convertendo DTO para entidade <TaskItem>
        var taskItem = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            Priority = createTaskDto.Priority.GetValueOrDefault(),
        };

        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync();

        return StatusCode(201, ToResponseDto(taskItem));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PatchTaskAsync(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(taskId => taskId.TaskItemId == id);

        if (task is null)
            return NotFound("Task não encontrada...");

        if (updateTaskDto.Title is not null)
            task.Title = updateTaskDto.Title;

        if (updateTaskDto.Description is not null)
            task.Description = updateTaskDto.Description;

        if (updateTaskDto.Status is not null)
            task.Status = updateTaskDto.Status.Value;

        if (updateTaskDto.Priority is not null)
            task.Priority = updateTaskDto.Priority.Value;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        return Ok(ToResponseDto(task));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteByIdTaskAsync(int id)
    {
        var taskItem = await _context.Tasks.FirstOrDefaultAsync(task => task.TaskItemId == id);

        if (taskItem is null)
            return NotFound("Task não encontrada...");

        _context.Tasks.Remove(taskItem);
        await _context.SaveChangesAsync(); 

        return NoContent();
    }

}
