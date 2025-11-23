using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Todo>> GetAllAsync()
    {
        return await _context.Todos.AsNoTracking().OrderBy(t => t.Id).ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(long id)
    {
        return await _context.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
    }
    public async Task<Todo> AddAsync(Todo todo)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task<Todo?> UpdateAsync(long id, Action<Todo> updateAction)
    {
        var existing = await _context.Todos.FindAsync(id);

        if (existing == null)
        {
            return null;
        }

        updateAction(existing);

        await _context.SaveChangesAsync();

        return existing;
    }
    public async Task<bool> DeleteAsync(long id)
    {
        var existing = await _context.Todos.FindAsync(id);

        if (existing == null)
        {
            return false;
        }

        _context.Todos.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}