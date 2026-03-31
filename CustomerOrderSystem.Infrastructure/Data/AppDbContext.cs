using CustomerOrderSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(150).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(255).IsRequired();
            entity.Property(c => c.PhoneNumber).HasMaxLength(30);
            entity.HasIndex(c => c.Email).IsUnique();

            entity.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderDateUtc).IsRequired();
            entity.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();
            entity.HasIndex(o => o.CustomerId);

            entity.HasMany(o => o.OrderItems)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(i => i.Quantity).IsRequired();
            entity.Property(i => i.UnitPrice).HasPrecision(18, 2).IsRequired();
            entity.HasIndex(i => i.OrderId);
        });
    }
}

