using E_CommerceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceSystem
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProducts> OrderProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Email)
                        .IsUnique();

            modelBuilder.Entity<Product>()
       .HasOne(p => p.Category)
       .WithMany(c => c.Products)
       .HasForeignKey(p => p.CID)
       .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SID)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → User (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UID);

            // Order → OrderProducts (one-to-many)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderProducts)
                .WithOne(op => op.Order)
                .HasForeignKey(op => op.OID);
            modelBuilder.Entity<Review>()
    .HasIndex(r => new { r.UID, r.PID })
    .IsUnique(); // ✅ only one review per user per product

            modelBuilder.Entity<Product>()
    .Property(p => p.RowVersion)
    .IsRowVersion();

            modelBuilder.Entity<Order>()
                .Property(o => o.RowVersion)
                .IsRowVersion();


         


            modelBuilder.Entity<Review>()
       .HasOne(r => r.Product)
       .WithMany(p => p.Reviews)
       .HasForeignKey(r => r.PID)
       .OnDelete(DeleteBehavior.Restrict);  // ✅ prevent cascade

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UID)
                .OnDelete(DeleteBehavior.Restrict);  // ✅ pr

        }

    }
}
