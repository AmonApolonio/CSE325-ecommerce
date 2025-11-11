using Microsoft.EntityFrameworkCore;
using Npgsql;
using backend.Models;
using System.Reflection; 
namespace backend.Data
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        // =================================================================
        // DB SETS (TABELAS)
        // =================================================================

        // Core Entities
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Seller> Sellers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Currency> Currencies { get; set; } = null!;

        // Transactional Entities
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

        // Supporting Entities
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<OrderProduct> OrderProducts { get; set; } = null!;

        // =================================================================
        // MODEL AND RELATIONSHIP CONFIGURATION
        // =================================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------------------------------------------
            // 1. POSTGRESQL ENUM SETUP (CRITICAL FOR NPGSQL)
            // -------------------------------------------------------------

            // Ensure Npgsql knows how to map C# enums to PostgreSQL custom types
            modelBuilder.HasPostgresEnum<OrderStatus>("order_status", "public");
            modelBuilder.HasPostgresEnum<PaymentStatus>("payment_status", "public");
            modelBuilder.HasPostgresEnum<CartStatus>("cart_status", "public"); // Assumindo CartStatus

            // Apply column type conversion to all entities using these enums
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasColumnType("public.order_status");

            modelBuilder.Entity<Payment>()
                .Property(p => p.TransactionStatus)
                .HasColumnType("public.payment_status");

            modelBuilder.Entity<Cart>()
                .Property(c => c.Status)
                .HasColumnType("public.cart_status");

            // -------------------------------------------------------------
            // 2. MANY-TO-MANY (OrderProduct) COMPOSITE KEY CONFIGURATION
            // -------------------------------------------------------------

            // Define the composite primary key for the junction table
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            // Configure the Order relationship
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts) // Chave da relação: Order -> OrderProducts
                .HasForeignKey(op => op.OrderId);

            // Configure the Product relationship
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts) // Chave da relação: Product -> OrderProducts
                .HasForeignKey(op => op.ProductId);

            // -------------------------------------------------------------
            // 3. FOREIGN KEY RELATIONSHIPS (Fluent API)
            // -------------------------------------------------------------

            // Client (One) to Order (Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId);

            // Order (One) to Payment (Many)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);

            // Product (One) to CartItem (Many)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId);

            // Product (One) to ProductImage (Many)
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId);

            // Currency (One) to Order (Many)
            modelBuilder.Entity<Order>()
                .HasOne<Currency>()
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CurrencyCode)
                .IsRequired();

            // -------------------------------------------------------------
            // 4. Index Configuration (Mantidas)
            // -------------------------------------------------------------

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SellerId).IsUnique(false);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId).IsUnique(false);
        }
    }
}