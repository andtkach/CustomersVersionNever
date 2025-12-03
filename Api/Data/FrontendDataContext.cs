using Common.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class FrontendDataContext(DbContextOptions<FrontendDataContext> options) : DbContext(options)
{
    public DbSet<Intent> Intents { get; set; }
    public DbSet<Consumer> Consumers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Intent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Entity).IsRequired().HasMaxLength(200);
            entity.Property(e => e.State).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
            entity.Property(e => e.UpdatedAtUtc).IsRequired(false);

            entity.HasIndex(e => e.Company).HasDatabaseName("IX_Intents_Company");
        });

        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => new { e.MessageId, e.ConsumerName });

            entity.Property(e => e.MessageId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ConsumerName).HasMaxLength(100).IsRequired();

            entity.HasIndex(e => new { e.MessageId, e.ConsumerName }).IsUnique();
        });
    }
}
