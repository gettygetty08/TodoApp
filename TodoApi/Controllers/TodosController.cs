using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : Controller
{
    private readonly TodoDbContext _context;

    public TodosController(TodoDbContext context)
    {
        _context = context;
    }

    private static DateTime ToUtc(DateTime dt)
    {
        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt,DateTimeKind.Utc),
            _ => dt
        };
    }

    // GET: api/Todos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
    {
        // GET: api/Todos
        var todos = await _context.Todos
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync();

        return Ok(todos);
    }

    // GET: api/Todos/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<Todo>> GetTodo(long id)
    {
        var todo = await _context.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (todo == null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    // POST: api/Todos
    [HttpPost]
    public async Task<ActionResult<Todo>> CreateTodo([FromBody] Todo todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            // ここは本当はDTO＋ModelStateでやるべきだが、まずは最低限
            return BadRequest("Title is required.");
        }

        // クライアントから来た Id / CreatedAt / UpdatedAt は信用しない
        todo.Id = 0;
        var now = DateTime.UtcNow;
        todo.CreatedAt = now;
        todo.UpdatedAt = now;

        if (todo.DueDate.HasValue)
        {
            todo.DueDate = ToUtc(todo.DueDate.Value);
        }

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        // REST的には 201 + Location が正解
        return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<Todo>> UpdateTodo(long id, [FromBody] Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest("Route id and body id do not match.");
        }

        var existing = await _context.Todos.FindAsync(id);

        if (existing == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            return BadRequest("Title is required.");
        }

        existing.Title = todo.Title;
        existing.Description = todo.Description;
        existing.IsDone = todo.IsDone;
        existing.DueDate = todo.DueDate;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTodo(long id)
    {
        var existing = await _context.Todos.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        _context.Todos.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}