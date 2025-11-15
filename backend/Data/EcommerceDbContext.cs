using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using backend.Models;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

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

            modelBuilder.HasPostgresEnum<OrderStatus>("order_status", "public");
            modelBuilder.HasPostgresEnum<PaymentStatus>("payment_status", "public");
            modelBuilder.HasPostgresEnum<CartStatus>("cart_status", "public"); 

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

            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);
                
            // -------------------------------------------------------------
            // 3. CART AND CART ITEM CONFIGURATION
            // -------------------------------------------------------------
            
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId }); 
            
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items) 
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Client)
                .WithOne(cl => cl.Cart) 
                .HasForeignKey<Cart>(c => c.ClientId);

            // -------------------------------------------------------------
            // 4. FOREIGN KEY RELATIONSHIPS (Fluent API)
            // -------------------------------------------------------------

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(s => s.Products) 
                .HasForeignKey(p => p.SellerId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products) 
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems) 
                .HasForeignKey(ci => ci.ProductId);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Currency) 
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CurrencyCode)
                .IsRequired();

            // -------------------------------------------------------------
            // 5. Index Configuration
            // -------------------------------------------------------------

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SellerId).IsUnique(false);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId).IsUnique(false);
                
            // -------------------------------------------------------------
            // 6. CURRENCY DATA SEEDING
            // -------------------------------------------------------------
            modelBuilder.Entity<Currency>().HasData(GetCurrencySeedData());

            // -------------------------------------------------------------
            // 7. CATEGORY DATA SEEDING
            // -------------------------------------------------------------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId)
                      .ValueGeneratedNever(); 
                entity.HasData(GetCategorySeedData());
            });

            // -------------------------------------------------------------
            // 8. SELLER DATA SEEDING
            // -------------------------------------------------------------
            modelBuilder.Entity<Seller>(entity =>
            {
                entity.Property(e => e.SellerId)
                      .ValueGeneratedNever(); 
                entity.HasData(GetSellerSeedData());
            });
            
            // -------------------------------------------------------------
            // 9. PRODUCT DATA SEEDING
            // -------------------------------------------------------------
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId)
                      .ValueGeneratedNever(); 
                entity.HasData(GetProductSeedData());
            });

            // -------------------------------------------------------------
            // 10. PRODUCT IMAGE DATA SEEDING (CARGA DO SEU MODELO)
            // -------------------------------------------------------------
            modelBuilder.Entity<ProductImage>(entity =>
            {
                // DESABILITA A GERAÇÃO DE VALOR para permitir que o HasData insira o ID
                entity.Property(e => e.ProductImageId)
                      .ValueGeneratedNever(); 
                entity.HasData(GetProductImageSeedData());
            });
        }
        
        // =================================================================
        // DATA SEEDING LOGIC
        // =================================================================

        private const string MockImageUrlBase = "https://mock-cloud-storage.com/";
        private const string BaseImagePath = "images/products"; 
        private const string MockPasswordHash = "MOCK_HASH_FOR_SEEDING_12345"; 

        // Mapeamento dos novos IDs de Produto (Vidro e Metal) para os nomes de arquivo de imagem
        // A chave é o ProductId e o valor é o nome do arquivo.
        private static readonly Dictionary<long, string> NewProductImagesMap = new Dictionary<long, string>
        {
            // PRODUTOS DE VIDRO (Ethereal Glassworks - Categoria 3)
            { 39, "blown_glass_ornament.png" },
            { 40, "blown_glass_vase.png" },
            { 41, "crystal_orb.png" },
            { 42, "glass_bell.png" },
            { 43, "glass_coasters.png" },
            { 44, "glass_decanter.png" },
            { 45, "glass_paperweight.png" },
            { 46, "glass_tumbler_set.png" },
            { 47, "stained_glass_panel.png" },

            // PRODUTOS DE METAL (Forged Metals - Categoria 4)
            { 48, "decorative_steel_hook.png" },
            { 49, "forged_metal_bottle_rack.png" },
            { 50, "hand_forged_fire_poker.png" },
            { 51, "hand_forged_hook.jpg" },
            { 52, "hand_forged_sculpture.png" },
            { 53, "institutional_image" }, // Produto fictício
            { 54, "iron_grill.png" },
            { 55, "metal_art_piece.png" },
            { 56, "steel_bottle_opener.png" },
            { 57, "wrought_iron_candlestick.png" },
            { 58, "wrought_iron_coasters.png" }
        };

        private static List<Currency> GetCurrencySeedData()
        {
            return new List<Currency>
            {
                new Currency { CurrencyCode = "BRL", Name = "Brazilian Real", Symbol = "R$", ExchangeRateToBRL = 1.00m },
                new Currency { CurrencyCode = "USD", Name = "United States Dollar", Symbol = "$", ExchangeRateToBRL = 5.00m },
                new Currency { CurrencyCode = "EUR", Name = "Euro", Symbol = "€", ExchangeRateToBRL = 5.50m }
            };
        }

        private static List<Category> GetCategorySeedData()
        {
            return new List<Category>
            {
                new Category { CategoryId = 1, Name = "Canvas & Brush", Description = "Materials for canvas painting, easels, and fine arts supplies." },
                new Category { CategoryId = 2, Name = "Clay & Glaze", Description = "Specialty clays, earthenware, modeling tools, and glazes for ceramics." },
                new Category { CategoryId = 3, Name = "Ethereal Glassworks", Description = "Kits and materials for glass blowing, fusing, and creating glass mosaics." },
                new Category { CategoryId = 4, Name = "Forged Metals", Description = "Tools and metals for forging, welding, and creating decorative metal pieces." },
                new Category { CategoryId = 5, Name = "Glimmerstone Jewels", Description = "Raw stones, beads, wires, and clasps for assembling handmade jewelry." },
                new Category { CategoryId = 6, Name = "Heritage Leathers", Description = "Leathers of different textures, stamps, dyes, and needles for leatherwork." },
                new Category { CategoryId = 7, Name = "Paper & Ink", Description = "Rice papers, India inks, pens, and materials for calligraphy and bookbinding." },
                new Category { CategoryId = 8, Name = "Timbercraft", Description = "Selected woods, lathes, and precision tools for woodworking and carving." },
                new Category { CategoryId = 9, Name = "Wicker & Weave", Description = "Wicker, bamboo, and other natural fibers for basketry, braiding, and weaving." },
                new Category { CategoryId = 10, Name = "Wool & Whimsey", Description = "Natural wools, exotic yarns, and accessories for knitting, crochet, and needle felting." },
                new Category { CategoryId = 11, Name = "Stitch & Thread", Description = "Various fabrics, embroidery threads, needles, and machines for sewing and quilting." }
            };
        }
        
        private static List<Seller> GetSellerSeedData()
        {
            return new List<Seller>
            {
                new Seller
                {
                    SellerId = 1, Name = "Clara's Canvas Corner", PhotoUrl = "https://placehold.co/150x150/000/fff?text=CC", AboutMe = "Specialist in painting and fine embroidery, focusing on sustainable materials. Offers materials for the Canvas & Brush and Stitch & Thread categories.",
                    Address1 = "Rua das Artes, 100", Address2 = "Bloco A", City = "São Paulo", State = "SP", Country = "Brazil", ZipCode = "01000-000",
                    PhoneNumber = "11987654321", CommisionRate = 5.0m, Email = "clara.artesanato@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 2, Name = "Metal & Fire Forge", PhotoUrl = "https://placehold.co/150x150/333/fff?text=MFF", AboutMe = "Workshop focused on forged metal pieces and fused glass sculptures. Serves the Forged Metals and Ethereal Glassworks categories.",
                    Address1 = "Avenida Industrial, 50", Address2 = "Galpão 3", City = "Curitiba", State = "PR", Country = "Brazil", ZipCode = "80000-000",
                    PhoneNumber = "41998765432", CommisionRate = 7.5m, Email = "metal.fire@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 3, Name = "Rustic Wood & Leather", PhotoUrl = "https://placehold.co/150x150/666/fff?text=RWL", AboutMe = "Masters in the art of rustic joinery and traditional leather pieces. Sells products for Timbercraft, Heritage Leathers, and Wicker & Weave.",
                    Address1 = "Praça da Matriz, 22", Address2 = "", City = "Belo Horizonte", State = "MG", Country = "Brazil", ZipCode = "30000-000",
                    PhoneNumber = "31976543210", CommisionRate = 5.0m, Email = "rustic.crafts@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 4, Name = "The Potter's Gem", PhotoUrl = "https://placehold.co/150x150/8B4513/fff?text=TPG", AboutMe = "Dedicated to the art of forming clay and crafting delicate jewelry. Covers the Clay & Glaze and Glimmerstone Jewels categories.",
                    Address1 = "Rua do Ouro, 350", Address2 = "Andar 2", City = "Rio de Janeiro", State = "RJ", Country = "Brazil", ZipCode = "20000-000",
                    PhoneNumber = "21912345678", CommisionRate = 6.0m, Email = "potters.gem@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 5, Name = "Felt & Parchment Studio", PhotoUrl = "https://placehold.co/150x150/A0522D/fff?text=FPS", AboutMe = "Specialty studio for high-quality paper, inks, and natural wools. Covers the Paper & Ink and Wool & Whimsey categories.",
                    Address1 = "Avenida dos Papeis, 1500", Address2 = "Sala 101", City = "Porto Alegre", State = "RS", Country = "Brazil", ZipCode = "90000-000",
                    PhoneNumber = "51923456789", CommisionRate = 5.0m, Email = "felt.parchment@email.com", PasswordHash = MockPasswordHash
                }
            };
        }
        
        private static List<Product> GetProductSeedData()
        {
            // Price is stored as 'int' (using cents). Ex: 4599 = $45.99
            var products = new List<Product>
            {
                // PRODUTOS EXISTENTES (1 a 38)
                new Product { ProductId = 1, Name = "Premium Acrylic Paint Set (12 Colors)", Description = "High-viscosity acrylic paints ideal for canvas and professional artwork. Non-toxic and fast-drying.", Price = 4599, Inventory = 150.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 2, Name = "Hand-Thrown Stoneware Bowl (Small)", Description = "Locally sourced stoneware clay, pre-mixed with a forest-green glaze. Perfect for firing at cone 6.", Price = 2250, Inventory = 40.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 3, Name = "Glass Fusing Starter Kit - Dichroic", Description = "Complete kit for glass fusing, including safety gloves, cutting tools, and a selection of dichroic glass pieces.", Price = 9800, Inventory = 20.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 4, Name = "Wrought Iron Candle Holder (Medieval Style)", Description = "Heavy-duty wrought iron piece, hand-forged using traditional blacksmithing methods. Durable matte black finish.", Price = 7500, Inventory = 15.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 5, Name = "Raw Amethyst Geode (Small, 50g)", Description = "Natural Uruguayan Amethyst geode. Ideal for wire wrapping and jewelry making. Each stone is unique.", Price = 3500, Inventory = 75.0m, SellerId = 4, CategoryId = 5 },
                new Product { ProductId = 6, Name = "Vegetable-Tanned Cowhide (A4 size)", Description = "Premium full-grain leather, naturally tanned using vegetable extracts. Perfect thickness for wallets and small goods.", Price = 12000, Inventory = 30.0m, SellerId = 3, CategoryId = 6 },
                new Product { ProductId = 7, Name = "Japanese Calligraphy Ink Set (Sumi)", Description = "Traditional Sumi ink and five bamboo pens, housed in a wooden box. Suitable for calligraphy and detailed drawing.", Price = 3990, Inventory = 60.0m, SellerId = 5, CategoryId = 7 },
                new Product { ProductId = 8, Name = "Hand-Carved Oak Chopping Board", Description = "Made from sustainable Oakwood, treated with mineral oil. A rustic, durable board ideal for kitchen use or display.", Price = 5500, Inventory = 25.0m, SellerId = 3, CategoryId = 8 },
                new Product { ProductId = 9, Name = "Large Woven Bamboo Basket (Storage)", Description = "Hand-woven from natural, flexible bamboo fibers. Perfect for laundry or general home storage.", Price = 8500, Inventory = 35.0m, SellerId = 3, CategoryId = 9 },
                new Product { ProductId = 10, Name = "Merino Wool Skein (Sky Blue, 100g)", Description = "100% pure Merino wool, super soft and non-irritating. Perfect for baby clothes and sensitive skin projects.", Price = 1500, Inventory = 200.0m, SellerId = 5, CategoryId = 10 },
                new Product { ProductId = 11, Name = "Beginner Sewing Machine (Portable)", Description = "Compact and lightweight sewing machine with 12 built-in stitches. Includes thread, needles, and foot pedal.", Price = 29999, Inventory = 50.0m, SellerId = 1, CategoryId = 11 },
                new Product { ProductId = 12, Name = "Abstract Acrylic on Canvas", Description = "Original painting featuring vibrant, thick acrylic layers on a gallery-wrapped canvas. Signed by the artist.", Price = 35000, Inventory = 5.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 13, Name = "Abstract Art Digital Print", Description = "High-quality archival giclée print of a digital abstract piece. Available in multiple sizes, ready for framing.", Price = 4500, Inventory = 100.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 14, Name = "Acrylic-Primed Stretched Canvas (Large)", Description = "Triple-primed, 100% cotton canvas, perfect for large-scale acrylic and oil painting. Ready to hang.", Price = 7800, Inventory = 80.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 15, Name = "Professional Acrylic Paint Set (24 Colors)", Description = "Complete set of 24 artist-grade, heavy-body acrylic paints. High pigment concentration and lightfastness.", Price = 6899, Inventory = 120.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 16, Name = "Deluxe Artist Brush Set (20 pcs)", Description = "Assortment of 20 synthetic and natural hair brushes for oil, acrylic, and watercolor techniques. Includes travel case.", Price = 4200, Inventory = 150.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 17, Name = "Custom Pet Portrait Commission", Description = "Original, personalized oil painting of your pet on a linen canvas. Requires high-resolution photo reference.", Price = 150000, Inventory = 5.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 18, Name = "Graphite Drawing Pencil Set (H-B range)", Description = "Set of 12 professional graphite pencils, ranging from 6H (hard) to 8B (soft), ideal for detailed drawing.", Price = 2150, Inventory = 300.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 19, Name = "Master Oil Paint Set (12 Colors, Fine Art)", Description = "A curated selection of 12 premium, pigment-rich oil paints, favored by classical artists. Excellent texture.", Price = 9999, Inventory = 90.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 20, Name = "Framed Landscape Canvas Print", Description = "Museum-quality Giclée print of a scenic landscape, professionally framed with anti-glare glass.", Price = 12000, Inventory = 50.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 21, Name = "Large Format Institutional Easel", Description = "Heavy-duty, floor-standing H-frame easel made from seasoned beechwood. Suitable for canvases up to 80 inches.", Price = 45000, Inventory = 10.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 22, Name = "Original Landscape Oil Painting", Description = "Unique original oil painting capturing a dramatic mountain vista. Varnished and ready to display.", Price = 48000, Inventory = 1.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 23, Name = "Mixed Media Paper Pad (A3, 30 sheets)", Description = "Thick, durable, textured paper pad suitable for wet and dry techniques, including watercolor, gouache, and ink.", Price = 2990, Inventory = 250.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 24, Name = "Classic Portrait Oil Painting (Commission)", Description = "High-detail, traditional oil portrait commission on a premium canvas. Consultation required for execution.", Price = 95000, Inventory = 3.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 25, Name = "Original Oil Painting: Misty Meadow", Description = "A serene, atmospheric original oil work, characterized by soft focus and subtle colors. Unframed.", Price = 52000, Inventory = 1.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 26, Name = "Premium Plein Air Sketchbook (A5)", Description = "Pocket-sized sketchbook with perforated pages and a durable cover, perfect for sketching outdoors (plein air).", Price = 1850, Inventory = 400.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 27, Name = "Leather-Bound Sketchbook (Handmade)", Description = "Hand-stitched sketchbook with a genuine leather cover and high-quality acid-free drawing paper. A true heirloom piece.", Price = 7500, Inventory = 30.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 28, Name = "Custom Watercolor Portrait (Small)", Description = "Personalized watercolor portrait on heavy cotton paper. Ideal for gifts or small displays.", Price = 60000, Inventory = 10.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 29, Name = "Watercolor Portrait Print (Limited Edition)", Description = "Signed and numbered limited edition print of a popular watercolor portrait series. Comes with certificate of authenticity.", Price = 8500, Inventory = 40.0m, SellerId = 1, CategoryId = 1 },
                new Product { ProductId = 30, Name = "Studio H-Frame Wooden Easel", Description = "A solid, adjustable H-frame easel for studio work, offering maximum stability for various canvas sizes.", Price = 15000, Inventory = 20.0m, SellerId = 1, CategoryId = 1 },

                // PRODUTOS EXISTENTES - CLAY & GLAZE (Categoria 2, Vendedor 4)
                new Product { ProductId = 31, Name = "Hand-Painted Ceramic Coasters (Set of 4)", Description = "Unique set of four ceramic coasters, hand-painted and kiln-fired with a protective glaze. Includes cork backing.", Price = 3999, Inventory = 80.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 32, Name = "Tall Geometric Ceramic Vase", Description = "Modern, tall vase with a matte finish and geometric texture, perfect for dried arrangements or minimalist decor.", Price = 7990, Inventory = 30.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 33, Name = "Rustic Clay Dinner Plate", Description = "Large, durable dinner plate made from locally sourced clay with a rustic, uneven edge and semi-matte glaze.", Price = 2550, Inventory = 120.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 34, Name = "Miniature Fox Figurine Sculpture", Description = "Small, detailed sculpture of a resting fox, finished with a smooth, glossy glaze. Ideal for desks or shelves.", Price = 4900, Inventory = 50.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 35, Name = "Large Ocean-Blue Glazed Serving Bowl", Description = "Beautifully hand-glazed ceramic serving bowl in deep ocean blue, ideal for salads, fruit, or as a centerpiece.", Price = 5999, Inventory = 45.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 36, Name = "Pottery Mixing Bowl Set (3 Sizes)", Description = "Set of three nesting earthenware mixing bowls (small, medium, large), durable and perfect for baking and prep.", Price = 11500, Inventory = 25.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 37, Name = "Japanese Style Sake Cup Set (4 pcs)", Description = "Set of four small, hand-thrown sake cups with a delicate, speckled glaze, perfect for traditional or modern serving.", Price = 4590, Inventory = 60.0m, SellerId = 4, CategoryId = 2 },
                new Product { ProductId = 38, Name = "Large Terracotta Storage Jar", Description = "Rustic, unglazed terracotta jar with a tight-fitting cork lid, suitable for dry goods storage or decoration.", Price = 6500, Inventory = 40.0m, SellerId = 4, CategoryId = 2 },

                // PRODUTOS EXISTENTES - ETHEREAL GLASSWORKS (Categoria 3, Vendedor 2)
                new Product { ProductId = 39, Name = "Blown Glass Ornament (Limited Edition)", Description = "Delicate, hand-blown glass ornament with shimmering iridescence. Perfect for display or holiday decor.", Price = 4500, Inventory = 100.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 40, Name = "Elegant Blown Glass Vase (Tall)", Description = "Tall, slender vase created through traditional glass blowing techniques. Features a subtle blue gradient.", Price = 12000, Inventory = 30.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 41, Name = "Faceted Crystal Orb Paperweight", Description = "Solid, precision-cut crystal orb that refracts light beautifully. Ideal as a desk accessory or collector's item.", Price = 6500, Inventory = 50.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 42, Name = "Hand-Blown Glass Bell Sculpture", Description = "A unique glass sculpture shaped like a bell, producing a delicate, resonant tone. Decorative centerpiece.", Price = 9500, Inventory = 25.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 43, Name = "Set of 4 Fused Glass Coasters", Description = "Set of four durable glass coasters created by fusing colored glass sheets. Includes small rubber feet for protection.", Price = 5500, Inventory = 70.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 44, Name = "Engraved Crystal Glass Decanter", Description = "Premium, heavy-bottomed crystal decanter with detailed diamond-cut engraving. Suitable for whiskey or spirits.", Price = 18000, Inventory = 20.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 45, Name = "Artisan Glass Paperweight (Swirl)", Description = "Circular glass paperweight featuring an intricate, colorful internal swirl design. Signed by the artisan.", Price = 3200, Inventory = 80.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 46, Name = "Deluxe Glass Tumbler Set (6 Pcs)", Description = "Set of six high-quality glass tumblers with a weighted base. Perfect for everyday use or entertaining.", Price = 7500, Inventory = 40.0m, SellerId = 2, CategoryId = 3 },
                new Product { ProductId = 47, Name = "Custom Stained Glass Panel (Art Deco)", Description = "A commission for a unique Art Deco-style stained glass window panel. Price is an estimate; final cost depends on size/complexity.", Price = 450000, Inventory = 5.0m, SellerId = 2, CategoryId = 3 },

                // ---------------------------------------------------------------------
                // NOVOS PRODUTOS - FORGED METALS (Categoria 4, Vendedor 2)
                // Usando IDs a partir de 48
                // ---------------------------------------------------------------------
                new Product { ProductId = 48, Name = "Decorative Steel Hook (Wall Mount)", Description = "Hand-forged steel wall hook with a decorative scroll design. Ideal for coats or tools. Matte black finish.", Price = 2200, Inventory = 150.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 49, Name = "Forged Metal Bottle Rack (6 Bottles)", Description = "Wrought iron wine rack, holds six standard bottles securely. Rustic patina finish.", Price = 7900, Inventory = 35.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 50, Name = "Hand-Forged Fire Poker (Heavy Duty)", Description = "Solid steel fire poker, designed for heavy use in fireplaces or wood stoves. Ergonomic handle.", Price = 6500, Inventory = 20.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 51, Name = "Hand-Forged Utility Hook (Rustic)", Description = "Simple, durable iron utility hook. Excellent for kitchens or garages where a rustic look is desired.", Price = 1800, Inventory = 200.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 52, Name = "Hand-Forged Abstract Sculpture (Desk Size)", Description = "Small abstract sculpture made from twisted, polished steel bars. Unique art piece for a modern desk.", Price = 15000, Inventory = 10.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 53, Name = "Institutional Photo Placeholder", Description = "Placeholder product representing a general institutional image or item. Not for sale.", Price = 000, Inventory = 0.0m, SellerId = 2, CategoryId = 4 }, // Fictício
                new Product { ProductId = 54, Name = "Custom Iron Grill Insert (Heavy Gauge)", Description = "Made-to-order heavy-gauge iron grill or grate insert. Requires dimensions. High heat resistance.", Price = 35000, Inventory = 10.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 55, Name = "Abstract Metal Wall Art Piece (Small)", Description = "Geometric wall hanging created from welded metal scraps. Finished with a protective clear coat.", Price = 9800, Inventory = 15.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 56, Name = "Steel Bottle Opener (Keyring Style)", Description = "Compact and durable bottle opener made from brushed stainless steel. Ideal for keychains.", Price = 1250, Inventory = 300.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 57, Name = "Wrought Iron Candlestick (Set of 2)", Description = "Pair of elegant wrought iron candlesticks, hand-twisted design. Suitable for taper candles.", Price = 4999, Inventory = 70.0m, SellerId = 2, CategoryId = 4 },
                new Product { ProductId = 58, Name = "Wrought Iron Coasters (Set of 4)", Description = "Set of four iron coasters with a decorative base and felt pads to protect surfaces. Matte black finish.", Price = 5500, Inventory = 80.0m, SellerId = 2, CategoryId = 4 }
            };

            return products;
        }

        private static List<ProductImage> GetProductImageSeedData()
        {
            var products = GetProductSeedData();
            var images = new List<ProductImage>();
            long imageIdCounter = 1;

            // Mapeamento de produtos com nomes de arquivo específicos (Vidro e Metal)
            var specificProductsToImageFile = products
                .Where(p => NewProductImagesMap.ContainsKey(p.ProductId))
                .ToDictionary(p => p.ProductId, p => NewProductImagesMap[p.ProductId]);


            foreach (var product in products)
            {
                long sellerId = product.SellerId;
                long categoryId = product.CategoryId;

                // --- LÓGICA ESPECIAL PARA PRODUTOS COM IMAGEM ESPECÍFICA (IDs 39 em diante) ---
                if (specificProductsToImageFile.TryGetValue(product.ProductId, out var filename))
                {
                    // Usa o nome de arquivo fornecido para a URL
                    string productUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{filename}";
                    
                    images.Add(new ProductImage
                    {
                        ProductImageId = imageIdCounter++,
                        ProductId = product.ProductId,
                        Url = productUrl,
                        Alt = $"Imagem principal do produto artesanal: {product.Name}"
                    });
                    // Ignora a lógica de detalhes genéricos para esses produtos
                    continue; 
                }
                
                // --- LÓGICA EXISTENTE PARA PRODUTOS ANTIGOS (IDs 1 a 38) ---
                
                // 1. Imagem Principal
                string mainAlt = $"Imagem principal de {product.Name}.";
                // A imagem principal usa o ProductId e '_main.jpg'
                string mainUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{product.ProductId}_main.jpg";

                images.Add(new ProductImage
                {
                    ProductImageId = imageIdCounter++,
                    ProductId = product.ProductId,
                    Url = mainUrl,
                    Alt = mainAlt
                });

                // 2. Imagens de Detalhe (Simulação de 1 a 2 imagens de detalhe)
                int detailCount;
                if (product.ProductId >= 31)
                {
                    // Produtos cerâmicos tem 2 imagens de detalhe
                    detailCount = 2;
                }
                else
                {
                    // Lógica antiga (1 ou 2 imagens de detalhe)
                    detailCount = (product.ProductId % 2 == 0 && product.ProductId >= 12) ? 2 : 1;
                    if(product.ProductId < 12)
                    {
                        detailCount = 1;
                    }
                }
                

                for (int i = 1; i <= detailCount; i++)
                {
                    string detailAlt = $"Detalhe {i} de {product.Name}.";
                    string detailUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{product.ProductId}_detail_{i}.jpg";

                    images.Add(new ProductImage
                    {
                        ProductImageId = imageIdCounter++,
                        ProductId = product.ProductId,
                        Url = detailUrl,
                        Alt = detailAlt
                    });
                }
            }
            
            return images;
        }
    }
}