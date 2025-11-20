using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Mappings;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : Controller
{
    private readonly ITodoRepository _repository;

    public TodosController(ITodoRepository repository)
    {
        _repository = repository;
    }

    private static DateTime ToUtc(DateTime dt)
    {
        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            _ => dt
        };
    }

    // GET: api/Todos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos()
    {
        // GET: api/Todos
        var todos = await _repository.GetAllAsync();
        return Ok(todos.ToDtoList());
    }

    // GET: api/Todos/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<TodoDto>> GetTodo(long id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            return NotFoundTodo(id);
        }

        return Ok(todo.ToDto());
    }

    // POST: api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoDto>> CreateTodo([FromBody] TodoCreateRequest request)
    {
        // ModelState判定は不要（ApiControllerがやる）
        // if (!ModelState.IsValid)
        // {
        //     // ここは本当はDTO＋ModelStateでやるべきだが、まずは最低限
        //     return ValidationProblem(ModelState);
        // }

        if (request.DueDate.HasValue)
        {
            request.DueDate = ToUtc(request.DueDate.Value);
        }

        var entity = request.ToEntity();
        var created = await _repository.AddAsync(entity);
        var dto = created.ToDto();

        // REST的には 201 + Location が正解
        return CreatedAtAction(
            nameof(GetTodo),
            new { id = dto.Id },
            dto
        );
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<TodoDto>> UpdateTodo(long id, [FromBody] TodoUpdateRequest request)
    {
        // ModelState判定は不要（ApiControllerがやる）
        // if (!ModelState.IsValid)
        // {
        //     return ValidationProblem(ModelState);
        // }

        var updated = await _repository.UpdateAsync(id, entity =>
        {
            request.UpdateEntity(entity);
        });

        if (updated == null)
        {
            return NotFoundTodo(id);
        }


        return Ok(updated.ToDto());
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTodo(long id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            return NotFoundTodo(id);
        }

        return NoContent();
    }

    private NotFoundObjectResult NotFoundTodo(long id)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Todo not found",
            Type = "https://httpstatuses.com/404",
            Detail = $"Todo with id {id} was not found."
        };

        return NotFound(problem);
    }

}