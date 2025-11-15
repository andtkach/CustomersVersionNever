using Microsoft.EntityFrameworkCore;
using Worker.Data.Model;

namespace Worker.Data;

public class BackendDataContext(DbContextOptions<BackendDataContext> options) : DbContext(options)
{
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Customer> Customers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Institution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);

            // Configure one-to-many (Institution -> Customers)
            entity.HasMany(i => i.Customers)
                .WithOne(c => c.Institution)
                .HasForeignKey(c => c.InstitutionId)
                // Prevent accidental cascade delete; require explicit handling when removing an institution
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(200);

            // FK configuration
            entity.Property<Guid>(e => e.InstitutionId).IsRequired();
            entity.HasIndex(e => e.InstitutionId).HasDatabaseName("IX_Customers_InstitutionId");
        });
    }
}
