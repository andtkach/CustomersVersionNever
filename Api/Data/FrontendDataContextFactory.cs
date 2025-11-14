using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Data;

public class FrontendDataContextFactory : IDesignTimeDbContextFactory<FrontendDataContext>
{
    public FrontendDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FrontendDataContext>();
        var connectionString = "Server=localhost,1439;Database=Frontend;User Id=sa;Password=sql-password-2025!?;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);
        return new FrontendDataContext(optionsBuilder.Options);
    }
}
