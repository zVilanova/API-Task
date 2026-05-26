using APITask.DTOs;
using APITask.Models.Enums;
using APITask.Results;
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
        var result = await _taskService.GetTasksAsync(priority, status);

        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskByIdAsync(int id)
    {
        var result = await _taskService.GetTaskByIdAsync(id);

        if (result.Status == ServiceResultStatus.NotFound)
            return NotFound(result.Message);
       
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> PostTaskAsync([FromBody] CreateTaskDto createTaskDto)
    {
        if (createTaskDto is null)
            return BadRequest("Informações inválidas...");

        var result = await _taskService.CreateTaskAsync(createTaskDto);

        return StatusCode(201, result.Data);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PatchTaskAsync(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        if (updateTaskDto is null)
            return BadRequest("Informações inválidas...");

        var result = await _taskService.UpdateTaskAsync(id, updateTaskDto);

        if (result.Status == ServiceResultStatus.NotFound)
            return NotFound(result.Message);

        if (result.Status == ServiceResultStatus.ValidationError)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteByIdTaskAsync(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);

        if (result.Status == ServiceResultStatus.NotFound)
            return NotFound(result.Message);

        return NoContent();
    }
}
