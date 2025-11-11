using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
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
            modelBuilder.HasPostgresEnum<CartStatus>("cart_status", "public"); 

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
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            // Configure the Product relationship
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);
                
            // -------------------------------------------------------------
            // 3. CART AND CART ITEM CONFIGURATION
            // -------------------------------------------------------------
            
            // CartItem (Muitos-para-Muitos virtual)
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId }); // Chave composta para item do carrinho
            
            // Cart (One) to CartItem (Many)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items) // Assumindo uma coleção 'Items' na classe Cart
                .HasForeignKey(ci => ci.CartId);

            // Client (One) to Cart (One-to-One / One-to-Zero-or-One)
            // O carrinho pertence a um cliente, usando ClientId como Chave Primária/Estrangeira
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Client)
                .WithOne(cl => cl.Cart) // Assumindo uma propriedade 'Cart' na classe Client
                .HasForeignKey<Cart>(c => c.ClientId);

            // -------------------------------------------------------------
            // 4. FOREIGN KEY RELATIONSHIPS (Fluent API)
            // -------------------------------------------------------------

            // Seller (One) to Product (Many) - 💡 Adicionado
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(s => s.Products) // Assumindo uma coleção 'Products' na classe Seller
                .HasForeignKey(p => p.SellerId);

            // Product (One) to Category (Many) - 💡 Adicionado
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products) // Assumindo uma coleção 'Products' na classe Category
                .HasForeignKey(p => p.CategoryId);

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

            // Product (One) to CartItem (Many) - Configurado acima
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
                .HasOne(o => o.Currency) // Assumindo que Order tem uma FK CurrencyCode e uma nav prop Currency
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CurrencyCode)
                .IsRequired();

            // -------------------------------------------------------------
            // 5. Index Configuration (Mantidas)
            // -------------------------------------------------------------

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SellerId).IsUnique(false);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId).IsUnique(false);
        }
    }
}