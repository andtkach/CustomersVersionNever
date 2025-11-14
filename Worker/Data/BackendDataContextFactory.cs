using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Worker.Data;

public class BackendDataContextFactory : IDesignTimeDbContextFactory<BackendDataContext>
{
    public BackendDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BackendDataContext>();
        var connectionString = "Server=localhost,1439;Database=Backend;User Id=sa;Password=sql-password-2025!?;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);
        return new BackendDataContext(optionsBuilder.Options);
    }
}
