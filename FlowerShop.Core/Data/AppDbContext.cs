using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
namespace FlowerShop.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Florist> Florists => Set<Florist>();
    public DbSet<Flower> Flowers => Set<Flower>();
    public DbSet<Bouquet> Bouquets => Set<Bouquet>();
    public DbSet<BouquetFlower> BouquetFlowers => Set<BouquetFlower>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BouquetFlower>()
            .HasKey(bf => new { bf.BouquetId, bf.FlowerId });
        modelBuilder.Entity<BouquetFlower>()
            .HasOne(bf => bf.Bouquet)
            .WithMany(b => b.BouquetFlowers)
            .HasForeignKey(bf => bf.BouquetId);
        modelBuilder.Entity<BouquetFlower>()
            .HasOne(bf => bf.Flower)
            .WithMany(f => f.BouquetFlowers)
            .HasForeignKey(bf => bf.FlowerId);
        modelBuilder.Entity<Florist>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<int>();
        modelBuilder.Entity<Order>()
            .HasOne(o => o.AssignedFlorist)
            .WithMany()
            .HasForeignKey(o => o.AssignedFloristId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
