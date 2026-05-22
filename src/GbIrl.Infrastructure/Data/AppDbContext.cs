using GbIrl.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GbIrl.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<IrlItem> Items => Set<IrlItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IrlItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Sku).HasMaxLength(64).IsRequired();
            entity.Property(i => i.Description).HasMaxLength(256).IsRequired();
            entity.Property(i => i.Category).HasMaxLength(128).IsRequired();
            entity.Property(i => i.UnitCost).HasColumnType("decimal(18,2)");
            entity.Property(i => i.Condition).HasConversion<string>().HasMaxLength(32);
        });
    }
}
