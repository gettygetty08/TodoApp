using System;

namespace TodoApi.Dtos;

public class TodoDto
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsDone { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
