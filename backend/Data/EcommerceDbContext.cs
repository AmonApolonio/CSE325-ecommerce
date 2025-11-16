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
        // DB SETS (TABLES)
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
            // 6. CURRENCY DATA SEEDING (APLICADA MELHORIA DE CONSISTÊNCIA)
            // -------------------------------------------------------------
            modelBuilder.Entity<Currency>(entity =>
            {
                // Garante que o PK definido no HasData não seja gerado pelo banco.
                // Ajuste 'CurrencyCode' se o nome da PK for diferente.
                entity.Property(e => e.CurrencyCode) 
                      .ValueGeneratedNever(); 
                entity.HasData(GetCurrencySeedData());
            });

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
            // 9. CLIENT DATA SEEDING (NEW ADDITION)
            // -------------------------------------------------------------
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.UserId)
                      .ValueGeneratedNever(); 
                entity.HasData(GetClientSeedData());
            });

            // -------------------------------------------------------------
            // 10. PRODUCT DATA SEEDING
            // -------------------------------------------------------------
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId)
                      .ValueGeneratedNever(); 
                entity.HasData(GetProductSeedData());
            });

            // -------------------------------------------------------------
            // 11. PRODUCT IMAGE DATA SEEDING (YOUR MODEL LOAD)
            // -------------------------------------------------------------
            modelBuilder.Entity<ProductImage>(entity =>
            {
                // DISABLE VALUE GENERATION to allow HasData to insert the ID
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

        // Mapping of Product IDs to image filenames.
        private static readonly Dictionary<long, string> NewProductImagesMap = new Dictionary<long, string>
        {
            // IDs 39-47: Category 3 (Ethereal Glassworks)
            { 39, "blown_glass_ornament.png" },
            { 40, "blown_glass_vase.png" },
            { 41, "crystal_orb.png" },
            { 42, "glass_bell.png" },
            { 43, "glass_coasters.png" },
            { 44, "glass_decanter.png" },
            { 45, "glass_paperweight.png" },
            { 46, "glass_tumbler_set.png" },
            { 47, "stained_glass_panel.png" },

            // IDs 48-57: Category 4 (Forged Metals)
            { 48, "decorative_steel_hook.png" },
            { 49, "forged_metal_bottle_rack.png" },
            { 50, "hand_forged_fire_poker.png" },
            { 51, "hand_forged_hook.jpg" },
            { 52, "hand_forged_sculpture.png" },
            { 53, "iron_grill.png" },                   
            { 54, "metal_art_piece.png" },              
            { 55, "steel_bottle_opener.png" },          
            { 56, "wrought_iron_candlestick.png" },     
            { 57, "wrought_iron_coasters.png" },        

            // IDs 58-68: Category 5 (Glimmerstone Jewels)
            { 58, "amethyst_earrings.png" },            
            { 59, "beaded_necklace.png" },              
            { 60, "crystal_earrings.png" },             
            { 61, "cuff_bracelet.jpg" },                
            { 62, "delicate_gold_chain.jpg" },          
            { 63, "garnet_pendant.png" },               
            { 64, "gemstone_bracelet.png" },            
            { 65, "pearl_brooch.png" },                 
            { 66, "pendant_charm.png" },                
            { 67, "silver_ring.png" },                  
            { 68, "turquoise_ring.png" },               

            // IDs 69-78: Category 6 (Heritage Leathers)
            { 69, "journal_cover.png" },                
            { 70, "key_fob.png" },                      
            { 71, "leather_belt.png" },                 
            { 72, "leather_card_holder.png" },          
            { 73, "leather_cord_organizer.png" },       
            { 74, "leather_journal_cover.jpg" },        
            { 75, "leather_laptop_sleeve.png" },        
            { 76, "leather_passport_holder.png" },      
            { 77, "leather_wallet.png" },               
            { 78, "satchel_bag.png" },                  

            // IDs 79-88: Category 7 (Paper & Ink)
            { 79, "artistic_note_cards.png" },          
            { 80, "calligraphy_set.png" },              
            { 81, "custom_stationery.png" },            
            { 82, "custom_stationery_set.jpg" },        
            { 83, "custom_stationery_set_1.jpg" },      
            { 84, "hand_bound_journal.png" },           
            { 85, "handmade_envelopes.png" },           
            { 86, "ink_pen_set.png" },                  
            { 87, "letterpress_prints.png" },           
            { 88, "personalized_wax_seal.png" },        
            
            // IDs 89-98: Category 11 (Stitch & Thread)
            { 89, "custom_embroined_bag.png" },
            { 90, "hand_stitched_tapestry.png" },
            { 91, "quilted_blanket.png" },
            { 92, "embroidered_patch.png" },
            { 93, "handmade_fabric_coasters.png" },
            { 94, "silk_ribbon_art.png" },
            { 95, "embroidered_wall_hoop.png" },
            { 96, "textile_art.png" }, 
            { 97, "floral_embroidered_hoop.jpg" },
            { 98, "patterned_cushion.png" },
            
            // IDs 99-109: Category 8 (Timbercraft)
            { 99, "abstract_wood_sculpture.jpg" },
            { 100, "carved_wood_statue.png" },
            { 101, "coasters.png" },
            { 102, "decorative_box.png" },
            { 103, "hand_carved_spoon.png" },
            { 104, "hand_carved_whistle.png" },
            { 105, "wood_turned_salt_cellar.png" },
            { 106, "wooden_bowl.png" },
            { 107, "wooden_cutting_board.png" },
            { 108, "wooden_serving_board.jpg" },
            { 109, "wooden_tray.png" },

            // IDs 110-122: Category 9 (Wicker & Weave)
            { 110, "bamboo_fruit_bowl.jpg" },
            { 111, "decorative_tray.png" },
            { 112, "rattan_basket.jpg" },
            { 113, "rattan_coasters.png" },
            { 114, "rattan_lamp.png" },
            { 115, "rope_planter.png" },
            { 116, "seagrass_mat.png" },
            { 117, "wicker_basket.png" },
            { 118, "wicker_pendant_lamp.jpg" },
            { 119, "woven_chair.png" },
            { 120, "woven_storage_box.jpg" },
            { 121, "woven_wall_decor.png" },
            { 122, "woven_wall_hanging.jpg" },

            // IDs 123-133: Category 10 (Wool & Whimsy)
            { 123, "chunky_throw_blanket.jpg" },
            { 124, "felted_slippers.png" },
            { 125, "hand-knit_gloves.png" },
            { 126, "hand_knitted_scarf.jpg" },
            { 127, "knitted_dog_sweater.png" },
            { 128, "knitted_scarf.png" },
            { 129, "knitted_sweater.png" },
            { 130, "wool_beanie.png" },
            { 131, "wool_blanket.png" },
            { 132, "wool_headband.png" },
            { 133, "woolen_socks.png" }
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
                    Address1 = "Art Street, 100", Address2 = "Block A", City = "São Paulo", State = "SP", Country = "Brazil", ZipCode = "01000-000",
                    PhoneNumber = "11987654321", CommisionRate = 5.0m, Email = "clara.artesanato@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 2, Name = "Metal & Fire Forge", PhotoUrl = "https://placehold.co/150x150/333/fff?text=MFF", AboutMe = "Workshop focused on forged metal pieces and fused glass sculptures. Serves the Forged Metals and Ethereal Glassworks categories.",
                    Address1 = "Industrial Avenue, 50", Address2 = "Warehouse 3", City = "Curitiba", State = "PR", Country = "Brazil", ZipCode = "80000-000",
                    PhoneNumber = "41998765432", CommisionRate = 7.5m, Email = "metal.fire@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 3, Name = "Rustic Wood & Leather", PhotoUrl = "https://placehold.co/150x150/666/fff?text=RWL", AboutMe = "Masters in the art of rustic joinery and traditional leather pieces. Sells products for Timbercraft, Heritage Leathers, and Wicker & Weave.",
                    Address1 = "Matriz Square, 22", Address2 = "", City = "Belo Horizonte", State = "MG", Country = "Brazil", ZipCode = "30000-000",
                    PhoneNumber = "31976543210", CommisionRate = 5.0m, Email = "rustic.crafts@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 4, Name = "The Potter's Gem", PhotoUrl = "https://placehold.co/150x150/8B4513/fff?text=TPG", AboutMe = "Dedicated to the art of forming clay and crafting delicate jewelry. Covers the Clay & Glaze and Glimmerstone Jewels categories.",
                    Address1 = "Gold Street, 350", Address2 = "Floor 2", City = "Rio de Janeiro", State = "RJ", Country = "Brazil", ZipCode = "20000-000",
                    PhoneNumber = "21912345678", CommisionRate = 6.0m, Email = "potters.gem@email.com", PasswordHash = MockPasswordHash
                },
                new Seller
                {
                    SellerId = 5, Name = "Felt & Parchment Studio", PhotoUrl = "https://placehold.co/150x150/A0522D/fff?text=FPS", AboutMe = "Specialty studio for high-quality paper, inks, and natural wools. Covers the Paper & Ink and Wool & Whimsey categories.",
                    Address1 = "Paper Avenue, 1500", Address2 = "Room 101", City = "Porto Alegre", State = "RS", Country = "Brazil", ZipCode = "90000-000",
                    PhoneNumber = "51923456789", CommisionRate = 5.0m, Email = "felt.parchment@email.com", PasswordHash = MockPasswordHash
                }
            };
        }

        private static IEnumerable<Client> GetClientSeedData()
    {
        // Seed de Clientes ATUALIZADA com os nomes dos arquivos de imagem e dados detalhados
        return new List<Client>
        {
            new Client 
            { 
                UserId = 1, 
                Name = "Daniel Wilson", 
                Email = "daniel.wilson@email.com", 
                PasswordHash = MockPasswordHash,
                PhoneNumber = "11987654321",
                Address1 = "Rua do Cliente Feliz, 100",
                Address2 = "Apto 101",
                City = "São Paulo",
                State = "SP",
                Country = "Brazil",
                ZipCode = "01000-001"
            },
            new Client 
            { 
                UserId = 2, 
                Name = "Isabella Moore", 
                Email = "isabella.moore@email.com", 
                PasswordHash = MockPasswordHash, 
                PhoneNumber = "21991234567",
                Address1 = "Av. Atlântica, 2000",
                Address2 = "Bloco B, Sala 5",
                City = "Rio de Janeiro",
                State = "RJ",
                Country = "Brazil",
                ZipCode = "22010-020"
            },
            new Client 
            { 
                UserId = 3, 
                Name = "John Smith", 
                Email = "john.smith@email.com", 
                PasswordHash = MockPasswordHash, 
                PhoneNumber = "31987654321",
                Address1 = "Rua da Liberdade, 333",
                Address2 = "Casa",
                City = "Belo Horizonte",
                State = "MG",
                Country = "Brazil",
                ZipCode = "30140-010"
            },
            new Client 
            { 
                UserId = 4, 
                Name = "Olivia Miller", 
                Email = "olivia.miller@email.com", 
                PasswordHash = MockPasswordHash, 
                PhoneNumber = "41999887766",
                Address1 = "Rua das Flores, 500",
                Address2 = "Fundos",
                City = "Curitiba",
                State = "PR",
                Country = "Brazil",
                ZipCode = "80020-000"
            },
            new Client 
            { 
                UserId = 5, 
                Name = "Sophia Davis", 
                Email = "sophia.davis@email.com", 
                PasswordHash = MockPasswordHash, 
                PhoneNumber = "51987659876",
                Address1 = "Av. Principal, 800",
                Address2 = "Sala 12",
                City = "Porto Alegre",
                State = "RS",
                Country = "Brazil",
                ZipCode = "90020-006"
            },
            new Client 
            { 
                UserId = 6, 
                Name = "Emily Johnson", 
                Email = "emily.johnson@email.com", 
                PasswordHash = MockPasswordHash, 
                PhoneNumber = "61988776655",
                Address1 = "Praça dos Três Poderes, 1",
                Address2 = "",
                City = "Brasília",
                State = "DF",
                Country = "Brazil",
                ZipCode = "70100-000"
            },
            new Client 
            { 
                UserId = 7, 
                Name = "James Taylor", 
                Email = "james.taylor@email.com", 
                PasswordHash = MockPasswordHash, 
                PhoneNumber = "81977665544",
                Address1 = "Rua do Sol, 456",
                Address2 = "Ponto de Referência: Próximo à praia",
                City = "Recife",
                State = "PE",
                Country = "Brazil",
                ZipCode = "50000-000"
            },
            new Client 
            { 
                UserId = 8, 
                Name = "Michael Brown", 
                Email = "michael.brown@email.com", 
                PasswordHash = MockPasswordHash, 
                PhoneNumber = "71966554433",
                Address1 = "Avenida da Bahia, 123",
                Address2 = "Edifício Comercial",
                City = "Salvador",
                State = "BA",
                Country = "Brazil",
                ZipCode = "40000-000"
            }
        };
    }
        
        private static List<Product> GetProductSeedData()
        {
            // Price is stored as 'int' (using cents). Ex: 4599 = $45.99
            var products = new List<Product>();
            long currentId = 1;

            // --- EXISTING PRODUCTS (IDs 1 to 38, Category 1 and 2) ---
            products.Add(new Product { ProductId = currentId++, Name = "Premium Acrylic Paint Set (12 Colors)", Description = "High-viscosity acrylic paints ideal for canvas and professional artwork. Non-toxic and fast-drying.", Price = 4599, Inventory = 150.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Thrown Stoneware Bowl (Small)", Description = "Locally sourced stoneware clay, pre-mixed with a forest-green glaze. Perfect for firing at cone 6.", Price = 2250, Inventory = 40.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Glass Fusing Starter Kit - Dichroic", Description = "Complete kit for glass fusing, including safety gloves, cutting tools, and a selection of dichroic glass pieces.", Price = 9800, Inventory = 20.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Wrought Iron Candle Holder (Medieval Style)", Description = "Heavy-duty wrought iron piece, hand-forged using traditional blacksmithing methods. Durable matte black finish.", Price = 7500, Inventory = 15.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Raw Amethyst Geode (Small, 50g)", Description = "Natural Uruguayan Amethyst geode. Ideal for wire wrapping and jewelry making. Each stone is unique.", Price = 3500, Inventory = 75.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Vegetable-Tanned Cowhide (A4 size)", Description = "Premium full-grain leather, naturally tanned using vegetable extracts. Perfect thickness for wallets and small goods.", Price = 12000, Inventory = 30.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Japanese Calligraphy Ink Set (Sumi)", Description = "Traditional Sumi ink and five bamboo pens, housed in a wooden box. Suitable for calligraphy and detailed drawing.", Price = 3990, Inventory = 60.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Carved Oak Chopping Board", Description = "Made from sustainable Oakwood, treated with mineral oil. A rustic, durable board ideal for kitchen use or display.", Price = 5500, Inventory = 25.0m, SellerId = 3, CategoryId = 8 });
            products.Add(new Product { ProductId = currentId++, Name = "Large Woven Bamboo Basket (Storage)", Description = "Hand-woven from natural, flexible bamboo fibers. Perfect for laundry or general home storage.", Price = 8500, Inventory = 35.0m, SellerId = 3, CategoryId = 9 });
            products.Add(new Product { ProductId = currentId++, Name = "Merino Wool Skein (Sky Blue, 100g)", Description = "100% pure Merino wool, super soft and non-irritating. Perfect for baby clothes and sensitive skin projects.", Price = 1500, Inventory = 200.0m, SellerId = 5, CategoryId = 10 });
            products.Add(new Product { ProductId = currentId++, Name = "Beginner Sewing Machine (Portable)", Description = "Compact and lightweight sewing machine with 12 built-in stitches. Includes thread, needles, and foot pedal.", Price = 29999, Inventory = 50.0m, SellerId = 1, CategoryId = 11 });
            products.Add(new Product { ProductId = currentId++, Name = "Abstract Acrylic on Canvas", Description = "Original painting featuring vibrant, thick acrylic layers on a gallery-wrapped canvas. Signed by the artist.", Price = 35000, Inventory = 5.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Abstract Art Digital Print", Description = "High-quality archival giclée print of a digital abstract piece. Available in multiple sizes, ready for framing.", Price = 4500, Inventory = 100.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Acrylic-Primed Stretched Canvas (Large)", Description = "Triple-primed, 100% cotton canvas, perfect for large-scale acrylic and oil painting. Ready to hang.", Price = 7800, Inventory = 80.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Professional Acrylic Paint Set (24 Colors)", Description = "Complete set of 24 artist-grade, heavy-body acrylic paints. High pigment concentration and lightfastness.", Price = 6899, Inventory = 120.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Deluxe Artist Brush Set (20 pcs)", Description = "Assortment of 20 synthetic and natural hair brushes for oil, acrylic, and watercolor techniques. Includes travel case.", Price = 4200, Inventory = 150.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Custom Pet Portrait Commission", Description = "Original, personalized oil painting of your pet on a linen canvas. Requires high-resolution photo reference.", Price = 150000, Inventory = 5.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Graphite Drawing Pencil Set (H-B range)", Description = "Set of 12 professional graphite pencils, ranging from 6H (hard) to 8B (soft), ideal for detailed drawing.", Price = 2150, Inventory = 300.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Master Oil Paint Set (12 Colors, Fine Art)", Description = "A curated selection of 12 premium, pigment-rich oil paints, favored by classical artists. Excellent texture.", Price = 9999, Inventory = 90.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Framed Landscape Canvas Print", Description = "Museum-quality Giclée print of a scenic landscape, professionally framed with anti-glare glass.", Price = 12000, Inventory = 50.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Large Format Institutional Easel", Description = "Heavy-duty, floor-standing H-frame easel made from seasoned beechwood. Suitable for canvases up to 80 inches.", Price = 45000, Inventory = 10.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Original Landscape Oil Painting", Description = "Unique original oil painting capturing a dramatic mountain vista. Varnished and ready to display.", Price = 48000, Inventory = 1.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Mixed Media Paper Pad (A3, 30 sheets)", Description = "Thick, durable, textured paper pad suitable for wet and dry techniques, including watercolor, gouache, and ink.", Price = 2990, Inventory = 250.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Classic Portrait Oil Painting (Commission)", Description = "High-detail, traditional oil portrait commission on a premium canvas. Consultation required for execution.", Price = 95000, Inventory = 3.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Original Oil Painting: Misty Meadow", Description = "A serene, atmospheric original oil work, characterized by soft focus and subtle colors. Unframed.", Price = 52000, Inventory = 1.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Premium Plein Air Sketchbook (A5)", Description = "Pocket-sized sketchbook with perforated pages and a durable cover, perfect for sketching outdoors (plein air).", Price = 1850, Inventory = 400.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Leather-Bound Sketchbook (Handmade)", Description = "Hand-stitched sketchbook with a genuine leather cover and high-quality acid-free drawing paper. A true heirloom piece.", Price = 7500, Inventory = 30.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Custom Watercolor Portrait (Small)", Description = "Personalized watercolor portrait on heavy cotton paper. Ideal for gifts or small displays.", Price = 60000, Inventory = 10.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Watercolor Portrait Print (Limited Edition)", Description = "Signed and numbered limited edition print of a popular watercolor portrait series. Comes with certificate of authenticity.", Price = 8500, Inventory = 40.0m, SellerId = 1, CategoryId = 1 });
            products.Add(new Product { ProductId = currentId++, Name = "Studio H-Frame Wooden Easel", Description = "A solid, adjustable H-frame easel for studio work, offering maximum stability for various canvas sizes.", Price = 15000, Inventory = 20.0m, SellerId = 1, CategoryId = 1 });

            // CLAY & GLAZE PRODUCTS (Category 2, Seller 4)
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Painted Ceramic Coasters (Set of 4)", Description = "Unique set of four ceramic coasters, hand-painted and kiln-fired with a protective glaze. Includes cork backing.", Price = 3999, Inventory = 80.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Tall Geometric Ceramic Vase", Description = "Modern, tall vase with a matte finish and geometric texture, perfect for dried arrangements or minimalist decor.", Price = 7990, Inventory = 30.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Rustic Clay Dinner Plate", Description = "Large, durable dinner plate made from locally sourced clay with a rustic, uneven edge and semi-matte glaze.", Price = 2550, Inventory = 120.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Miniature Fox Figurine Sculpture", Description = "Small, detailed sculpture of a resting fox, finished with a smooth, glossy glaze. Ideal for desks or shelves.", Price = 4900, Inventory = 50.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Large Ocean-Blue Glazed Serving Bowl", Description = "Beautifully hand-glazed ceramic serving bowl in deep ocean blue, ideal for salads, fruit, or as a centerpiece.", Price = 5999, Inventory = 45.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Pottery Mixing Bowl Set (3 Sizes)", Description = "Set of three nesting earthenware mixing bowls (small, medium, large), durable and perfect for baking and prep.", Price = 11500, Inventory = 25.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Japanese Style Sake Cup Set (4 pcs)", Description = "Set of four small, hand-thrown sake cups with a delicate, speckled glaze, perfect for traditional or modern serving.", Price = 4590, Inventory = 60.0m, SellerId = 4, CategoryId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Large Terracotta Storage Jar", Description = "Rustic, unglazed terracotta jar with a tight-fitting cork lid, suitable for dry goods storage or decoration.", Price = 6500, Inventory = 40.0m, SellerId = 4, CategoryId = 2 });

            // ETHEREAL GLASSWORKS PRODUCTS (Category 3, Seller 2)
            products.Add(new Product { ProductId = currentId++, Name = "Blown Glass Ornament (Limited Edition)", Description = "Delicate, hand-blown glass ornament with shimmering iridescence. Perfect for display or holiday decor.", Price = 4500, Inventory = 100.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Elegant Blown Glass Vase (Tall)", Description = "Tall, slender vase created through traditional glass blowing techniques. Features a subtle blue gradient.", Price = 12000, Inventory = 30.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Faceted Crystal Orb Paperweight", Description = "Solid, precision-cut crystal orb that refracts light beautifully. Ideal as a desk accessory or collector's item.", Price = 6500, Inventory = 50.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Blown Glass Bell Sculpture", Description = "A unique glass sculpture shaped like a bell, producing a delicate, resonant tone. Decorative centerpiece.", Price = 9500, Inventory = 25.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Set of 4 Fused Glass Coasters", Description = "Set of four durable glass coasters created by fusing colored glass sheets. Includes small rubber feet for protection.", Price = 5500, Inventory = 70.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Engraved Crystal Glass Decanter", Description = "Premium, heavy-bottomed crystal decanter with detailed diamond-cut engraving. Suitable for whiskey or spirits.", Price = 18000, Inventory = 20.0m, CategoryId = 3, SellerId = 2 });
            products.Add(new Product { ProductId = currentId++, Name = "Artisan Glass Paperweight (Swirl)", Description = "Circular glass paperweight featuring an intricate, colorful internal swirl design. Signed by the artisan.", Price = 3200, Inventory = 80.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Deluxe Glass Tumbler Set (6 Pcs)", Description = "Set of six high-quality glass tumblers with a weighted base. Perfect for everyday use or entertaining.", Price = 7500, Inventory = 40.0m, SellerId = 2, CategoryId = 3 });
            products.Add(new Product { ProductId = currentId++, Name = "Custom Stained Glass Panel (Art Deco)", Description = "A commission for a unique Art Deco-style stained glass window panel. Price is an estimate; final cost depends on size/complexity.", Price = 450000, Inventory = 5.0m, SellerId = 2, CategoryId = 3 });

            // FORGED METALS PRODUCTS (Category 4, Seller 2)
            products.Add(new Product { ProductId = currentId++, Name = "Decorative Steel Hook (Wall Mount)", Description = "Hand-forged steel wall hook with a decorative scroll design. Ideal for coats or tools. Matte black finish.", Price = 2200, Inventory = 150.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Forged Metal Bottle Rack (6 Bottles)", Description = "Wrought iron wine rack, holds six standard bottles securely. Rustic patina finish.", Price = 7900, Inventory = 35.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Forged Fire Poker (Heavy Duty)", Description = "Solid steel fire poker, designed for heavy use in fireplaces or wood stoves. Ergonomic handle.", Price = 6500, Inventory = 20.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Forged Utility Hook (Rustic)", Description = "Simple, durable iron utility hook. Excellent for kitchens or garages where a rustic look is desired.", Price = 1800, Inventory = 200.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Forged Abstract Sculpture (Desk Size)", Description = "Small abstract sculpture made from twisted, polished steel bars. Unique art piece for a modern desk.", Price = 15000, Inventory = 10.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Custom Iron Grill Insert (Heavy Gauge)", Description = "Made-to-order heavy-gauge iron grill or grate insert. Requires dimensions. High heat resistance.", Price = 35000, Inventory = 10.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Abstract Metal Wall Art Piece (Small)", Description = "Geometric wall hanging created from welded metal scraps. Finished with a protective clear coat.", Price = 9800, Inventory = 15.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Steel Bottle Opener (Keyring Style)", Description = "Compact and durable bottle opener made from brushed stainless steel. Ideal for keychains.", Price = 1250, Inventory = 300.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Wrought Iron Candlestick (Set of 2)", Description = "Pair of elegant wrought iron candlesticks, hand-twisted design. Suitable for taper candles.", Price = 4999, Inventory = 70.0m, SellerId = 2, CategoryId = 4 });
            products.Add(new Product { ProductId = currentId++, Name = "Wrought Iron Coasters (Set of 4)", Description = "Set of four iron coasters with a decorative base and felt pads to protect surfaces. Matte black finish.", Price = 5500, Inventory = 80.0m, SellerId = 2, CategoryId = 4 });
            
            // GLIMMERSTONE JEWELS PRODUCTS (Category 5, Seller 4)
            products.Add(new Product { ProductId = currentId++, Name = "Elegant Amethyst Earrings (Sterling Silver)", Description = "Delicate drop earrings featuring polished amethyst stones set in hypoallergenic sterling silver.", Price = 18990, Inventory = 85.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Colorful Glass Beaded Necklace (Long)", Description = "Hand-strung long necklace with vibrant mixed glass beads and a simple brass clasp. Versatile piece.", Price = 14500, Inventory = 110.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Sparkling Crystal Stud Earrings (Small)", Description = "Classic stud earrings featuring premium faceted crystals for maximum brilliance and daily wear.", Price = 15990, Inventory = 150.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Wide Polished Cuff Bracelet (Brass)", Description = "Statement cuff bracelet, hand-polished brass with a comfortable, adjustable fit. Modern design.", Price = 29900, Inventory = 40.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Delicate Gold Plated Chain Necklace", Description = "Thin and delicate gold-plated chain, perfect for layering or wearing alone. Durable and tarnish-resistant.", Price = 49990, Inventory = 180.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Deep Red Garnet Pendant (Silver)", Description = "Sterling silver pendant featuring a deep red, oval-cut natural garnet stone. Includes a simple chain.", Price = 21000, Inventory = 60.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Multi-Gemstone Charm Bracelet", Description = "Bracelet adorned with various small tumbled gemstones (quartz, jade, rose quartz). Adjustable closure.", Price = 34950, Inventory = 75.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Vintage Freshwater Pearl Brooch", Description = "Hand-assembled brooch featuring genuine freshwater pearls in a vintage floral arrangement. Pin closure.", Price = 17550, Inventory = 30.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Zodiac Sign Pendant Charm (Custom)", Description = "Personalized pendant charm with the customer's zodiac sign engraved. Gold-plated option available.", Price = 11990, Inventory = 95.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Minimalist Sterling Silver Ring", Description = "Simple, polished sterling silver band ring. Unisex design, perfect for stacking or as a promise ring.", Price = 9500, Inventory = 200.0m, SellerId = 4, CategoryId = 5 });
            products.Add(new Product { ProductId = currentId++, Name = "Bohemian Turquoise Inlaid Ring", Description = "Sterling silver ring featuring natural turquoise stone inlay in a bohemian, detailed setting.", Price = 19900, Inventory = 50.0m, SellerId = 4, CategoryId = 5 });

            // HERITAGE LEATHERS PRODUCTS (Category 6, Seller 3)
            products.Add(new Product { ProductId = currentId++, Name = "Rustic Leather Journal Cover (A5)", Description = "Hand-stitched leather cover for A5 journals, featuring a wrap-around tie closure. Natural patina finish.", Price = 16500, Inventory = 90.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Customizable Leather Key Fob", Description = "Small, durable leather strap with a polished metal ring. Customizable with up to three initials.", Price = 3500, Inventory = 250.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Full-Grain Leather Belt (Classic Brown)", Description = "Heavy-duty, full-grain leather belt with a solid brass buckle. Designed to last a lifetime.", Price = 22000, Inventory = 110.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Slim Leather Card Holder (Minimalist)", Description = "Minimalist card holder for 4-6 cards, made from slim, smooth leather. Ideal for front pocket carry.", Price = 8990, Inventory = 180.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Leather Cord Organizer (Set of 3)", Description = "Set of three small leather snaps for organizing electronic cables and wires. Keeps desk tidy.", Price = 4500, Inventory = 300.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Leather Bound Journal Cover (Distressed)", Description = "Journal cover made from distressed leather, giving it a vintage, rugged look. Includes an elastic pen loop.", Price = 17990, Inventory = 70.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Leather Laptop Sleeve (13 inch)", Description = "Protective and stylish leather sleeve for 13-inch laptops, lined with soft suede. Simple snap closure.", Price = 35000, Inventory = 50.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Travel Leather Passport Holder", Description = "Classic passport holder with slots for boarding passes and credit cards. Engravable initials option.", Price = 14500, Inventory = 100.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Bifold Leather Wallet (Classic Design)", Description = "Traditional bifold wallet with multiple card slots and a dedicated cash compartment. Smooth, durable leather.", Price = 19990, Inventory = 130.0m, SellerId = 3, CategoryId = 6 });
            products.Add(new Product { ProductId = currentId++, Name = "Large Leather Satchel Bag (Cross-body)", Description = "Full-sized leather satchel with adjustable cross-body strap. Ideal for daily commute or travel. Features multiple compartments.", Price = 79900, Inventory = 20.0m, SellerId = 3, CategoryId = 6 });

            // ---------------------------------------------------------------------
            // PAPER & INK PRODUCTS (Category 7, Seller 5)
            // ---------------------------------------------------------------------
            products.Add(new Product { ProductId = currentId++, Name = "Artistic Hand-Painted Note Cards (Set of 10)", Description = "Set of 10 heavyweight, textured note cards with original watercolor designs. Matching envelopes included.", Price = 4990, Inventory = 150.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Traditional Calligraphy Starter Set", Description = "Set includes a fine wooden pen, three interchangeable nibs, and a bottle of rich black India ink. Perfect for beginners.", Price = 6500, Inventory = 100.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Bespoke Custom Stationery - Letterhead", Description = "Custom design and printing of personalized letterheads on premium linen paper. Min. order: 50 sheets.", Price = 9999, Inventory = 100.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Deluxe Custom Stationery Set (Monogrammed)", Description = "Monogrammed stationery set including 50 letterheads and 50 envelopes, printed on thick cotton paper.", Price = 18000, Inventory = 80.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Premium Custom Stationery Set (Professional)", Description = "Professional set including letterhead, business cards, and envelopes with integrated custom branding. Design consultation required.", Price = 25000, Inventory = 50.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Bound Linen Journal (Lined)", Description = "A5 journal with a stitched linen spine and archival, acid-free lined paper. Ideal for daily writing or sketching.", Price = 12990, Inventory = 120.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Handmade Paper Envelopes (Mixed Colors)", Description = "Set of 20 unique envelopes made from recycled cotton fibers. Each envelope has a slight variation in color and texture.", Price = 3500, Inventory = 200.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Fountain Pen and Ink Set (Beginner's)", Description = "Smooth-writing fountain pen with fine nib, five ink cartridges, and a converter for bottled ink. Elegant gift box.", Price = 7990, Inventory = 95.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Limited Edition Letterpress Prints (A4)", Description = "Signed and numbered A4 art prints, created using traditional letterpress techniques. Deep, tactile impression.", Price = 5990, Inventory = 70.0m, SellerId = 5, CategoryId = 7 });
            products.Add(new Product { ProductId = currentId++, Name = "Personalized Brass Wax Seal Kit", Description = "Custom-engraved solid brass seal with a wooden handle, three sticks of sealing wax, and a melting spoon. Perfect for elegant correspondence.", Price = 9500, Inventory = 60.0m, SellerId = 5, CategoryId = 7 });

            // ---------------------------------------------------------------------
            // STITCH & THREAD PRODUCTS (Category 11, Seller 1)
            // ---------------------------------------------------------------------
            products.Add(new Product { ProductId = currentId++, Name = "Custom Embroidered Canvas Tote Bag", Description = "Durable canvas tote bag featuring custom machine or hand embroidery (up to 10 characters). Perfect for groceries or books.", Price = 8990, Inventory = 75.0m, SellerId = 1, CategoryId = 11 }); // 89
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Stitched Abstract Tapestry (Medium)", Description = "Medium-sized wall tapestry, intricately stitched with mixed threads and textures. Ready to hang.", Price = 24990, Inventory = 15.0m, SellerId = 1, CategoryId = 11 }); // 90
            products.Add(new Product { ProductId = currentId++, Name = "Handmade Quilted Throw Blanket (Cotton)", Description = "Warm and soft quilted blanket made from 100% pre-washed cotton fabric. Unique pattern and design.", Price = 35000, Inventory = 20.0m, SellerId = 1, CategoryId = 11 }); // 91
            products.Add(new Product { ProductId = currentId++, Name = "Woven Embroidered Iron-On Patch (Custom)", Description = "Small, durable patch with custom woven or embroidered design. Iron-on backing for easy application.", Price = 1990, Inventory = 250.0m, SellerId = 1, CategoryId = 11 }); // 92
            products.Add(new Product { ProductId = currentId++, Name = "Handmade Fabric Coasters (Set of 6)", Description = "Set of six reversible fabric coasters, made with high-quality, absorbent cotton. Machine washable.", Price = 4500, Inventory = 100.0m, SellerId = 1, CategoryId = 11 }); // 93
            products.Add(new Product { ProductId = currentId++, Name = "Silk Ribbon Embroidery Art (Framed)", Description = "Small framed artwork created using delicate silk ribbon embroidery techniques, featuring floral motifs.", Price = 15000, Inventory = 30.0m, SellerId = 1, CategoryId = 11 }); // 94
            products.Add(new Product { ProductId = currentId++, Name = "Embroidered Wall Hoop Art (Botanical)", Description = "Modern wall decoration featuring botanical hand embroidery stretched within a wooden hoop frame (20cm).", Price = 7500, Inventory = 50.0m, SellerId = 1, CategoryId = 11 }); // 95
            // Institutional image ignored (no product entry).
            products.Add(new Product { ProductId = currentId++, Name = "Abstract Textile Wall Art (Woven/Mixed Media)", Description = "Unique wall piece combining weaving, stitching, and mixed media fabrics for a textured, abstract look.", Price = 19990, Inventory = 10.0m, SellerId = 1, CategoryId = 11 }); // 96
            products.Add(new Product { ProductId = currentId++, Name = "Delicate Floral Embroidered Hoop (Large)", Description = "Large wooden hoop (30cm) featuring a highly detailed, delicate floral embroidery design. Perfect centerpiece.", Price = 12000, Inventory = 40.0m, SellerId = 1, CategoryId = 11 }); // 97
            products.Add(new Product { ProductId = currentId++, Name = "Patterned Throw Cushion (Hand-Sewn)", Description = "Square throw cushion (45x45cm) with a unique patterned fabric cover, hand-sewn with an invisible zipper closure. Pillow insert included.", Price = 8500, Inventory = 60.0m, SellerId = 1, CategoryId = 11 }); // 98
            
            // ---------------------------------------------------------------------
            // TIMBERCRAFT PRODUCTS (Category 8, Seller 3)
            // ---------------------------------------------------------------------
            products.Add(new Product { ProductId = currentId++, Name = "Abstract Hardwood Sculpture", Description = "An organic, modern art piece carved from hardwood. Ideal for contemporary decor.", Price = 18990, Inventory = 15.0m, SellerId = 3, CategoryId = 8 }); // 99
            products.Add(new Product { ProductId = currentId++, Name = "Carved Wooden Figurative Statue", Description = "Detailed wooden statue, hand-carved with classic motifs. A collector's art piece.", Price = 24900, Inventory = 10.0m, SellerId = 3, CategoryId = 8 }); // 100
            products.Add(new Product { ProductId = currentId++, Name = "Wooden Coasters Set (4 pieces)", Description = "Elegant coasters with a smooth, moisture-resistant finish. Adds rustic-chic style to the table.", Price = 4500, Inventory = 120.0m, SellerId = 3, CategoryId = 8 }); // 101
            products.Add(new Product { ProductId = currentId++, Name = "Decorative Wooden Keepsake Box", Description = "Hinged and clasped box, ideal for storing jewelry or small treasures. Design highlights the natural wood grain.", Price = 7850, Inventory = 70.0m, SellerId = 3, CategoryId = 8 }); // 102
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Carved Wooden Serving Spoon", Description = "Unique kitchen utensil, carved with attention to detail. Perfect for serving.", Price = 3450, Inventory = 150.0m, SellerId = 3, CategoryId = 8 }); // 103
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Carved Wooden Whistle", Description = "Functional and meticulously carved whistle. Great for collectors or as a handmade toy.", Price = 2299, Inventory = 200.0m, SellerId = 3, CategoryId = 8 }); // 104
            products.Add(new Product { ProductId = currentId++, Name = "Wood Turned Salt Cellar", Description = "Charming tabletop salt cellar with a lid, precisely turned for gourmet salt.", Price = 4999, Inventory = 80.0m, SellerId = 3, CategoryId = 8 }); // 105
            products.Add(new Product { ProductId = currentId++, Name = "Large Natural Wooden Serving Bowl", Description = "Beautiful turned bowl for salads, fruits, or as a centerpiece. Hand-polished to highlight the grain.", Price = 6790, Inventory = 60.0m, SellerId = 3, CategoryId = 8 }); // 106
            products.Add(new Product { ProductId = currentId++, Name = "Artisan Block Wooden Cutting Board", Description = "Robust and durable board, made from glued wood blocks. Perfect for preparation or presentation.", Price = 8990, Inventory = 90.0m, SellerId = 3, CategoryId = 8 }); // 107
            products.Add(new Product { ProductId = currentId++, Name = "Rustic Wooden Serving Board for Cheeses and Snacks", Description = "Board with an organic shape and handle, ideal for arranging and serving charcuterie and appetizers.", Price = 9500, Inventory = 75.0m, SellerId = 3, CategoryId = 8 }); // 108
            products.Add(new Product { ProductId = currentId++, Name = "Elegant Wooden Serving Tray", Description = "Lightweight tray with raised edges and recessed handles, ideal for breakfast or table organization.", Price = 11200, Inventory = 50.0m, SellerId = 3, CategoryId = 8 }); // 109

            // ---------------------------------------------------------------------
            // WICKER & WEAVE PRODUCTS (Category 9, Seller 3)
            // ---------------------------------------------------------------------
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Woven Bamboo Fruit Bowl", Description = "Large, shallow bowl woven from natural bamboo strips. Lightweight and perfect for fruit or bread storage.", Price = 4990, Inventory = 110.0m, SellerId = 3, CategoryId = 9 }); // 110
            products.Add(new Product { ProductId = currentId++, Name = "Round Rattan Decorative Tray (Small)", Description = "Flat, tightly woven rattan tray, ideal as a centerpiece on a coffee table or for serving small items.", Price = 3500, Inventory = 150.0m, SellerId = 3, CategoryId = 9 }); // 111
            products.Add(new Product { ProductId = currentId++, Name = "Large Rattan Storage Basket with Lid", Description = "Durable, cylindrical storage basket woven from thick rattan, complete with a fitted lid for organized storage.", Price = 12000, Inventory = 40.0m, SellerId = 3, CategoryId = 9 }); // 112
            products.Add(new Product { ProductId = currentId++, Name = "Set of 6 Rattan Coasters", Description = "Set of six small, round coasters woven from fine rattan fibers, providing a natural, protective surface.", Price = 2500, Inventory = 200.0m, SellerId = 3, CategoryId = 9 }); // 113
            products.Add(new Product { ProductId = currentId++, Name = "Modern Rattan Table Lamp Base", Description = "Mid-century style lamp base woven from open-weave rattan. Provides warm, textured lighting (shade and bulb not included).", Price = 9500, Inventory = 30.0m, SellerId = 3, CategoryId = 9 }); // 114
            products.Add(new Product { ProductId = currentId++, Name = "Woven Rope Plant Pot Cover", Description = "Soft, hand-braided jute rope cover for standard plant pots. Adds a nautical, bohemian touch to indoor plants.", Price = 3990, Inventory = 90.0m, SellerId = 3, CategoryId = 9 }); // 115
            products.Add(new Product { ProductId = currentId++, Name = "Round Hand-Braided Seagrass Mat (60cm)", Description = "Large circular mat hand-braided from natural seagrass. Perfect as a floor mat or textured wall hanging.", Price = 5500, Inventory = 70.0m, SellerId = 3, CategoryId = 9 }); // 116
            products.Add(new Product { ProductId = currentId++, Name = "Classic Wicker Laundry Basket", Description = "Extra-large, traditional wicker basket with handles. Ideal for laundry or general bulk storage.", Price = 11000, Inventory = 35.0m, SellerId = 3, CategoryId = 9 }); // 117
            products.Add(new Product { ProductId = currentId++, Name = "Geometric Wicker Pendant Lamp Shade", Description = "Geometric-shaped lampshade woven from natural wicker, creating beautiful light patterns (wiring not included).", Price = 7500, Inventory = 50.0m, SellerId = 3, CategoryId = 9 }); // 118
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Woven Rattan Accent Chair", Description = "A fully woven rattan accent chair with a high back and comfortable seat cushion (included). Durable and stylish.", Price = 45000, Inventory = 10.0m, SellerId = 3, CategoryId = 9 }); // 119
            products.Add(new Product { ProductId = currentId++, Name = "Woven Seagrass Storage Box (Rectangular)", Description = "Rectangular box woven from thick seagrass with integrated handles. Suitable for shelf storage.", Price = 6990, Inventory = 60.0m, SellerId = 3, CategoryId = 9 }); // 120
            products.Add(new Product { ProductId = currentId++, Name = "Abstract Woven Wall Decor Piece", Description = "Unique circular wall decoration featuring abstract patterns woven from mixed natural fibers (jute, rattan, cotton).", Price = 8900, Inventory = 45.0m, SellerId = 3, CategoryId = 9 }); // 121
            products.Add(new Product { ProductId = currentId++, Name = "Bohemian Woven Wall Hanging (Fringed)", Description = "Large wall tapestry featuring intricate knotting and weaving techniques, finished with long cotton fringes.", Price = 15000, Inventory = 20.0m, SellerId = 3, CategoryId = 9 }); // 122

            // ---------------------------------------------------------------------
            // NEW PRODUCTS - WOOL & WHIMSEY (Category 10, Seller 5)
            // ---------------------------------------------------------------------
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Knit Chunky Throw Blanket (Merino)", Description = "Luxurious, oversized throw blanket hand-knit with super-thick Merino wool. Perfect for cozying up on the sofa.", Price = 39990, Inventory = 25.0m, SellerId = 5, CategoryId = 10 }); // 123
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Felted Wool Slippers (Indoor)", Description = "Soft, warm, and durable slippers made from needle-felted wool, featuring a non-slip suede sole. Unique colors.", Price = 8990, Inventory = 120.0m, SellerId = 5, CategoryId = 10 }); // 124
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Knit Fingerless Wool Gloves", Description = "Thick, warm, and stylish fingerless gloves knitted from durable natural wool. Ideal for outdoor activities or typing.", Price = 3500, Inventory = 150.0m, SellerId = 5, CategoryId = 10 }); // 125
            products.Add(new Product { ProductId = currentId++, Name = "Long Hand-Knitted Scarf (Alpaca Blend)", Description = "Extra-long and soft scarf, knitted from a premium Alpaca and wool blend. Provides excellent warmth without bulk.", Price = 7500, Inventory = 90.0m, SellerId = 5, CategoryId = 10 }); // 126
            products.Add(new Product { ProductId = currentId++, Name = "Whimsical Knitted Dog Sweater (Various Sizes)", Description = "Cute and cozy hand-knitted sweater for small to medium dogs, made from hypoallergenic yarn. Different colors available.", Price = 4990, Inventory = 80.0m, SellerId = 5, CategoryId = 10 }); // 127
            products.Add(new Product { ProductId = currentId++, Name = "Classic Knitted Wool Scarf (Plaid)", Description = "Traditional knitted scarf featuring a classic plaid pattern. Perfect for everyday winter wear.", Price = 5990, Inventory = 110.0m, SellerId = 5, CategoryId = 10 }); // 128
            products.Add(new Product { ProductId = currentId++, Name = "Handmade Oversized Knitted Sweater (Unisex)", Description = "Warm, comfortable, oversized sweater, hand-knitted with a cable pattern. Relaxed fit.", Price = 14990, Inventory = 50.0m, SellerId = 5, CategoryId = 10 }); // 129
            products.Add(new Product { ProductId = currentId++, Name = "Soft Knit Wool Beanie Hat (Slouchy Fit)", Description = "Warm beanie hat, tightly knitted from soft wool, with a slouchy, comfortable fit. Available in dark colors.", Price = 3990, Inventory = 200.0m, SellerId = 5, CategoryId = 10 }); // 130
            products.Add(new Product { ProductId = currentId++, Name = "Pure Wool Throw Blanket (Geometric Pattern)", Description = "Medium-weight blanket woven from pure wool, featuring a subtle geometric pattern. Durable and naturally insulating.", Price = 18000, Inventory = 40.0m, SellerId = 5, CategoryId = 10 }); // 131
            products.Add(new Product { ProductId = currentId++, Name = "Hand-Knit Wool Headband/Ear Warmer", Description = "Wide, hand-knitted headband designed to keep ears warm. Ideal for running or outdoor activities in cold weather.", Price = 2500, Inventory = 180.0m, SellerId = 5, CategoryId = 10 }); // 132
            products.Add(new Product { ProductId = currentId++, Name = "Thick Woolen Bed Socks (Crew Length)", Description = "Super-thick, soft woolen socks designed for sleeping or lounging. Provides maximum warmth.", Price = 1990, Inventory = 250.0m, SellerId = 5, CategoryId = 10 }); // 133
            
            return products;
        }

        private static List<ProductImage> GetProductImageSeedData()
        {
            var products = GetProductSeedData();
            var images = new List<ProductImage>();
            long imageIdCounter = 1;

            // Mapping of products with specific file names (ID 39 onwards)
            var specificProductsToImageFile = products
                .Where(p => NewProductImagesMap.ContainsKey(p.ProductId))
                .ToDictionary(p => p.ProductId, p => NewProductImagesMap[p.ProductId]);


            foreach (var product in products)
            {
                long sellerId = product.SellerId;
                long categoryId = product.CategoryId;

                // --- SPECIAL LOGIC FOR PRODUCTS WITH SPECIFIC IMAGES (IDs 39 to 133) ---
                if (product.ProductId >= 39)
                {
                    if (specificProductsToImageFile.TryGetValue(product.ProductId, out var filename))
                    {
                        // Use the provided file name for the URL
                        string productUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{filename}";
                        
                        images.Add(new ProductImage
                        {
                            ProductImageId = imageIdCounter++,
                            ProductId = product.ProductId,
                            Url = productUrl,
                            Alt = $"Main image for artisan product: {product.Name}"
                        });

                        // For categories 6 (Leather), 7 (Paper), 8 (Wood), 9 (Wicker), 10 (Wool) and 11 (Textile), add 1 generic detail image.
                        if (product.CategoryId == 6 || product.CategoryId == 7 || product.CategoryId == 8 || product.CategoryId == 9 || product.CategoryId == 10 || product.CategoryId == 11)
                        {
                            string detailAlt = $"Detail 1 of {product.Name}.";
                            string detailUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{product.ProductId}_detail_1.jpg";

                            images.Add(new ProductImage
                            {
                                ProductImageId = imageIdCounter++,
                                ProductId = product.ProductId,
                                Url = detailUrl,
                                Alt = detailAlt
                            });
                        }
                    }
                    continue; 
                }
                
                // --- EXISTING LOGIC FOR OLDER PRODUCTS (IDs 1 to 38) ---
                
                // 1. Main Image
                string mainAlt = $"Main image of {product.Name}.";
                // The main image uses the ProductId and '_main.jpg'
                string mainUrl = $"{MockImageUrlBase}{BaseImagePath}/{sellerId}/{categoryId}/{product.ProductId}_main.jpg";

                images.Add(new ProductImage
                {
                    ProductImageId = imageIdCounter++,
                    ProductId = product.ProductId,
                    Url = mainUrl,
                    Alt = mainAlt
                });

                // 2. Detail Images (Simulate 1 to 2 detail images)
                int detailCount;
                if (product.ProductId >= 12 && product.ProductId % 2 == 0)
                {
                    detailCount = 2; 
                }
                else
                {
                    detailCount = 1;
                }
                
                for (int i = 1; i <= detailCount; i++)
                {
                    string detailAlt = $"Detail {i} of {product.Name}.";
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