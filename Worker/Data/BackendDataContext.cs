using Microsoft.EntityFrameworkCore;

namespace Worker.Data;

public class BackendDataContext(DbContextOptions<BackendDataContext> options) : DbContext(options)
{
    public DbSet<Institution> Institutions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Institution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
        });
    }
}
