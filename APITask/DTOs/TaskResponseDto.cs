namespace APITask.DTOs;

public class TaskResponseDto
{
    public int TaskItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}
