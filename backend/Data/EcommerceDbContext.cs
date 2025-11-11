using Microsoft.EntityFrameworkCore;
using backend.Models;

public class EcommerceDbContext : DbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
        : base(options)
    {
    }

    //
    // Mapping of Primary Tables
    //
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    
    //
    // Mapping of Join Tables (N:M) and Details
    //
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    // Also include DbSet for your recommendation tables, such as Recommendation, RecommendationProduct, etc.

    //
    // Model and Composite Key Configuration
    //
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Composite Key Configuration for the Orders/Products Join Table

        modelBuilder.Entity<OrderProduct>()
            // The Primary Key is composed of the Order ID and the Product ID
            .HasKey(op => new { op.OrderId, op.ProductId });

        // 2. Composite Key Configuration for the Cart/Products Join Table

        modelBuilder.Entity<CartItem>()
             .HasKey(ci => new { ci.CartId, ci.ProductId }); 

        // 3. Configuration for Column Names with Case Sensitivity 
        // Example: Maps the 'name' column in SQL to the Name property in C#
        modelBuilder.Entity<Category>().Property(c => c.Name).HasColumnName("name");
        
        // Configuration for correct ENUM type mapping
        
        // Currency mapping, ensuring the VARCHAR(3) type is respected
        modelBuilder.Entity<Currency>().Property(c => c.CurrencyCode).HasMaxLength(3);
        
     
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SellerId).IsUnique(false);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.CategoryId).IsUnique(false);
            
        modelBuilder.Entity<Order>()
        .Property(o => o.Status)
        .HasConversion<string>();
    }
}