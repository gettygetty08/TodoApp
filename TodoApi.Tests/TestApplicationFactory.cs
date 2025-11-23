using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Data;

namespace TodoApi.Tests;

public class TestApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 既存の DbContext 設定を削除
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));

            if(descriptor != null)
            {
                services.Remove(descriptor);
            }

            //InMemory DBを使用
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseInMemoryDatabase("TodoTestDb");
            });


            //必要ならテスト用のデータ投入もここで行える

            
        });

    }
}