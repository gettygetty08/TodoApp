using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {

    }
    public DbSet<Todo> Todos { get; set; } = default;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entity = modelBuilder.Entity<Todo>();
        entity.ToTable("todos", schema: "public");
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Id).HasColumnName("id");

        entity.Property(t => t.Title).HasColumnName("title").HasMaxLength(200).IsRequired();

        entity.Property(t => t.Description)
              .HasColumnName("description");

        entity.Property(t => t.IsDone)
              .HasColumnName("is_done")
              .IsRequired();

        entity.Property(t => t.DueDate)
              .HasColumnName("due_date")
              .HasColumnType("timestamp with time zone");

        entity.Property(t => t.CreatedAt)
              .HasColumnName("created_at")
              .HasColumnType("timestamp with time zone")
              .IsRequired();

        entity.Property(t => t.UpdatedAt)
              .HasColumnName("updated_at")
              .HasColumnType("timestamp with time zone")
              .IsRequired();

    }


}