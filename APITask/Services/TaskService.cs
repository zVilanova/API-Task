using APITask.Data;
using APITask.DTOs;
using APITask.Models;
using APITask.Models.Enums;
using APITask.Results;
using APITask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APITask.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    public TaskService(AppDbContext context)
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

    public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetTasksAsync(TaskItemPriority? priority, TaskItemStatus? status)
    {
        var query = _context.Tasks.AsNoTracking().AsQueryable(); // AsNoTracking() — não rastreia mudanças, melhor performance em consultas GET

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        var tasks = await query.ToListAsync(); // ToListAsync() — executa o SELECT e armazena em lista de forma assíncrona   

        var response = tasks.Select(t => ToResponseDto(t));
        return ServiceResult<IEnumerable<TaskResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<TaskResponseDto>> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.TaskItemId == id);

        if (task is null)
            return ServiceResult<TaskResponseDto>.NotFound("Task não encontrada...");

        return ServiceResult<TaskResponseDto>.Success(ToResponseDto(task));
    }

    public async Task<ServiceResult<TaskResponseDto>> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        var taskItem = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            Priority = createTaskDto.Priority.GetValueOrDefault(),
        };

        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync();

        return ServiceResult<TaskResponseDto>.Success(ToResponseDto(taskItem));
    }

    public async Task<ServiceResult<TaskResponseDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskItemId == id);

        if (task is null)
            return ServiceResult<TaskResponseDto>.NotFound("Task não encontrada...");

        if (updateTaskDto.Title is not null)
            task.Title = updateTaskDto.Title;

        if (updateTaskDto.Description is not null)
            task.Description = updateTaskDto.Description;

        if (updateTaskDto.Status is not null)
        {
            if (!Enum.IsDefined(typeof(TaskItemStatus), updateTaskDto.Status.Value))
                return ServiceResult<TaskResponseDto>.ValidationError("Status inválido...");

            task.Status = updateTaskDto.Status.Value;
        }

        if (updateTaskDto.Priority is not null)
        {
            if (!Enum.IsDefined(typeof(TaskItemPriority), updateTaskDto.Priority.Value))
                return ServiceResult<TaskResponseDto>.ValidationError("Prioridade inválida...");

            task.Priority = updateTaskDto.Priority.Value;
        }

        await _context.SaveChangesAsync();

        return ServiceResult<TaskResponseDto>.Success(ToResponseDto(task));
    }

    public async Task<ServiceResult<bool>> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskItemId == id);

        if (task is null)
            return ServiceResult<bool>.NotFound("Task não encontrada...");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }
}
