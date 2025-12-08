using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Entities;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersProduct> OrdersProducts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Recomendation> Recomendations { get; set; }

    public virtual DbSet<RecomendationsClient> RecomendationsClients { get; set; }

    public virtual DbSet<RecomendationsProduct> RecomendationsProducts { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("cart_status", new[] { "Active", "Abandoned", "Converted", "Expired" })
            .HasPostgresEnum("order_status", new[] { "Pending Payment", "Payment Confirmed", "Payment Failed", "Cancelled", "Processing", "Shipped", "Out for Delivery", "Delivered", "Completed", "Returned" })
            .HasPostgresEnum("payment_status", new[] { "Pending", "Approved", "Failed", "Refunded", "Captured" });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("cart_pkey");

            entity.ToTable("cart");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("cart_user_id_fkey");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.ProductId }).HasName("cart_items_pkey");

            entity.ToTable("cart_items");

            entity.Property(e => e.CartItemId)
                .ValueGeneratedOnAdd()
                .HasColumnName("cart_item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("cart_items_cart_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("cart_items_product_id_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address1)
                .HasMaxLength(255)
                .HasColumnName("address1");
            entity.Property(e => e.Address2)
                .HasMaxLength(255)
                .HasColumnName("address2");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(20)
                .HasColumnName("zip_code");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyCode).HasName("currencies_pkey");

            entity.ToTable("currencies");

            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasColumnName("currency_code");
            entity.Property(e => e.ExchangeRateToBrl).HasColumnName("exchange_rate_to_brl");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Symbol)
                .HasMaxLength(5)
                .HasColumnName("symbol");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ClientId)
                .ValueGeneratedOnAdd()
                .HasColumnName("client_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'BRL'::character varying")
                .HasColumnName("currency_code");
            entity.Property(e => e.FreightCents).HasColumnName("freight_cents");
            entity.Property(e => e.SubTotalCents).HasColumnName("sub_total_cents");
            entity.Property(e => e.TaxCents).HasColumnName("tax_cents");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_currency");
        });

        modelBuilder.Entity<OrdersProduct>(entity =>
        {
            entity.HasKey(e => new { e.OrdersOrderId, e.ProductsProductId }).HasName("orders_products_pkey");

            entity.ToTable("orders_products");

            entity.Property(e => e.OrdersOrderId)
                .ValueGeneratedOnAdd()
                .HasColumnName("orders_order_id");
            entity.Property(e => e.ProductsProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("products_product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.OrdersOrder).WithMany(p => p.OrdersProducts)
                .HasForeignKey(d => d.OrdersOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_products_orders_order_id_fkey");

            entity.HasOne(d => d.ProductsProduct).WithMany(p => p.OrdersProducts)
                .HasForeignKey(d => d.ProductsProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_products_products_product_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_order_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("product_id");
            entity.Property(e => e.CategoryId)
                .HasDefaultValue(0L)
                .HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Inventory).HasColumnName("inventory");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasDefaultValue(0)
                .HasColumnName("price");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");

            entity.HasOne(d => d.Seller).WithMany(p => p.Products)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_seller_id_fkey");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("product_image");

            entity.Property(e => e.Alt)
                .HasMaxLength(200)
                .HasColumnName("alt");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductImageId)
                .ValueGeneratedOnAdd()
                .HasColumnName("product_image_id");
            entity.Property(e => e.Url)
                .HasMaxLength(150)
                .HasColumnName("url");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_image_product_id_fkey");
        });

        modelBuilder.Entity<Recomendation>(entity =>
        {
            entity.HasKey(e => e.RecommendationId).HasName("recomendations_pkey");

            entity.ToTable("recomendations");

            entity.Property(e => e.RecommendationId).HasColumnName("recommendation_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Commentary).HasColumnName("commentary");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
        });

        modelBuilder.Entity<RecomendationsClient>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("recomendations_clients");

            entity.Property(e => e.ClientsUserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("clients_user_id");
            entity.Property(e => e.RecomendationsUserId).HasColumnName("recomendations_user_id");

            entity.HasOne(d => d.ClientsUser).WithMany()
                .HasForeignKey(d => d.ClientsUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recomendations_clients_clients_user_id_fkey");
        });

        modelBuilder.Entity<RecomendationsProduct>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("recomendations_products");

            entity.Property(e => e.ProductsProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Products_product_id");
            entity.Property(e => e.RecomendationsProductId).HasColumnName("recomendations_product_id");

            entity.HasOne(d => d.ProductsProduct).WithMany()
                .HasForeignKey(d => d.ProductsProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recomendations_products_Products_product_id_fkey");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("sellers_pkey");

            entity.ToTable("sellers");

            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.AboutMe).HasColumnName("about_me");
            entity.Property(e => e.Address1)
                .HasMaxLength(255)
                .HasColumnName("address1");
            entity.Property(e => e.Address2)
                .HasMaxLength(255)
                .HasColumnName("address2");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.CommisionRate)
                .HasDefaultValueSql("5")
                .HasColumnName("commision_rate");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(150)
                .HasColumnName("photo_url");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(20)
                .HasColumnName("zip_code");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
