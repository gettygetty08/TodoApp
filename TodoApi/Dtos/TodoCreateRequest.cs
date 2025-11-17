using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos;

public class TodoCreateRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }
}
