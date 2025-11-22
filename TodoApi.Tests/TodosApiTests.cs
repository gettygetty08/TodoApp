using System.Net;
using System.Net.Http.Json;
using TodoApi.Dtos;
using Xunit;

namespace TodoApi.Tests;

public class TodosApiTests : IClassFixture<TestApplicationFactory>
{
    private readonly HttpClient _client;

    public TodosApiTests(TestApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_ReturnOK()
    {
        //Act
        var respone = await _client.GetAsync("/api/Todos");

        //Assert
        Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_The_GetById_Works()
    {
        //Arrange
        var createRequest = new TodoCreateRequest
        {
            Title = "Test From xUnit",
            Description = "Created in test"
        };

        //Act - create
        var createResponse = await _client.PostAsJsonAsync("/api/Todos", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<TodoDto>();

        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal(createRequest.Title, created.Title);

        //Act - get by id
        var getResponse = await _client.GetAsync($"/api/Todos/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoDto>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(createRequest.Title, fetched.Title);
    }

    [Fact]
    public async Task CreateTodo_WithoutTitle_ReturnBadRequest()
    {
        //Arrange
        var createRequest = new TodoCreateRequest
        {
            Title = "",
            Description = "No Title"
        };

        //Act
        var response = await _client.PostAsJsonAsync("api/Todos", createRequest);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // ProblemDetails として返っていることだけ軽く確認
        var problem = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(problem);
        Assert.True(problem!.ContainsKey("title")); // Title or type/title など

    }

}