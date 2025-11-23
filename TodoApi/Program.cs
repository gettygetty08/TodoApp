using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

//CORSポリシー名
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 1. CORS をサービスに登録
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// PostgreSQL の接続文字列を取得
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext をDIに登録
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// 2. CORS をミドルウェアで有効にする
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program{}