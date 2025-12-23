using Microsoft.EntityFrameworkCore;

namespace Auth.Data;

public static class Extensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        // Add logging here
        Console.WriteLine("Applying database migrations...");
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine($"Database connection string {dbContext.Database.GetDbConnection().ConnectionString}");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Applying database migrations done");
    }
}
