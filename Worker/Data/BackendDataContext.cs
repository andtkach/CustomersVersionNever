using Microsoft.EntityFrameworkCore;
using Worker.Data.Model;

namespace Worker.Data;

public class BackendDataContext(DbContextOptions<BackendDataContext> options) : DbContext(options)
{
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Address> Addresses { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Institution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.HasMany(i => i.Customers)
                .WithOne(c => c.Institution)
                .HasForeignKey(c => c.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Company).HasDatabaseName("IX_Institutions_Company");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LastName).HasMaxLength(200);

            entity.Property<Guid>(e => e.InstitutionId).IsRequired();
            entity.HasIndex(e => e.InstitutionId).HasDatabaseName("IX_Customers_InstitutionId");

            entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Company).HasDatabaseName("IX_Customers_Company");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).HasMaxLength(2000);

            entity.Property(e => e.Active).HasDefaultValue(true);
            
            entity.Property<Guid>(e => e.CustomerId).IsRequired();
            entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_Documents_CustomerId");

            entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Company).HasDatabaseName("IX_Documents_Company");
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Country).IsRequired().HasMaxLength(200);
            entity.Property(e => e.City).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Street).IsRequired().HasMaxLength(200);

            entity.Property(e => e.Current).HasDefaultValue(false);

            entity.Property<Guid>(e => e.CustomerId).IsRequired();
            entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_Addresses_CustomerId");

            entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Company).HasDatabaseName("IX_Addresses_Company");
        });
    }
}
