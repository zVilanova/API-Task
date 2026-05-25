using APITask.DTOs;
using APITask.Models.Enums;

namespace APITask.Services.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetTasksAsync(TaskItemPriority? priority, TaskItemStatus? status);
    Task<TaskResponseDto?> GetTaskByIdAsync(int id);
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(int id);
}
