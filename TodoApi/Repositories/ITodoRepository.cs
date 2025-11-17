using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    Task<List<Todo>> GetAllAsync();

    Task<Todo?> GetByIdAsync(long id);

    Task<Todo> AddAsync(Todo todo);

    Task<Todo?> UpdateAsync(long id, Action<Todo> updateAction);

    Task<bool> DeleteAsync(long id);
}
