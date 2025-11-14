using Common.Model;
using Microsoft.EntityFrameworkCore;

namespace Worker.Data;

public class FrontendDataContext(DbContextOptions<FrontendDataContext> options) : DbContext(options)
{
    public DbSet<Intent> Intents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
