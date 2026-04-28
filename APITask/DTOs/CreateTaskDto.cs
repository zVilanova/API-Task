using APITask.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace APITask.DTOs;

public class CreateTaskDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required]
    public TaskItemPriority? Priority { get; set; }
}
