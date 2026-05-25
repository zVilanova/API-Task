using APITask.DTOs;
using APITask.Models.Enums;
using APITask.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APITask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksAsync([FromQuery] TaskItemPriority? priority, [FromQuery] TaskItemStatus? status)
    {
        var tasks = await _taskService.GetTasksAsync(priority, status);

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskByIdAsync(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);

        if (task is null)
            return NotFound("Task não encontrada...");
       
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> PostTaskAsync([FromBody] CreateTaskDto createTaskDto)
    {
        if (createTaskDto is null)
            return BadRequest("Informações inválidas...");

        var task = await _taskService.CreateTaskAsync(createTaskDto);

        return StatusCode(201, task);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PatchTaskAsync(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        if (updateTaskDto is null)
            return BadRequest("Informações inválidas...");

        if (updateTaskDto.Status is not null)
        {
            if (!Enum.IsDefined(typeof(TaskItemStatus), updateTaskDto.Status.Value))
                return BadRequest("Status inválido...");
        }

        if (updateTaskDto.Priority is not null)
        {
            if (!Enum.IsDefined(typeof(TaskItemPriority), updateTaskDto.Priority.Value))
                return BadRequest("Prioridade inválida...");
        }

        var task = await _taskService.UpdateTaskAsync(id, updateTaskDto);

        if (task is null)
            return NotFound("Task não encontrada...");

        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteByIdTaskAsync(int id)
    {
        var taskDeleted = await _taskService.DeleteTaskAsync(id);

        if (taskDeleted is false)
            return NotFound("Task não encontrada...");

        return NoContent();
    }
}
