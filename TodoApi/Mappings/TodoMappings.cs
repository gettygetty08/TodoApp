using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Mappings;

public static class TodoMappings
{
    public static TodoDto ToDto(this Todo entity)
    {
        return new TodoDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsDone = entity.IsDone,
            DueDate = entity.DueDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static List<TodoDto> ToDtoList(this IEnumerable<Todo> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    public static Todo ToEntity(this TodoCreateRequest request)
    {
        var now = DateTime.UtcNow;

        return new Todo
        {
            Title = request.Title,
            Description = request.Description,
            IsDone = false,
            DueDate = request.DueDate,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    public static void UpdateEntity(this TodoUpdateRequest request, Todo entity)
    {
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.IsDone = request.IsDone;
        entity.DueDate = request.DueDate;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}