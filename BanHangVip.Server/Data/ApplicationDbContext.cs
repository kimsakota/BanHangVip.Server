using Microsoft.EntityFrameworkCore;
using BanHangVip.Backend.Models;

namespace BanHangVip.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<HistoryItem> HistoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Cấu hình quan hệ 1-n giữa Customer và Order
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Customer nếu còn Order

            // Cấu hình quan hệ 1-n giữa Order và OrderItem
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Order thì xóa luôn OrderItems

            // ⭐ Quan hệ Product - HistoryItem
            modelBuilder.Entity<Product>()
                .HasMany(p => p.HistoryItems)
                .WithOne(h => h.Product)
                .HasForeignKey(h => h.ProductId)
                .OnDelete(DeleteBehavior.SetNull); // Xóa Product thì set ProductId = null
        }
    }
}
