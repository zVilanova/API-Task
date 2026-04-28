using APITask.Models.Enums;

namespace APITask.Models;

public class TaskItem
{
    public int TaskItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public TaskItemPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
