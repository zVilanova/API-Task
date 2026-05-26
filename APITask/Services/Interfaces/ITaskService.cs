using APITask.DTOs;
using APITask.Models.Enums;
using APITask.Results;

namespace APITask.Services.Interfaces;

public interface ITaskService
{
    Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetTasksAsync(TaskItemPriority? priority, TaskItemStatus? status);
    Task<ServiceResult<TaskResponseDto>> GetTaskByIdAsync(int id);
    Task<ServiceResult<TaskResponseDto>> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<ServiceResult<TaskResponseDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task<ServiceResult<bool>> DeleteTaskAsync(int id);
}
