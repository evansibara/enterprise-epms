using EPMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace EPMS.IntegrationTests;

/// <summary>
/// Factory yang menjalankan EPMS.WebApi di memory dengan PostgreSQL asli
/// di dalam container Docker (Testcontainers.PostgreSql, sesuai section 6
/// dokumen), bukan in-memory provider, supaya perilaku query/migration
/// benar-benar diuji terhadap database nyata.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("epms_test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgresContainer.GetConnectionString(),
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["Jwt:Secret"] = "INTEGRATION_TEST_SECRET_KEY_MINIMAL_32_KARAKTER",
                ["Jwt:Issuer"] = "EPMS.IntegrationTests",
                ["Jwt:Audience"] = "EPMS.IntegrationTests"
            });
        });

        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        });
    }

    public async Task InitializeAsync() => await _postgresContainer.StartAsync();

    public new async Task DisposeAsync() => await _postgresContainer.StopAsync();
}
