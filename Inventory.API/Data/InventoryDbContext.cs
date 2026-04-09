using Microsoft.EntityFrameworkCore;
using Inventory.API.Models;

namespace Inventory.API.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Inventory.API.Models.Inventory> Inventories { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Inventory.API.Models.Inventory>()
                .HasKey(i => new { i.ProductId, i.WarehouseId });

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(m => m.Product)
                .WithMany()
                .HasForeignKey(m => m.ProductId);

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(m => m.Warehouse)
                .WithMany()
                .HasForeignKey(m => m.WarehouseId);
        }
    }
}

