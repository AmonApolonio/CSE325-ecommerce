using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class AddEcommerceSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            // =================================================================
            // 1. CRIAÇÃO FORÇADA DOS TIPOS ENUM (Correção para o erro 42704)
            // =================================================================
            // O Npgsql/EF Core nem sempre cria os ENUMs a tempo.
            // Executamos os comandos SQL DDL (Data Definition Language) explicitamente
            // para garantir que os tipos existam antes de serem referenciados nas tabelas.

            migrationBuilder.Sql(@$"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'cart_status') THEN
                        CREATE TYPE cart_status AS ENUM ('active', 'abandoned', 'converted', 'expired');
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'order_status') THEN
                        CREATE TYPE order_status AS ENUM ('pending_payment', 'payment_confirmed', 'payment_failed', 'cancelled', 'processing', 'shipped', 'out_for_delivery', 'delivered', 'completed', 'returned');
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'payment_status') THEN
                        CREATE TYPE payment_status AS ENUM ('pending', 'approved', 'failed', 'refunded', 'captured');
                    END IF;
                END
                $$ LANGUAGE plpgsql;");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:cart_status.public", "active,abandoned,converted,expired")
                .Annotation("Npgsql:Enum:order_status.public", "pending_payment,payment_confirmed,payment_failed,cancelled,processing,shipped,out_for_delivery,delivered,completed,returned")
                .Annotation("Npgsql:Enum:payment_status.public", "pending,approved,failed,refunded,captured");

            migrationBuilder.Sql("DROP TABLE IF EXISTS public.product_images CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS public.product CASCADE;");
            
            // Drop tabelas core
            migrationBuilder.Sql("DROP TABLE IF EXISTS public.category CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS public.client CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS public.currency CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS public.seller CASCADE;");


            migrationBuilder.CreateTable(
                name: "category",
                schema: "public",
                columns: table => new
                {
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    address1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    address2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    zip_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                schema: "public",
                columns: table => new
                {
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    symbol = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    exchange_rate_to_brl = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencies", x => x.currency_code);
                });

            migrationBuilder.CreateTable(
                name: "sellers",
                schema: "public",
                columns: table => new
                {
                    seller_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    photo_url = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    about_me = table.Column<string>(type: "text", nullable: true),
                    address1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    address2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    zip_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    commision_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellers", x => x.seller_id);
                });

            migrationBuilder.CreateTable(
                name: "cart",
                schema: "public",
                columns: table => new
                {
                    cart_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<long>(type: "bigint", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "public.cart_status", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_cart_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "clients",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "public",
                columns: table => new
                {
                    order_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<long>(type: "bigint", nullable: false),
                    sub_total_cents = table.Column<decimal>(type: "numeric", nullable: false),
                    tax_cents = table.Column<decimal>(type: "numeric", nullable: false),
                    freight_cents = table.Column<decimal>(type: "numeric", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "public.order_status", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_orders_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "clients",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "public",
                        principalTable: "currencies",
                        principalColumn: "currency_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "public",
                columns: table => new
                {
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<int>(type: "integer", nullable: false),
                    inventory = table.Column<decimal>(type: "numeric", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    seller_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_products_category_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_sellers_seller_id",
                        column: x => x.seller_id,
                        principalSchema: "public",
                        principalTable: "sellers",
                        principalColumn: "seller_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "public",
                columns: table => new
                {
                    payment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transaction_status = table.Column<int>(type: "public.payment_status", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_payments_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "public",
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart_items",
                schema: "public",
                columns: table => new
                {
                    cart_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    cart_item = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_items", x => new { x.cart_id, x.product_id });
                    table.ForeignKey(
                        name: "FK_cart_items_cart_cart_id",
                        column: x => x.cart_id,
                        principalSchema: "public",
                        principalTable: "cart",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cart_items_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "public",
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders_products",
                schema: "public",
                columns: table => new
                {
                    orders_order_id = table.Column<long>(type: "bigint", nullable: false),
                    products_product_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders_products", x => new { x.orders_order_id, x.products_product_id });
                    table.ForeignKey(
                        name: "FK_orders_products_orders_orders_order_id",
                        column: x => x.orders_order_id,
                        principalSchema: "public",
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_products_products_products_product_id",
                        column: x => x.products_product_id,
                        principalSchema: "public",
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_image",
                schema: "public",
                columns: table => new
                {
                    product_image_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    url = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    alt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_image", x => x.product_image_id);
                    table.ForeignKey(
                        name: "FK_product_image_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "public",
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "category",
                columns: new[] { "category_id", "description", "name" },
                values: new object[,]
                {
                    { 1L, "Materials for canvas painting, easels, and fine arts supplies.", "Canvas & Brush" },
                    { 2L, "Specialty clays, earthenware, modeling tools, and glazes for ceramics.", "Clay & Glaze" },
                    { 3L, "Kits and materials for glass blowing, fusing, and creating glass mosaics.", "Ethereal Glassworks" },
                    { 4L, "Tools and metals for forging, welding, and creating decorative metal pieces.", "Forged Metals" },
                    { 5L, "Raw stones, beads, wires, and clasps for assembling handmade jewelry.", "Glimmerstone Jewels" },
                    { 6L, "Leathers of different textures, stamps, dyes, and needles for leatherwork.", "Heritage Leathers" },
                    { 7L, "Rice papers, India inks, pens, and materials for calligraphy and bookbinding.", "Paper & Ink" },
                    { 8L, "Selected woods, lathes, and precision tools for woodworking and carving.", "Timbercraft" },
                    { 9L, "Wicker, bamboo, and other natural fibers for basketry, braiding, and weaving.", "Wicker & Weave" },
                    { 10L, "Natural wools, exotic yarns, and accessories for knitting, crochet, and needle felting.", "Wool & Whimsey" },
                    { 11L, "Various fabrics, embroidery threads, needles, and machines for sewing and quilting.", "Stitch & Thread" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "clients",
                columns: new[] { "user_id", "address1", "address2", "city", "country", "email", "name", "password_hash", "phone_number", "state", "zip_code" },
                values: new object[,]
                {
                    { 1L, "Rua do Cliente Feliz, 100", "Apto 101", "São Paulo", "Brazil", "daniel.wilson@email.com", "Daniel Wilson", "MOCK_HASH_FOR_SEEDING_12345", "11987654321", "SP", "01000-001" },
                    { 2L, "Av. Atlântica, 2000", "Bloco B, Sala 5", "Rio de Janeiro", "Brazil", "isabella.moore@email.com", "Isabella Moore", "MOCK_HASH_FOR_SEEDING_12345", "21991234567", "RJ", "22010-020" },
                    { 3L, "Rua da Liberdade, 333", "Casa", "Belo Horizonte", "Brazil", "john.smith@email.com", "John Smith", "MOCK_HASH_FOR_SEEDING_12345", "31987654321", "MG", "30140-010" },
                    { 4L, "Rua das Flores, 500", "Fundos", "Curitiba", "Brazil", "olivia.miller@email.com", "Olivia Miller", "MOCK_HASH_FOR_SEEDING_12345", "41999887766", "PR", "80020-000" },
                    { 5L, "Av. Principal, 800", "Sala 12", "Porto Alegre", "Brazil", "sophia.davis@email.com", "Sophia Davis", "MOCK_HASH_FOR_SEEDING_12345", "51987659876", "RS", "90020-006" },
                    { 6L, "Praça dos Três Poderes, 1", "", "Brasília", "Brazil", "emily.johnson@email.com", "Emily Johnson", "MOCK_HASH_FOR_SEEDING_12345", "61988776655", "DF", "70100-000" },
                    { 7L, "Rua do Sol, 456", "Ponto de Referência: Próximo à praia", "Recife", "Brazil", "james.taylor@email.com", "James Taylor", "MOCK_HASH_FOR_SEEDING_12345", "81977665544", "PE", "50000-000" },
                    { 8L, "Avenida da Bahia, 123", "Edifício Comercial", "Salvador", "Brazil", "michael.brown@email.com", "Michael Brown", "MOCK_HASH_FOR_SEEDING_12345", "71966554433", "BA", "40000-000" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "currencies",
                columns: new[] { "currency_code", "exchange_rate_to_brl", "name", "symbol" },
                values: new object[,]
                {
                    { "BRL", 1.00m, "Brazilian Real", "R$" },
                    { "EUR", 5.50m, "Euro", "€" },
                    { "USD", 5.00m, "United States Dollar", "$" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "sellers",
                columns: new[] { "seller_id", "about_me", "address1", "address2", "city", "commision_rate", "country", "email", "name", "password_hash", "phone_number", "photo_url", "state", "zip_code" },
                values: new object[,]
                {
                    { 1L, "Specialist in painting and fine embroidery, focusing on sustainable materials. Offers materials for the Canvas & Brush and Stitch & Thread categories.", "Art Street, 100", "Block A", "São Paulo", 5.0m, "Brazil", "clara.artesanato@email.com", "Clara's Canvas Corner", "MOCK_HASH_FOR_SEEDING_12345", "11987654321", "https://placehold.co/150x150/000/fff?text=CC", "SP", "01000-000" },
                    { 2L, "Workshop focused on forged metal pieces and fused glass sculptures. Serves the Forged Metals and Ethereal Glassworks categories.", "Industrial Avenue, 50", "Warehouse 3", "Curitiba", 7.5m, "Brazil", "metal.fire@email.com", "Metal & Fire Forge", "MOCK_HASH_FOR_SEEDING_12345", "41998765432", "https://placehold.co/150x150/333/fff?text=MFF", "PR", "80000-000" },
                    { 3L, "Masters in the art of rustic joinery and traditional leather pieces. Sells products for Timbercraft, Heritage Leathers, and Wicker & Weave.", "Matriz Square, 22", "", "Belo Horizonte", 5.0m, "Brazil", "rustic.crafts@email.com", "Rustic Wood & Leather", "MOCK_HASH_FOR_SEEDING_12345", "31976543210", "https://placehold.co/150x150/666/fff?text=RWL", "MG", "30000-000" },
                    { 4L, "Dedicated to the art of forming clay and crafting delicate jewelry. Covers the Clay & Glaze and Glimmerstone Jewels categories.", "Gold Street, 350", "Floor 2", "Rio de Janeiro", 6.0m, "Brazil", "potters.gem@email.com", "The Potter's Gem", "MOCK_HASH_FOR_SEEDING_12345", "21912345678", "https://placehold.co/150x150/8B4513/fff?text=TPG", "RJ", "20000-000" },
                    { 5L, "Specialty studio for high-quality paper, inks, and natural wools. Covers the Paper & Ink and Wool & Whimsey categories.", "Paper Avenue, 1500", "Room 101", "Porto Alegre", 5.0m, "Brazil", "felt.parchment@email.com", "Felt & Parchment Studio", "MOCK_HASH_FOR_SEEDING_12345", "51923456789", "https://placehold.co/150x150/A0522D/fff?text=FPS", "RS", "90000-000" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "products",
                columns: new[] { "product_id", "category_id", "description", "inventory", "name", "price", "seller_id" },
                values: new object[,]
                {
                    { 1L, 1L, "High-viscosity acrylic paints ideal for canvas and professional artwork. Non-toxic and fast-drying.", 150.0m, "Premium Acrylic Paint Set (12 Colors)", 4599, 1L },
                    { 2L, 2L, "Locally sourced stoneware clay, pre-mixed with a forest-green glaze. Perfect for firing at cone 6.", 40.0m, "Hand-Thrown Stoneware Bowl (Small)", 2250, 4L },
                    { 3L, 3L, "Complete kit for glass fusing, including safety gloves, cutting tools, and a selection of dichroic glass pieces.", 20.0m, "Glass Fusing Starter Kit - Dichroic", 9800, 2L },
                    { 4L, 4L, "Heavy-duty wrought iron piece, hand-forged using traditional blacksmithing methods. Durable matte black finish.", 15.0m, "Wrought Iron Candle Holder (Medieval Style)", 7500, 2L },
                    { 5L, 5L, "Natural Uruguayan Amethyst geode. Ideal for wire wrapping and jewelry making. Each stone is unique.", 75.0m, "Raw Amethyst Geode (Small, 50g)", 3500, 4L },
                    { 6L, 6L, "Premium full-grain leather, naturally tanned using vegetable extracts. Perfect thickness for wallets and small goods.", 30.0m, "Vegetable-Tanned Cowhide (A4 size)", 12000, 3L },
                    { 7L, 7L, "Traditional Sumi ink and five bamboo pens, housed in a wooden box. Suitable for calligraphy and detailed drawing.", 60.0m, "Japanese Calligraphy Ink Set (Sumi)", 3990, 5L },
                    { 8L, 8L, "Made from sustainable Oakwood, treated with mineral oil. A rustic, durable board ideal for kitchen use or display.", 25.0m, "Hand-Carved Oak Chopping Board", 5500, 3L },
                    { 9L, 9L, "Hand-woven from natural, flexible bamboo fibers. Perfect for laundry or general home storage.", 35.0m, "Large Woven Bamboo Basket (Storage)", 8500, 3L },
                    { 10L, 10L, "100% pure Merino wool, super soft and non-irritating. Perfect for baby clothes and sensitive skin projects.", 200.0m, "Merino Wool Skein (Sky Blue, 100g)", 1500, 5L },
                    { 11L, 11L, "Compact and lightweight sewing machine with 12 built-in stitches. Includes thread, needles, and foot pedal.", 50.0m, "Beginner Sewing Machine (Portable)", 29999, 1L },
                    { 12L, 1L, "Original painting featuring vibrant, thick acrylic layers on a gallery-wrapped canvas. Signed by the artist.", 5.0m, "Abstract Acrylic on Canvas", 35000, 1L },
                    { 13L, 1L, "High-quality archival giclée print of a digital abstract piece. Available in multiple sizes, ready for framing.", 100.0m, "Abstract Art Digital Print", 4500, 1L },
                    { 14L, 1L, "Triple-primed, 100% cotton canvas, perfect for large-scale acrylic and oil painting. Ready to hang.", 80.0m, "Acrylic-Primed Stretched Canvas (Large)", 7800, 1L },
                    { 15L, 1L, "Complete set of 24 artist-grade, heavy-body acrylic paints. High pigment concentration and lightfastness.", 120.0m, "Professional Acrylic Paint Set (24 Colors)", 6899, 1L },
                    { 16L, 1L, "Assortment of 20 synthetic and natural hair brushes for oil, acrylic, and watercolor techniques. Includes travel case.", 150.0m, "Deluxe Artist Brush Set (20 pcs)", 4200, 1L },
                    { 17L, 1L, "Original, personalized oil painting of your pet on a linen canvas. Requires high-resolution photo reference.", 5.0m, "Custom Pet Portrait Commission", 150000, 1L },
                    { 18L, 1L, "Set of 12 professional graphite pencils, ranging from 6H (hard) to 8B (soft), ideal for detailed drawing.", 300.0m, "Graphite Drawing Pencil Set (H-B range)", 2150, 1L },
                    { 19L, 1L, "A curated selection of 12 premium, pigment-rich oil paints, favored by classical artists. Excellent texture.", 90.0m, "Master Oil Paint Set (12 Colors, Fine Art)", 9999, 1L },
                    { 20L, 1L, "Museum-quality Giclée print of a scenic landscape, professionally framed with anti-glare glass.", 50.0m, "Framed Landscape Canvas Print", 12000, 1L },
                    { 21L, 1L, "Heavy-duty, floor-standing H-frame easel made from seasoned beechwood. Suitable for canvases up to 80 inches.", 10.0m, "Large Format Institutional Easel", 45000, 1L },
                    { 22L, 1L, "Unique original oil painting capturing a dramatic mountain vista. Varnished and ready to display.", 1.0m, "Original Landscape Oil Painting", 48000, 1L },
                    { 23L, 1L, "Thick, durable, textured paper pad suitable for wet and dry techniques, including watercolor, gouache, and ink.", 250.0m, "Mixed Media Paper Pad (A3, 30 sheets)", 2990, 1L },
                    { 24L, 1L, "High-detail, traditional oil portrait commission on a premium canvas. Consultation required for execution.", 3.0m, "Classic Portrait Oil Painting (Commission)", 95000, 1L },
                    { 25L, 1L, "A serene, atmospheric original oil work, characterized by soft focus and subtle colors. Unframed.", 1.0m, "Original Oil Painting: Misty Meadow", 52000, 1L },
                    { 26L, 1L, "Pocket-sized sketchbook with perforated pages and a durable cover, perfect for sketching outdoors (plein air).", 400.0m, "Premium Plein Air Sketchbook (A5)", 1850, 1L },
                    { 27L, 1L, "Hand-stitched sketchbook with a genuine leather cover and high-quality acid-free drawing paper. A true heirloom piece.", 30.0m, "Leather-Bound Sketchbook (Handmade)", 7500, 1L },
                    { 28L, 1L, "Personalized watercolor portrait on heavy cotton paper. Ideal for gifts or small displays.", 10.0m, "Custom Watercolor Portrait (Small)", 60000, 1L },
                    { 29L, 1L, "Signed and numbered limited edition print of a popular watercolor portrait series. Comes with certificate of authenticity.", 40.0m, "Watercolor Portrait Print (Limited Edition)", 8500, 1L },
                    { 30L, 1L, "A solid, adjustable H-frame easel for studio work, offering maximum stability for various canvas sizes.", 20.0m, "Studio H-Frame Wooden Easel", 15000, 1L },
                    { 31L, 2L, "Unique set of four ceramic coasters, hand-painted and kiln-fired with a protective glaze. Includes cork backing.", 80.0m, "Hand-Painted Ceramic Coasters (Set of 4)", 3999, 4L },
                    { 32L, 2L, "Modern, tall vase with a matte finish and geometric texture, perfect for dried arrangements or minimalist decor.", 30.0m, "Tall Geometric Ceramic Vase", 7990, 4L },
                    { 33L, 2L, "Large, durable dinner plate made from locally sourced clay with a rustic, uneven edge and semi-matte glaze.", 120.0m, "Rustic Clay Dinner Plate", 2550, 4L },
                    { 34L, 2L, "Small, detailed sculpture of a resting fox, finished with a smooth, glossy glaze. Ideal for desks or shelves.", 50.0m, "Miniature Fox Figurine Sculpture", 4900, 4L },
                    { 35L, 2L, "Beautifully hand-glazed ceramic serving bowl in deep ocean blue, ideal for salads, fruit, or as a centerpiece.", 45.0m, "Large Ocean-Blue Glazed Serving Bowl", 5999, 4L },
                    { 36L, 2L, "Set of three nesting earthenware mixing bowls (small, medium, large), durable and perfect for baking and prep.", 25.0m, "Pottery Mixing Bowl Set (3 Sizes)", 11500, 4L },
                    { 37L, 2L, "Set of four small, hand-thrown sake cups with a delicate, speckled glaze, perfect for traditional or modern serving.", 60.0m, "Japanese Style Sake Cup Set (4 pcs)", 4590, 4L },
                    { 38L, 2L, "Rustic, unglazed terracotta jar with a tight-fitting cork lid, suitable for dry goods storage or decoration.", 40.0m, "Large Terracotta Storage Jar", 6500, 4L },
                    { 39L, 3L, "Delicate, hand-blown glass ornament with shimmering iridescence. Perfect for display or holiday decor.", 100.0m, "Blown Glass Ornament (Limited Edition)", 4500, 2L },
                    { 40L, 3L, "Tall, slender vase created through traditional glass blowing techniques. Features a subtle blue gradient.", 30.0m, "Elegant Blown Glass Vase (Tall)", 12000, 2L },
                    { 41L, 3L, "Solid, precision-cut crystal orb that refracts light beautifully. Ideal as a desk accessory or collector's item.", 50.0m, "Faceted Crystal Orb Paperweight", 6500, 2L },
                    { 42L, 3L, "A unique glass sculpture shaped like a bell, producing a delicate, resonant tone. Decorative centerpiece.", 25.0m, "Hand-Blown Glass Bell Sculpture", 9500, 2L },
                    { 43L, 3L, "Set of four durable glass coasters created by fusing colored glass sheets. Includes small rubber feet for protection.", 70.0m, "Set of 4 Fused Glass Coasters", 5500, 2L },
                    { 44L, 3L, "Premium, heavy-bottomed crystal decanter with detailed diamond-cut engraving. Suitable for whiskey or spirits.", 20.0m, "Engraved Crystal Glass Decanter", 18000, 2L },
                    { 45L, 3L, "Circular glass paperweight featuring an intricate, colorful internal swirl design. Signed by the artisan.", 80.0m, "Artisan Glass Paperweight (Swirl)", 3200, 2L },
                    { 46L, 3L, "Set of six high-quality glass tumblers with a weighted base. Perfect for everyday use or entertaining.", 40.0m, "Deluxe Glass Tumbler Set (6 Pcs)", 7500, 2L },
                    { 47L, 3L, "A commission for a unique Art Deco-style stained glass window panel. Price is an estimate; final cost depends on size/complexity.", 5.0m, "Custom Stained Glass Panel (Art Deco)", 450000, 2L },
                    { 48L, 4L, "Hand-forged steel wall hook with a decorative scroll design. Ideal for coats or tools. Matte black finish.", 150.0m, "Decorative Steel Hook (Wall Mount)", 2200, 2L },
                    { 49L, 4L, "Wrought iron wine rack, holds six standard bottles securely. Rustic patina finish.", 35.0m, "Forged Metal Bottle Rack (6 Bottles)", 7900, 2L },
                    { 50L, 4L, "Solid steel fire poker, designed for heavy use in fireplaces or wood stoves. Ergonomic handle.", 20.0m, "Hand-Forged Fire Poker (Heavy Duty)", 6500, 2L },
                    { 51L, 4L, "Simple, durable iron utility hook. Excellent for kitchens or garages where a rustic look is desired.", 200.0m, "Hand-Forged Utility Hook (Rustic)", 1800, 2L },
                    { 52L, 4L, "Small abstract sculpture made from twisted, polished steel bars. Unique art piece for a modern desk.", 10.0m, "Hand-Forged Abstract Sculpture (Desk Size)", 15000, 2L },
                    { 53L, 4L, "Made-to-order heavy-gauge iron grill or grate insert. Requires dimensions. High heat resistance.", 10.0m, "Custom Iron Grill Insert (Heavy Gauge)", 35000, 2L },
                    { 54L, 4L, "Geometric wall hanging created from welded metal scraps. Finished with a protective clear coat.", 15.0m, "Abstract Metal Wall Art Piece (Small)", 9800, 2L },
                    { 55L, 4L, "Compact and durable bottle opener made from brushed stainless steel. Ideal for keychains.", 300.0m, "Steel Bottle Opener (Keyring Style)", 1250, 2L },
                    { 56L, 4L, "Pair of elegant wrought iron candlesticks, hand-twisted design. Suitable for taper candles.", 70.0m, "Wrought Iron Candlestick (Set of 2)", 4999, 2L },
                    { 57L, 4L, "Set of four iron coasters with a decorative base and felt pads to protect surfaces. Matte black finish.", 80.0m, "Wrought Iron Coasters (Set of 4)", 5500, 2L },
                    { 58L, 5L, "Delicate drop earrings featuring polished amethyst stones set in hypoallergenic sterling silver.", 85.0m, "Elegant Amethyst Earrings (Sterling Silver)", 18990, 4L },
                    { 59L, 5L, "Hand-strung long necklace with vibrant mixed glass beads and a simple brass clasp. Versatile piece.", 110.0m, "Colorful Glass Beaded Necklace (Long)", 14500, 4L },
                    { 60L, 5L, "Classic stud earrings featuring premium faceted crystals for maximum brilliance and daily wear.", 150.0m, "Sparkling Crystal Stud Earrings (Small)", 15990, 4L },
                    { 61L, 5L, "Statement cuff bracelet, hand-polished brass with a comfortable, adjustable fit. Modern design.", 40.0m, "Wide Polished Cuff Bracelet (Brass)", 29900, 4L },
                    { 62L, 5L, "Thin and delicate gold-plated chain, perfect for layering or wearing alone. Durable and tarnish-resistant.", 180.0m, "Delicate Gold Plated Chain Necklace", 49990, 4L },
                    { 63L, 5L, "Sterling silver pendant featuring a deep red, oval-cut natural garnet stone. Includes a simple chain.", 60.0m, "Deep Red Garnet Pendant (Silver)", 21000, 4L },
                    { 64L, 5L, "Bracelet adorned with various small tumbled gemstones (quartz, jade, rose quartz). Adjustable closure.", 75.0m, "Multi-Gemstone Charm Bracelet", 34950, 4L },
                    { 65L, 5L, "Hand-assembled brooch featuring genuine freshwater pearls in a vintage floral arrangement. Pin closure.", 30.0m, "Vintage Freshwater Pearl Brooch", 17550, 4L },
                    { 66L, 5L, "Personalized pendant charm with the customer's zodiac sign engraved. Gold-plated option available.", 95.0m, "Zodiac Sign Pendant Charm (Custom)", 11990, 4L },
                    { 67L, 5L, "Simple, polished sterling silver band ring. Unisex design, perfect for stacking or as a promise ring.", 200.0m, "Minimalist Sterling Silver Ring", 9500, 4L },
                    { 68L, 5L, "Sterling silver ring featuring natural turquoise stone inlay in a bohemian, detailed setting.", 50.0m, "Bohemian Turquoise Inlaid Ring", 19900, 4L },
                    { 69L, 6L, "Hand-stitched leather cover for A5 journals, featuring a wrap-around tie closure. Natural patina finish.", 90.0m, "Rustic Leather Journal Cover (A5)", 16500, 3L },
                    { 70L, 6L, "Small, durable leather strap with a polished metal ring. Customizable with up to three initials.", 250.0m, "Customizable Leather Key Fob", 3500, 3L },
                    { 71L, 6L, "Heavy-duty, full-grain leather belt with a solid brass buckle. Designed to last a lifetime.", 110.0m, "Full-Grain Leather Belt (Classic Brown)", 22000, 3L },
                    { 72L, 6L, "Minimalist card holder for 4-6 cards, made from slim, smooth leather. Ideal for front pocket carry.", 180.0m, "Slim Leather Card Holder (Minimalist)", 8990, 3L },
                    { 73L, 6L, "Set of three small leather snaps for organizing electronic cables and wires. Keeps desk tidy.", 300.0m, "Leather Cord Organizer (Set of 3)", 4500, 3L },
                    { 74L, 6L, "Journal cover made from distressed leather, giving it a vintage, rugged look. Includes an elastic pen loop.", 70.0m, "Leather Bound Journal Cover (Distressed)", 17990, 3L },
                    { 75L, 6L, "Protective and stylish leather sleeve for 13-inch laptops, lined with soft suede. Simple snap closure.", 50.0m, "Leather Laptop Sleeve (13 inch)", 35000, 3L },
                    { 76L, 6L, "Classic passport holder with slots for boarding passes and credit cards. Engravable initials option.", 100.0m, "Travel Leather Passport Holder", 14500, 3L },
                    { 77L, 6L, "Traditional bifold wallet with multiple card slots and a dedicated cash compartment. Smooth, durable leather.", 130.0m, "Bifold Leather Wallet (Classic Design)", 19990, 3L },
                    { 78L, 6L, "Full-sized leather satchel with adjustable cross-body strap. Ideal for daily commute or travel. Features multiple compartments.", 20.0m, "Large Leather Satchel Bag (Cross-body)", 79900, 3L },
                    { 79L, 7L, "Set of 10 heavyweight, textured note cards with original watercolor designs. Matching envelopes included.", 150.0m, "Artistic Hand-Painted Note Cards (Set of 10)", 4990, 5L },
                    { 80L, 7L, "Set includes a fine wooden pen, three interchangeable nibs, and a bottle of rich black India ink. Perfect for beginners.", 100.0m, "Traditional Calligraphy Starter Set", 6500, 5L },
                    { 81L, 7L, "Custom design and printing of personalized letterheads on premium linen paper. Min. order: 50 sheets.", 100.0m, "Bespoke Custom Stationery - Letterhead", 9999, 5L },
                    { 82L, 7L, "Monogrammed stationery set including 50 letterheads and 50 envelopes, printed on thick cotton paper.", 80.0m, "Deluxe Custom Stationery Set (Monogrammed)", 18000, 5L },
                    { 83L, 7L, "Professional set including letterhead, business cards, and envelopes with integrated custom branding. Design consultation required.", 50.0m, "Premium Custom Stationery Set (Professional)", 25000, 5L },
                    { 84L, 7L, "A5 journal with a stitched linen spine and archival, acid-free lined paper. Ideal for daily writing or sketching.", 120.0m, "Hand-Bound Linen Journal (Lined)", 12990, 5L },
                    { 85L, 7L, "Set of 20 unique envelopes made from recycled cotton fibers. Each envelope has a slight variation in color and texture.", 200.0m, "Handmade Paper Envelopes (Mixed Colors)", 3500, 5L },
                    { 86L, 7L, "Smooth-writing fountain pen with fine nib, five ink cartridges, and a converter for bottled ink. Elegant gift box.", 95.0m, "Fountain Pen and Ink Set (Beginner's)", 7990, 5L },
                    { 87L, 7L, "Signed and numbered A4 art prints, created using traditional letterpress techniques. Deep, tactile impression.", 70.0m, "Limited Edition Letterpress Prints (A4)", 5990, 5L },
                    { 88L, 7L, "Custom-engraved solid brass seal with a wooden handle, three sticks of sealing wax, and a melting spoon. Perfect for elegant correspondence.", 60.0m, "Personalized Brass Wax Seal Kit", 9500, 5L },
                    { 89L, 11L, "Durable canvas tote bag featuring custom machine or hand embroidery (up to 10 characters). Perfect for groceries or books.", 75.0m, "Custom Embroidered Canvas Tote Bag", 8990, 1L },
                    { 90L, 11L, "Medium-sized wall tapestry, intricately stitched with mixed threads and textures. Ready to hang.", 15.0m, "Hand-Stitched Abstract Tapestry (Medium)", 24990, 1L },
                    { 91L, 11L, "Warm and soft quilted blanket made from 100% pre-washed cotton fabric. Unique pattern and design.", 20.0m, "Handmade Quilted Throw Blanket (Cotton)", 35000, 1L },
                    { 92L, 11L, "Small, durable patch with custom woven or embroidered design. Iron-on backing for easy application.", 250.0m, "Woven Embroidered Iron-On Patch (Custom)", 1990, 1L },
                    { 93L, 11L, "Set of six reversible fabric coasters, made with high-quality, absorbent cotton. Machine washable.", 100.0m, "Handmade Fabric Coasters (Set of 6)", 4500, 1L },
                    { 94L, 11L, "Small framed artwork created using delicate silk ribbon embroidery techniques, featuring floral motifs.", 30.0m, "Silk Ribbon Embroidery Art (Framed)", 15000, 1L },
                    { 95L, 11L, "Modern wall decoration featuring botanical hand embroidery stretched within a wooden hoop frame (20cm).", 50.0m, "Embroidered Wall Hoop Art (Botanical)", 7500, 1L },
                    { 96L, 11L, "Unique wall piece combining weaving, stitching, and mixed media fabrics for a textured, abstract look.", 10.0m, "Abstract Textile Wall Art (Woven/Mixed Media)", 19990, 1L },
                    { 97L, 11L, "Large wooden hoop (30cm) featuring a highly detailed, delicate floral embroidery design. Perfect centerpiece.", 40.0m, "Delicate Floral Embroidered Hoop (Large)", 12000, 1L },
                    { 98L, 11L, "Square throw cushion (45x45cm) with a unique patterned fabric cover, hand-sewn with an invisible zipper closure. Pillow insert included.", 60.0m, "Patterned Throw Cushion (Hand-Sewn)", 8500, 1L },
                    { 99L, 8L, "An organic, modern art piece carved from hardwood. Ideal for contemporary decor.", 15.0m, "Abstract Hardwood Sculpture", 18990, 3L },
                    { 100L, 8L, "Detailed wooden statue, hand-carved with classic motifs. A collector's art piece.", 10.0m, "Carved Wooden Figurative Statue", 24900, 3L },
                    { 101L, 8L, "Elegant coasters with a smooth, moisture-resistant finish. Adds rustic-chic style to the table.", 120.0m, "Wooden Coasters Set (4 pieces)", 4500, 3L },
                    { 102L, 8L, "Hinged and clasped box, ideal for storing jewelry or small treasures. Design highlights the natural wood grain.", 70.0m, "Decorative Wooden Keepsake Box", 7850, 3L },
                    { 103L, 8L, "Unique kitchen utensil, carved with attention to detail. Perfect for serving.", 150.0m, "Hand-Carved Wooden Serving Spoon", 3450, 3L },
                    { 104L, 8L, "Functional and meticulously carved whistle. Great for collectors or as a handmade toy.", 200.0m, "Hand-Carved Wooden Whistle", 2299, 3L },
                    { 105L, 8L, "Charming tabletop salt cellar with a lid, precisely turned for gourmet salt.", 80.0m, "Wood Turned Salt Cellar", 4999, 3L },
                    { 106L, 8L, "Beautiful turned bowl for salads, fruits, or as a centerpiece. Hand-polished to highlight the grain.", 60.0m, "Large Natural Wooden Serving Bowl", 6790, 3L },
                    { 107L, 8L, "Robust and durable board, made from glued wood blocks. Perfect for preparation or presentation.", 90.0m, "Artisan Block Wooden Cutting Board", 8990, 3L },
                    { 108L, 8L, "Board with an organic shape and handle, ideal for arranging and serving charcuterie and appetizers.", 75.0m, "Rustic Wooden Serving Board for Cheeses and Snacks", 9500, 3L },
                    { 109L, 8L, "Lightweight tray with raised edges and recessed handles, ideal for breakfast or table organization.", 50.0m, "Elegant Wooden Serving Tray", 11200, 3L },
                    { 110L, 9L, "Large, shallow bowl woven from natural bamboo strips. Lightweight and perfect for fruit or bread storage.", 110.0m, "Hand-Woven Bamboo Fruit Bowl", 4990, 3L },
                    { 111L, 9L, "Flat, tightly woven rattan tray, ideal as a centerpiece on a coffee table or for serving small items.", 150.0m, "Round Rattan Decorative Tray (Small)", 3500, 3L },
                    { 112L, 9L, "Durable, cylindrical storage basket woven from thick rattan, complete with a fitted lid for organized storage.", 40.0m, "Large Rattan Storage Basket with Lid", 12000, 3L },
                    { 113L, 9L, "Set of six small, round coasters woven from fine rattan fibers, providing a natural, protective surface.", 200.0m, "Set of 6 Rattan Coasters", 2500, 3L },
                    { 114L, 9L, "Mid-century style lamp base woven from open-weave rattan. Provides warm, textured lighting (shade and bulb not included).", 30.0m, "Modern Rattan Table Lamp Base", 9500, 3L },
                    { 115L, 9L, "Soft, hand-braided jute rope cover for standard plant pots. Adds a nautical, bohemian touch to indoor plants.", 90.0m, "Woven Rope Plant Pot Cover", 3990, 3L },
                    { 116L, 9L, "Large circular mat hand-braided from natural seagrass. Perfect as a floor mat or textured wall hanging.", 70.0m, "Round Hand-Braided Seagrass Mat (60cm)", 5500, 3L },
                    { 117L, 9L, "Extra-large, traditional wicker basket with handles. Ideal for laundry or general bulk storage.", 35.0m, "Classic Wicker Laundry Basket", 11000, 3L },
                    { 118L, 9L, "Geometric-shaped lampshade woven from natural wicker, creating beautiful light patterns (wiring not included).", 50.0m, "Geometric Wicker Pendant Lamp Shade", 7500, 3L },
                    { 119L, 9L, "A fully woven rattan accent chair with a high back and comfortable seat cushion (included). Durable and stylish.", 10.0m, "Hand-Woven Rattan Accent Chair", 45000, 3L },
                    { 120L, 9L, "Rectangular box woven from thick seagrass with integrated handles. Suitable for shelf storage.", 60.0m, "Woven Seagrass Storage Box (Rectangular)", 6990, 3L },
                    { 121L, 9L, "Unique circular wall decoration featuring abstract patterns woven from mixed natural fibers (jute, rattan, cotton).", 45.0m, "Abstract Woven Wall Decor Piece", 8900, 3L },
                    { 122L, 9L, "Large wall tapestry featuring intricate knotting and weaving techniques, finished with long cotton fringes.", 20.0m, "Bohemian Woven Wall Hanging (Fringed)", 15000, 3L },
                    { 123L, 10L, "Luxurious, oversized throw blanket hand-knit with super-thick Merino wool. Perfect for cozying up on the sofa.", 25.0m, "Hand-Knit Chunky Throw Blanket (Merino)", 39990, 5L },
                    { 124L, 10L, "Soft, warm, and durable slippers made from needle-felted wool, featuring a non-slip suede sole. Unique colors.", 120.0m, "Hand-Felted Wool Slippers (Indoor)", 8990, 5L },
                    { 125L, 10L, "Thick, warm, and stylish fingerless gloves knitted from durable natural wool. Ideal for outdoor activities or typing.", 150.0m, "Hand-Knit Fingerless Wool Gloves", 3500, 5L },
                    { 126L, 10L, "Extra-long and soft scarf, knitted from a premium Alpaca and wool blend. Provides excellent warmth without bulk.", 90.0m, "Long Hand-Knitted Scarf (Alpaca Blend)", 7500, 5L },
                    { 127L, 10L, "Cute and cozy hand-knitted sweater for small to medium dogs, made from hypoallergenic yarn. Different colors available.", 80.0m, "Whimsical Knitted Dog Sweater (Various Sizes)", 4990, 5L },
                    { 128L, 10L, "Traditional knitted scarf featuring a classic plaid pattern. Perfect for everyday winter wear.", 110.0m, "Classic Knitted Wool Scarf (Plaid)", 5990, 5L },
                    { 129L, 10L, "Warm, comfortable, oversized sweater, hand-knitted with a cable pattern. Relaxed fit.", 50.0m, "Handmade Oversized Knitted Sweater (Unisex)", 14990, 5L },
                    { 130L, 10L, "Warm beanie hat, tightly knitted from soft wool, with a slouchy, comfortable fit. Available in dark colors.", 200.0m, "Soft Knit Wool Beanie Hat (Slouchy Fit)", 3990, 5L },
                    { 131L, 10L, "Medium-weight blanket woven from pure wool, featuring a subtle geometric pattern. Durable and naturally insulating.", 40.0m, "Pure Wool Throw Blanket (Geometric Pattern)", 18000, 5L },
                    { 132L, 10L, "Wide, hand-knitted headband designed to keep ears warm. Ideal for running or outdoor activities in cold weather.", 180.0m, "Hand-Knit Wool Headband/Ear Warmer", 2500, 5L },
                    { 133L, 10L, "Super-thick, soft woolen socks designed for sleeping or lounging. Provides maximum warmth.", 250.0m, "Thick Woolen Bed Socks (Crew Length)", 1990, 5L }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "product_image",
                columns: new[] { "product_image_id", "alt", "product_id", "url" },
                values: new object[,]
                {
                    { 1L, "Main image of Premium Acrylic Paint Set (12 Colors).", 1L, "https://mock-cloud-storage.com/images/products/1/1/1_main.jpg" },
                    { 2L, "Detail 1 of Premium Acrylic Paint Set (12 Colors).", 1L, "https://mock-cloud-storage.com/images/products/1/1/1_detail_1.jpg" },
                    { 3L, "Main image of Hand-Thrown Stoneware Bowl (Small).", 2L, "https://mock-cloud-storage.com/images/products/4/2/2_main.jpg" },
                    { 4L, "Detail 1 of Hand-Thrown Stoneware Bowl (Small).", 2L, "https://mock-cloud-storage.com/images/products/4/2/2_detail_1.jpg" },
                    { 5L, "Main image of Glass Fusing Starter Kit - Dichroic.", 3L, "https://mock-cloud-storage.com/images/products/2/3/3_main.jpg" },
                    { 6L, "Detail 1 of Glass Fusing Starter Kit - Dichroic.", 3L, "https://mock-cloud-storage.com/images/products/2/3/3_detail_1.jpg" },
                    { 7L, "Main image of Wrought Iron Candle Holder (Medieval Style).", 4L, "https://mock-cloud-storage.com/images/products/2/4/4_main.jpg" },
                    { 8L, "Detail 1 of Wrought Iron Candle Holder (Medieval Style).", 4L, "https://mock-cloud-storage.com/images/products/2/4/4_detail_1.jpg" },
                    { 9L, "Main image of Raw Amethyst Geode (Small, 50g).", 5L, "https://mock-cloud-storage.com/images/products/4/5/5_main.jpg" },
                    { 10L, "Detail 1 of Raw Amethyst Geode (Small, 50g).", 5L, "https://mock-cloud-storage.com/images/products/4/5/5_detail_1.jpg" },
                    { 11L, "Main image of Vegetable-Tanned Cowhide (A4 size).", 6L, "https://mock-cloud-storage.com/images/products/3/6/6_main.jpg" },
                    { 12L, "Detail 1 of Vegetable-Tanned Cowhide (A4 size).", 6L, "https://mock-cloud-storage.com/images/products/3/6/6_detail_1.jpg" },
                    { 13L, "Main image of Japanese Calligraphy Ink Set (Sumi).", 7L, "https://mock-cloud-storage.com/images/products/5/7/7_main.jpg" },
                    { 14L, "Detail 1 of Japanese Calligraphy Ink Set (Sumi).", 7L, "https://mock-cloud-storage.com/images/products/5/7/7_detail_1.jpg" },
                    { 15L, "Main image of Hand-Carved Oak Chopping Board.", 8L, "https://mock-cloud-storage.com/images/products/3/8/8_main.jpg" },
                    { 16L, "Detail 1 of Hand-Carved Oak Chopping Board.", 8L, "https://mock-cloud-storage.com/images/products/3/8/8_detail_1.jpg" },
                    { 17L, "Main image of Large Woven Bamboo Basket (Storage).", 9L, "https://mock-cloud-storage.com/images/products/3/9/9_main.jpg" },
                    { 18L, "Detail 1 of Large Woven Bamboo Basket (Storage).", 9L, "https://mock-cloud-storage.com/images/products/3/9/9_detail_1.jpg" },
                    { 19L, "Main image of Merino Wool Skein (Sky Blue, 100g).", 10L, "https://mock-cloud-storage.com/images/products/5/10/10_main.jpg" },
                    { 20L, "Detail 1 of Merino Wool Skein (Sky Blue, 100g).", 10L, "https://mock-cloud-storage.com/images/products/5/10/10_detail_1.jpg" },
                    { 21L, "Main image of Beginner Sewing Machine (Portable).", 11L, "https://mock-cloud-storage.com/images/products/1/11/11_main.jpg" },
                    { 22L, "Detail 1 of Beginner Sewing Machine (Portable).", 11L, "https://mock-cloud-storage.com/images/products/1/11/11_detail_1.jpg" },
                    { 23L, "Main image of Abstract Acrylic on Canvas.", 12L, "https://mock-cloud-storage.com/images/products/1/1/12_main.jpg" },
                    { 24L, "Detail 1 of Abstract Acrylic on Canvas.", 12L, "https://mock-cloud-storage.com/images/products/1/1/12_detail_1.jpg" },
                    { 25L, "Detail 2 of Abstract Acrylic on Canvas.", 12L, "https://mock-cloud-storage.com/images/products/1/1/12_detail_2.jpg" },
                    { 26L, "Main image of Abstract Art Digital Print.", 13L, "https://mock-cloud-storage.com/images/products/1/1/13_main.jpg" },
                    { 27L, "Detail 1 of Abstract Art Digital Print.", 13L, "https://mock-cloud-storage.com/images/products/1/1/13_detail_1.jpg" },
                    { 28L, "Main image of Acrylic-Primed Stretched Canvas (Large).", 14L, "https://mock-cloud-storage.com/images/products/1/1/14_main.jpg" },
                    { 29L, "Detail 1 of Acrylic-Primed Stretched Canvas (Large).", 14L, "https://mock-cloud-storage.com/images/products/1/1/14_detail_1.jpg" },
                    { 30L, "Detail 2 of Acrylic-Primed Stretched Canvas (Large).", 14L, "https://mock-cloud-storage.com/images/products/1/1/14_detail_2.jpg" },
                    { 31L, "Main image of Professional Acrylic Paint Set (24 Colors).", 15L, "https://mock-cloud-storage.com/images/products/1/1/15_main.jpg" },
                    { 32L, "Detail 1 of Professional Acrylic Paint Set (24 Colors).", 15L, "https://mock-cloud-storage.com/images/products/1/1/15_detail_1.jpg" },
                    { 33L, "Main image of Deluxe Artist Brush Set (20 pcs).", 16L, "https://mock-cloud-storage.com/images/products/1/1/16_main.jpg" },
                    { 34L, "Detail 1 of Deluxe Artist Brush Set (20 pcs).", 16L, "https://mock-cloud-storage.com/images/products/1/1/16_detail_1.jpg" },
                    { 35L, "Detail 2 of Deluxe Artist Brush Set (20 pcs).", 16L, "https://mock-cloud-storage.com/images/products/1/1/16_detail_2.jpg" },
                    { 36L, "Main image of Custom Pet Portrait Commission.", 17L, "https://mock-cloud-storage.com/images/products/1/1/17_main.jpg" },
                    { 37L, "Detail 1 of Custom Pet Portrait Commission.", 17L, "https://mock-cloud-storage.com/images/products/1/1/17_detail_1.jpg" },
                    { 38L, "Main image of Graphite Drawing Pencil Set (H-B range).", 18L, "https://mock-cloud-storage.com/images/products/1/1/18_main.jpg" },
                    { 39L, "Detail 1 of Graphite Drawing Pencil Set (H-B range).", 18L, "https://mock-cloud-storage.com/images/products/1/1/18_detail_1.jpg" },
                    { 40L, "Detail 2 of Graphite Drawing Pencil Set (H-B range).", 18L, "https://mock-cloud-storage.com/images/products/1/1/18_detail_2.jpg" },
                    { 41L, "Main image of Master Oil Paint Set (12 Colors, Fine Art).", 19L, "https://mock-cloud-storage.com/images/products/1/1/19_main.jpg" },
                    { 42L, "Detail 1 of Master Oil Paint Set (12 Colors, Fine Art).", 19L, "https://mock-cloud-storage.com/images/products/1/1/19_detail_1.jpg" },
                    { 43L, "Main image of Framed Landscape Canvas Print.", 20L, "https://mock-cloud-storage.com/images/products/1/1/20_main.jpg" },
                    { 44L, "Detail 1 of Framed Landscape Canvas Print.", 20L, "https://mock-cloud-storage.com/images/products/1/1/20_detail_1.jpg" },
                    { 45L, "Detail 2 of Framed Landscape Canvas Print.", 20L, "https://mock-cloud-storage.com/images/products/1/1/20_detail_2.jpg" },
                    { 46L, "Main image of Large Format Institutional Easel.", 21L, "https://mock-cloud-storage.com/images/products/1/1/21_main.jpg" },
                    { 47L, "Detail 1 of Large Format Institutional Easel.", 21L, "https://mock-cloud-storage.com/images/products/1/1/21_detail_1.jpg" },
                    { 48L, "Main image of Original Landscape Oil Painting.", 22L, "https://mock-cloud-storage.com/images/products/1/1/22_main.jpg" },
                    { 49L, "Detail 1 of Original Landscape Oil Painting.", 22L, "https://mock-cloud-storage.com/images/products/1/1/22_detail_1.jpg" },
                    { 50L, "Detail 2 of Original Landscape Oil Painting.", 22L, "https://mock-cloud-storage.com/images/products/1/1/22_detail_2.jpg" },
                    { 51L, "Main image of Mixed Media Paper Pad (A3, 30 sheets).", 23L, "https://mock-cloud-storage.com/images/products/1/1/23_main.jpg" },
                    { 52L, "Detail 1 of Mixed Media Paper Pad (A3, 30 sheets).", 23L, "https://mock-cloud-storage.com/images/products/1/1/23_detail_1.jpg" },
                    { 53L, "Main image of Classic Portrait Oil Painting (Commission).", 24L, "https://mock-cloud-storage.com/images/products/1/1/24_main.jpg" },
                    { 54L, "Detail 1 of Classic Portrait Oil Painting (Commission).", 24L, "https://mock-cloud-storage.com/images/products/1/1/24_detail_1.jpg" },
                    { 55L, "Detail 2 of Classic Portrait Oil Painting (Commission).", 24L, "https://mock-cloud-storage.com/images/products/1/1/24_detail_2.jpg" },
                    { 56L, "Main image of Original Oil Painting: Misty Meadow.", 25L, "https://mock-cloud-storage.com/images/products/1/1/25_main.jpg" },
                    { 57L, "Detail 1 of Original Oil Painting: Misty Meadow.", 25L, "https://mock-cloud-storage.com/images/products/1/1/25_detail_1.jpg" },
                    { 58L, "Main image of Premium Plein Air Sketchbook (A5).", 26L, "https://mock-cloud-storage.com/images/products/1/1/26_main.jpg" },
                    { 59L, "Detail 1 of Premium Plein Air Sketchbook (A5).", 26L, "https://mock-cloud-storage.com/images/products/1/1/26_detail_1.jpg" },
                    { 60L, "Detail 2 of Premium Plein Air Sketchbook (A5).", 26L, "https://mock-cloud-storage.com/images/products/1/1/26_detail_2.jpg" },
                    { 61L, "Main image of Leather-Bound Sketchbook (Handmade).", 27L, "https://mock-cloud-storage.com/images/products/1/1/27_main.jpg" },
                    { 62L, "Detail 1 of Leather-Bound Sketchbook (Handmade).", 27L, "https://mock-cloud-storage.com/images/products/1/1/27_detail_1.jpg" },
                    { 63L, "Main image of Custom Watercolor Portrait (Small).", 28L, "https://mock-cloud-storage.com/images/products/1/1/28_main.jpg" },
                    { 64L, "Detail 1 of Custom Watercolor Portrait (Small).", 28L, "https://mock-cloud-storage.com/images/products/1/1/28_detail_1.jpg" },
                    { 65L, "Detail 2 of Custom Watercolor Portrait (Small).", 28L, "https://mock-cloud-storage.com/images/products/1/1/28_detail_2.jpg" },
                    { 66L, "Main image of Watercolor Portrait Print (Limited Edition).", 29L, "https://mock-cloud-storage.com/images/products/1/1/29_main.jpg" },
                    { 67L, "Detail 1 of Watercolor Portrait Print (Limited Edition).", 29L, "https://mock-cloud-storage.com/images/products/1/1/29_detail_1.jpg" },
                    { 68L, "Main image of Studio H-Frame Wooden Easel.", 30L, "https://mock-cloud-storage.com/images/products/1/1/30_main.jpg" },
                    { 69L, "Detail 1 of Studio H-Frame Wooden Easel.", 30L, "https://mock-cloud-storage.com/images/products/1/1/30_detail_1.jpg" },
                    { 70L, "Detail 2 of Studio H-Frame Wooden Easel.", 30L, "https://mock-cloud-storage.com/images/products/1/1/30_detail_2.jpg" },
                    { 71L, "Main image of Hand-Painted Ceramic Coasters (Set of 4).", 31L, "https://mock-cloud-storage.com/images/products/4/2/31_main.jpg" },
                    { 72L, "Detail 1 of Hand-Painted Ceramic Coasters (Set of 4).", 31L, "https://mock-cloud-storage.com/images/products/4/2/31_detail_1.jpg" },
                    { 73L, "Main image of Tall Geometric Ceramic Vase.", 32L, "https://mock-cloud-storage.com/images/products/4/2/32_main.jpg" },
                    { 74L, "Detail 1 of Tall Geometric Ceramic Vase.", 32L, "https://mock-cloud-storage.com/images/products/4/2/32_detail_1.jpg" },
                    { 75L, "Detail 2 of Tall Geometric Ceramic Vase.", 32L, "https://mock-cloud-storage.com/images/products/4/2/32_detail_2.jpg" },
                    { 76L, "Main image of Rustic Clay Dinner Plate.", 33L, "https://mock-cloud-storage.com/images/products/4/2/33_main.jpg" },
                    { 77L, "Detail 1 of Rustic Clay Dinner Plate.", 33L, "https://mock-cloud-storage.com/images/products/4/2/33_detail_1.jpg" },
                    { 78L, "Main image of Miniature Fox Figurine Sculpture.", 34L, "https://mock-cloud-storage.com/images/products/4/2/34_main.jpg" },
                    { 79L, "Detail 1 of Miniature Fox Figurine Sculpture.", 34L, "https://mock-cloud-storage.com/images/products/4/2/34_detail_1.jpg" },
                    { 80L, "Detail 2 of Miniature Fox Figurine Sculpture.", 34L, "https://mock-cloud-storage.com/images/products/4/2/34_detail_2.jpg" },
                    { 81L, "Main image of Large Ocean-Blue Glazed Serving Bowl.", 35L, "https://mock-cloud-storage.com/images/products/4/2/35_main.jpg" },
                    { 82L, "Detail 1 of Large Ocean-Blue Glazed Serving Bowl.", 35L, "https://mock-cloud-storage.com/images/products/4/2/35_detail_1.jpg" },
                    { 83L, "Main image of Pottery Mixing Bowl Set (3 Sizes).", 36L, "https://mock-cloud-storage.com/images/products/4/2/36_main.jpg" },
                    { 84L, "Detail 1 of Pottery Mixing Bowl Set (3 Sizes).", 36L, "https://mock-cloud-storage.com/images/products/4/2/36_detail_1.jpg" },
                    { 85L, "Detail 2 of Pottery Mixing Bowl Set (3 Sizes).", 36L, "https://mock-cloud-storage.com/images/products/4/2/36_detail_2.jpg" },
                    { 86L, "Main image of Japanese Style Sake Cup Set (4 pcs).", 37L, "https://mock-cloud-storage.com/images/products/4/2/37_main.jpg" },
                    { 87L, "Detail 1 of Japanese Style Sake Cup Set (4 pcs).", 37L, "https://mock-cloud-storage.com/images/products/4/2/37_detail_1.jpg" },
                    { 88L, "Main image of Large Terracotta Storage Jar.", 38L, "https://mock-cloud-storage.com/images/products/4/2/38_main.jpg" },
                    { 89L, "Detail 1 of Large Terracotta Storage Jar.", 38L, "https://mock-cloud-storage.com/images/products/4/2/38_detail_1.jpg" },
                    { 90L, "Detail 2 of Large Terracotta Storage Jar.", 38L, "https://mock-cloud-storage.com/images/products/4/2/38_detail_2.jpg" },
                    { 91L, "Main image for artisan product: Blown Glass Ornament (Limited Edition)", 39L, "https://mock-cloud-storage.com/images/products/2/3/blown_glass_ornament.png" },
                    { 92L, "Main image for artisan product: Elegant Blown Glass Vase (Tall)", 40L, "https://mock-cloud-storage.com/images/products/2/3/blown_glass_vase.png" },
                    { 93L, "Main image for artisan product: Faceted Crystal Orb Paperweight", 41L, "https://mock-cloud-storage.com/images/products/2/3/crystal_orb.png" },
                    { 94L, "Main image for artisan product: Hand-Blown Glass Bell Sculpture", 42L, "https://mock-cloud-storage.com/images/products/2/3/glass_bell.png" },
                    { 95L, "Main image for artisan product: Set of 4 Fused Glass Coasters", 43L, "https://mock-cloud-storage.com/images/products/2/3/glass_coasters.png" },
                    { 96L, "Main image for artisan product: Engraved Crystal Glass Decanter", 44L, "https://mock-cloud-storage.com/images/products/2/3/glass_decanter.png" },
                    { 97L, "Main image for artisan product: Artisan Glass Paperweight (Swirl)", 45L, "https://mock-cloud-storage.com/images/products/2/3/glass_paperweight.png" },
                    { 98L, "Main image for artisan product: Deluxe Glass Tumbler Set (6 Pcs)", 46L, "https://mock-cloud-storage.com/images/products/2/3/glass_tumbler_set.png" },
                    { 99L, "Main image for artisan product: Custom Stained Glass Panel (Art Deco)", 47L, "https://mock-cloud-storage.com/images/products/2/3/stained_glass_panel.png" },
                    { 100L, "Main image for artisan product: Decorative Steel Hook (Wall Mount)", 48L, "https://mock-cloud-storage.com/images/products/2/4/decorative_steel_hook.png" },
                    { 101L, "Main image for artisan product: Forged Metal Bottle Rack (6 Bottles)", 49L, "https://mock-cloud-storage.com/images/products/2/4/forged_metal_bottle_rack.png" },
                    { 102L, "Main image for artisan product: Hand-Forged Fire Poker (Heavy Duty)", 50L, "https://mock-cloud-storage.com/images/products/2/4/hand_forged_fire_poker.png" },
                    { 103L, "Main image for artisan product: Hand-Forged Utility Hook (Rustic)", 51L, "https://mock-cloud-storage.com/images/products/2/4/hand_forged_hook.jpg" },
                    { 104L, "Main image for artisan product: Hand-Forged Abstract Sculpture (Desk Size)", 52L, "https://mock-cloud-storage.com/images/products/2/4/hand_forged_sculpture.png" },
                    { 105L, "Main image for artisan product: Custom Iron Grill Insert (Heavy Gauge)", 53L, "https://mock-cloud-storage.com/images/products/2/4/iron_grill.png" },
                    { 106L, "Main image for artisan product: Abstract Metal Wall Art Piece (Small)", 54L, "https://mock-cloud-storage.com/images/products/2/4/metal_art_piece.png" },
                    { 107L, "Main image for artisan product: Steel Bottle Opener (Keyring Style)", 55L, "https://mock-cloud-storage.com/images/products/2/4/steel_bottle_opener.png" },
                    { 108L, "Main image for artisan product: Wrought Iron Candlestick (Set of 2)", 56L, "https://mock-cloud-storage.com/images/products/2/4/wrought_iron_candlestick.png" },
                    { 109L, "Main image for artisan product: Wrought Iron Coasters (Set of 4)", 57L, "https://mock-cloud-storage.com/images/products/2/4/wrought_iron_coasters.png" },
                    { 110L, "Main image for artisan product: Elegant Amethyst Earrings (Sterling Silver)", 58L, "https://mock-cloud-storage.com/images/products/4/5/amethyst_earrings.png" },
                    { 111L, "Main image for artisan product: Colorful Glass Beaded Necklace (Long)", 59L, "https://mock-cloud-storage.com/images/products/4/5/beaded_necklace.png" },
                    { 112L, "Main image for artisan product: Sparkling Crystal Stud Earrings (Small)", 60L, "https://mock-cloud-storage.com/images/products/4/5/crystal_earrings.png" },
                    { 113L, "Main image for artisan product: Wide Polished Cuff Bracelet (Brass)", 61L, "https://mock-cloud-storage.com/images/products/4/5/cuff_bracelet.jpg" },
                    { 114L, "Main image for artisan product: Delicate Gold Plated Chain Necklace", 62L, "https://mock-cloud-storage.com/images/products/4/5/delicate_gold_chain.jpg" },
                    { 115L, "Main image for artisan product: Deep Red Garnet Pendant (Silver)", 63L, "https://mock-cloud-storage.com/images/products/4/5/garnet_pendant.png" },
                    { 116L, "Main image for artisan product: Multi-Gemstone Charm Bracelet", 64L, "https://mock-cloud-storage.com/images/products/4/5/gemstone_bracelet.png" },
                    { 117L, "Main image for artisan product: Vintage Freshwater Pearl Brooch", 65L, "https://mock-cloud-storage.com/images/products/4/5/pearl_brooch.png" },
                    { 118L, "Main image for artisan product: Zodiac Sign Pendant Charm (Custom)", 66L, "https://mock-cloud-storage.com/images/products/4/5/pendant_charm.png" },
                    { 119L, "Main image for artisan product: Minimalist Sterling Silver Ring", 67L, "https://mock-cloud-storage.com/images/products/4/5/silver_ring.png" },
                    { 120L, "Main image for artisan product: Bohemian Turquoise Inlaid Ring", 68L, "https://mock-cloud-storage.com/images/products/4/5/turquoise_ring.png" },
                    { 121L, "Main image for artisan product: Rustic Leather Journal Cover (A5)", 69L, "https://mock-cloud-storage.com/images/products/3/6/journal_cover.png" },
                    { 122L, "Detail 1 of Rustic Leather Journal Cover (A5).", 69L, "https://mock-cloud-storage.com/images/products/3/6/69_detail_1.jpg" },
                    { 123L, "Main image for artisan product: Customizable Leather Key Fob", 70L, "https://mock-cloud-storage.com/images/products/3/6/key_fob.png" },
                    { 124L, "Detail 1 of Customizable Leather Key Fob.", 70L, "https://mock-cloud-storage.com/images/products/3/6/70_detail_1.jpg" },
                    { 125L, "Main image for artisan product: Full-Grain Leather Belt (Classic Brown)", 71L, "https://mock-cloud-storage.com/images/products/3/6/leather_belt.png" },
                    { 126L, "Detail 1 of Full-Grain Leather Belt (Classic Brown).", 71L, "https://mock-cloud-storage.com/images/products/3/6/71_detail_1.jpg" },
                    { 127L, "Main image for artisan product: Slim Leather Card Holder (Minimalist)", 72L, "https://mock-cloud-storage.com/images/products/3/6/leather_card_holder.png" },
                    { 128L, "Detail 1 of Slim Leather Card Holder (Minimalist).", 72L, "https://mock-cloud-storage.com/images/products/3/6/72_detail_1.jpg" },
                    { 129L, "Main image for artisan product: Leather Cord Organizer (Set of 3)", 73L, "https://mock-cloud-storage.com/images/products/3/6/leather_cord_organizer.png" },
                    { 130L, "Detail 1 of Leather Cord Organizer (Set of 3).", 73L, "https://mock-cloud-storage.com/images/products/3/6/73_detail_1.jpg" },
                    { 131L, "Main image for artisan product: Leather Bound Journal Cover (Distressed)", 74L, "https://mock-cloud-storage.com/images/products/3/6/leather_journal_cover.jpg" },
                    { 132L, "Detail 1 of Leather Bound Journal Cover (Distressed).", 74L, "https://mock-cloud-storage.com/images/products/3/6/74_detail_1.jpg" },
                    { 133L, "Main image for artisan product: Leather Laptop Sleeve (13 inch)", 75L, "https://mock-cloud-storage.com/images/products/3/6/leather_laptop_sleeve.png" },
                    { 134L, "Detail 1 of Leather Laptop Sleeve (13 inch).", 75L, "https://mock-cloud-storage.com/images/products/3/6/75_detail_1.jpg" },
                    { 135L, "Main image for artisan product: Travel Leather Passport Holder", 76L, "https://mock-cloud-storage.com/images/products/3/6/leather_passport_holder.png" },
                    { 136L, "Detail 1 of Travel Leather Passport Holder.", 76L, "https://mock-cloud-storage.com/images/products/3/6/76_detail_1.jpg" },
                    { 137L, "Main image for artisan product: Bifold Leather Wallet (Classic Design)", 77L, "https://mock-cloud-storage.com/images/products/3/6/leather_wallet.png" },
                    { 138L, "Detail 1 of Bifold Leather Wallet (Classic Design).", 77L, "https://mock-cloud-storage.com/images/products/3/6/77_detail_1.jpg" },
                    { 139L, "Main image for artisan product: Large Leather Satchel Bag (Cross-body)", 78L, "https://mock-cloud-storage.com/images/products/3/6/satchel_bag.png" },
                    { 140L, "Detail 1 of Large Leather Satchel Bag (Cross-body).", 78L, "https://mock-cloud-storage.com/images/products/3/6/78_detail_1.jpg" },
                    { 141L, "Main image for artisan product: Artistic Hand-Painted Note Cards (Set of 10)", 79L, "https://mock-cloud-storage.com/images/products/5/7/artistic_note_cards.png" },
                    { 142L, "Detail 1 of Artistic Hand-Painted Note Cards (Set of 10).", 79L, "https://mock-cloud-storage.com/images/products/5/7/79_detail_1.jpg" },
                    { 143L, "Main image for artisan product: Traditional Calligraphy Starter Set", 80L, "https://mock-cloud-storage.com/images/products/5/7/calligraphy_set.png" },
                    { 144L, "Detail 1 of Traditional Calligraphy Starter Set.", 80L, "https://mock-cloud-storage.com/images/products/5/7/80_detail_1.jpg" },
                    { 145L, "Main image for artisan product: Bespoke Custom Stationery - Letterhead", 81L, "https://mock-cloud-storage.com/images/products/5/7/custom_stationery.png" },
                    { 146L, "Detail 1 of Bespoke Custom Stationery - Letterhead.", 81L, "https://mock-cloud-storage.com/images/products/5/7/81_detail_1.jpg" },
                    { 147L, "Main image for artisan product: Deluxe Custom Stationery Set (Monogrammed)", 82L, "https://mock-cloud-storage.com/images/products/5/7/custom_stationery_set.jpg" },
                    { 148L, "Detail 1 of Deluxe Custom Stationery Set (Monogrammed).", 82L, "https://mock-cloud-storage.com/images/products/5/7/82_detail_1.jpg" },
                    { 149L, "Main image for artisan product: Premium Custom Stationery Set (Professional)", 83L, "https://mock-cloud-storage.com/images/products/5/7/custom_stationery_set_1.jpg" },
                    { 150L, "Detail 1 of Premium Custom Stationery Set (Professional).", 83L, "https://mock-cloud-storage.com/images/products/5/7/83_detail_1.jpg" },
                    { 151L, "Main image for artisan product: Hand-Bound Linen Journal (Lined)", 84L, "https://mock-cloud-storage.com/images/products/5/7/hand_bound_journal.png" },
                    { 152L, "Detail 1 of Hand-Bound Linen Journal (Lined).", 84L, "https://mock-cloud-storage.com/images/products/5/7/84_detail_1.jpg" },
                    { 153L, "Main image for artisan product: Handmade Paper Envelopes (Mixed Colors)", 85L, "https://mock-cloud-storage.com/images/products/5/7/handmade_envelopes.png" },
                    { 154L, "Detail 1 of Handmade Paper Envelopes (Mixed Colors).", 85L, "https://mock-cloud-storage.com/images/products/5/7/85_detail_1.jpg" },
                    { 155L, "Main image for artisan product: Fountain Pen and Ink Set (Beginner's)", 86L, "https://mock-cloud-storage.com/images/products/5/7/ink_pen_set.png" },
                    { 156L, "Detail 1 of Fountain Pen and Ink Set (Beginner's).", 86L, "https://mock-cloud-storage.com/images/products/5/7/86_detail_1.jpg" },
                    { 157L, "Main image for artisan product: Limited Edition Letterpress Prints (A4)", 87L, "https://mock-cloud-storage.com/images/products/5/7/letterpress_prints.png" },
                    { 158L, "Detail 1 of Limited Edition Letterpress Prints (A4).", 87L, "https://mock-cloud-storage.com/images/products/5/7/87_detail_1.jpg" },
                    { 159L, "Main image for artisan product: Personalized Brass Wax Seal Kit", 88L, "https://mock-cloud-storage.com/images/products/5/7/personalized_wax_seal.png" },
                    { 160L, "Detail 1 of Personalized Brass Wax Seal Kit.", 88L, "https://mock-cloud-storage.com/images/products/5/7/88_detail_1.jpg" },
                    { 161L, "Main image for artisan product: Custom Embroidered Canvas Tote Bag", 89L, "https://mock-cloud-storage.com/images/products/1/11/custom_embroined_bag.png" },
                    { 162L, "Detail 1 of Custom Embroidered Canvas Tote Bag.", 89L, "https://mock-cloud-storage.com/images/products/1/11/89_detail_1.jpg" },
                    { 163L, "Main image for artisan product: Hand-Stitched Abstract Tapestry (Medium)", 90L, "https://mock-cloud-storage.com/images/products/1/11/hand_stitched_tapestry.png" },
                    { 164L, "Detail 1 of Hand-Stitched Abstract Tapestry (Medium).", 90L, "https://mock-cloud-storage.com/images/products/1/11/90_detail_1.jpg" },
                    { 165L, "Main image for artisan product: Handmade Quilted Throw Blanket (Cotton)", 91L, "https://mock-cloud-storage.com/images/products/1/11/quilted_blanket.png" },
                    { 166L, "Detail 1 of Handmade Quilted Throw Blanket (Cotton).", 91L, "https://mock-cloud-storage.com/images/products/1/11/91_detail_1.jpg" },
                    { 167L, "Main image for artisan product: Woven Embroidered Iron-On Patch (Custom)", 92L, "https://mock-cloud-storage.com/images/products/1/11/embroidered_patch.png" },
                    { 168L, "Detail 1 of Woven Embroidered Iron-On Patch (Custom).", 92L, "https://mock-cloud-storage.com/images/products/1/11/92_detail_1.jpg" },
                    { 169L, "Main image for artisan product: Handmade Fabric Coasters (Set of 6)", 93L, "https://mock-cloud-storage.com/images/products/1/11/handmade_fabric_coasters.png" },
                    { 170L, "Detail 1 of Handmade Fabric Coasters (Set of 6).", 93L, "https://mock-cloud-storage.com/images/products/1/11/93_detail_1.jpg" },
                    { 171L, "Main image for artisan product: Silk Ribbon Embroidery Art (Framed)", 94L, "https://mock-cloud-storage.com/images/products/1/11/silk_ribbon_art.png" },
                    { 172L, "Detail 1 of Silk Ribbon Embroidery Art (Framed).", 94L, "https://mock-cloud-storage.com/images/products/1/11/94_detail_1.jpg" },
                    { 173L, "Main image for artisan product: Embroidered Wall Hoop Art (Botanical)", 95L, "https://mock-cloud-storage.com/images/products/1/11/embroidered_wall_hoop.png" },
                    { 174L, "Detail 1 of Embroidered Wall Hoop Art (Botanical).", 95L, "https://mock-cloud-storage.com/images/products/1/11/95_detail_1.jpg" },
                    { 175L, "Main image for artisan product: Abstract Textile Wall Art (Woven/Mixed Media)", 96L, "https://mock-cloud-storage.com/images/products/1/11/textile_art.png" },
                    { 176L, "Detail 1 of Abstract Textile Wall Art (Woven/Mixed Media).", 96L, "https://mock-cloud-storage.com/images/products/1/11/96_detail_1.jpg" },
                    { 177L, "Main image for artisan product: Delicate Floral Embroidered Hoop (Large)", 97L, "https://mock-cloud-storage.com/images/products/1/11/floral_embroidered_hoop.jpg" },
                    { 178L, "Detail 1 of Delicate Floral Embroidered Hoop (Large).", 97L, "https://mock-cloud-storage.com/images/products/1/11/97_detail_1.jpg" },
                    { 179L, "Main image for artisan product: Patterned Throw Cushion (Hand-Sewn)", 98L, "https://mock-cloud-storage.com/images/products/1/11/patterned_cushion.png" },
                    { 180L, "Detail 1 of Patterned Throw Cushion (Hand-Sewn).", 98L, "https://mock-cloud-storage.com/images/products/1/11/98_detail_1.jpg" },
                    { 181L, "Main image for artisan product: Abstract Hardwood Sculpture", 99L, "https://mock-cloud-storage.com/images/products/3/8/abstract_wood_sculpture.jpg" },
                    { 182L, "Detail 1 of Abstract Hardwood Sculpture.", 99L, "https://mock-cloud-storage.com/images/products/3/8/99_detail_1.jpg" },
                    { 183L, "Main image for artisan product: Carved Wooden Figurative Statue", 100L, "https://mock-cloud-storage.com/images/products/3/8/carved_wood_statue.png" },
                    { 184L, "Detail 1 of Carved Wooden Figurative Statue.", 100L, "https://mock-cloud-storage.com/images/products/3/8/100_detail_1.jpg" },
                    { 185L, "Main image for artisan product: Wooden Coasters Set (4 pieces)", 101L, "https://mock-cloud-storage.com/images/products/3/8/coasters.png" },
                    { 186L, "Detail 1 of Wooden Coasters Set (4 pieces).", 101L, "https://mock-cloud-storage.com/images/products/3/8/101_detail_1.jpg" },
                    { 187L, "Main image for artisan product: Decorative Wooden Keepsake Box", 102L, "https://mock-cloud-storage.com/images/products/3/8/decorative_box.png" },
                    { 188L, "Detail 1 of Decorative Wooden Keepsake Box.", 102L, "https://mock-cloud-storage.com/images/products/3/8/102_detail_1.jpg" },
                    { 189L, "Main image for artisan product: Hand-Carved Wooden Serving Spoon", 103L, "https://mock-cloud-storage.com/images/products/3/8/hand_carved_spoon.png" },
                    { 190L, "Detail 1 of Hand-Carved Wooden Serving Spoon.", 103L, "https://mock-cloud-storage.com/images/products/3/8/103_detail_1.jpg" },
                    { 191L, "Main image for artisan product: Hand-Carved Wooden Whistle", 104L, "https://mock-cloud-storage.com/images/products/3/8/hand_carved_whistle.png" },
                    { 192L, "Detail 1 of Hand-Carved Wooden Whistle.", 104L, "https://mock-cloud-storage.com/images/products/3/8/104_detail_1.jpg" },
                    { 193L, "Main image for artisan product: Wood Turned Salt Cellar", 105L, "https://mock-cloud-storage.com/images/products/3/8/wood_turned_salt_cellar.png" },
                    { 194L, "Detail 1 of Wood Turned Salt Cellar.", 105L, "https://mock-cloud-storage.com/images/products/3/8/105_detail_1.jpg" },
                    { 195L, "Main image for artisan product: Large Natural Wooden Serving Bowl", 106L, "https://mock-cloud-storage.com/images/products/3/8/wooden_bowl.png" },
                    { 196L, "Detail 1 of Large Natural Wooden Serving Bowl.", 106L, "https://mock-cloud-storage.com/images/products/3/8/106_detail_1.jpg" },
                    { 197L, "Main image for artisan product: Artisan Block Wooden Cutting Board", 107L, "https://mock-cloud-storage.com/images/products/3/8/wooden_cutting_board.png" },
                    { 198L, "Detail 1 of Artisan Block Wooden Cutting Board.", 107L, "https://mock-cloud-storage.com/images/products/3/8/107_detail_1.jpg" },
                    { 199L, "Main image for artisan product: Rustic Wooden Serving Board for Cheeses and Snacks", 108L, "https://mock-cloud-storage.com/images/products/3/8/wooden_serving_board.jpg" },
                    { 200L, "Detail 1 of Rustic Wooden Serving Board for Cheeses and Snacks.", 108L, "https://mock-cloud-storage.com/images/products/3/8/108_detail_1.jpg" },
                    { 201L, "Main image for artisan product: Elegant Wooden Serving Tray", 109L, "https://mock-cloud-storage.com/images/products/3/8/wooden_tray.png" },
                    { 202L, "Detail 1 of Elegant Wooden Serving Tray.", 109L, "https://mock-cloud-storage.com/images/products/3/8/109_detail_1.jpg" },
                    { 203L, "Main image for artisan product: Hand-Woven Bamboo Fruit Bowl", 110L, "https://mock-cloud-storage.com/images/products/3/9/bamboo_fruit_bowl.jpg" },
                    { 204L, "Detail 1 of Hand-Woven Bamboo Fruit Bowl.", 110L, "https://mock-cloud-storage.com/images/products/3/9/110_detail_1.jpg" },
                    { 205L, "Main image for artisan product: Round Rattan Decorative Tray (Small)", 111L, "https://mock-cloud-storage.com/images/products/3/9/decorative_tray.png" },
                    { 206L, "Detail 1 of Round Rattan Decorative Tray (Small).", 111L, "https://mock-cloud-storage.com/images/products/3/9/111_detail_1.jpg" },
                    { 207L, "Main image for artisan product: Large Rattan Storage Basket with Lid", 112L, "https://mock-cloud-storage.com/images/products/3/9/rattan_basket.jpg" },
                    { 208L, "Detail 1 of Large Rattan Storage Basket with Lid.", 112L, "https://mock-cloud-storage.com/images/products/3/9/112_detail_1.jpg" },
                    { 209L, "Main image for artisan product: Set of 6 Rattan Coasters", 113L, "https://mock-cloud-storage.com/images/products/3/9/rattan_coasters.png" },
                    { 210L, "Detail 1 of Set of 6 Rattan Coasters.", 113L, "https://mock-cloud-storage.com/images/products/3/9/113_detail_1.jpg" },
                    { 211L, "Main image for artisan product: Modern Rattan Table Lamp Base", 114L, "https://mock-cloud-storage.com/images/products/3/9/rattan_lamp.png" },
                    { 212L, "Detail 1 of Modern Rattan Table Lamp Base.", 114L, "https://mock-cloud-storage.com/images/products/3/9/114_detail_1.jpg" },
                    { 213L, "Main image for artisan product: Woven Rope Plant Pot Cover", 115L, "https://mock-cloud-storage.com/images/products/3/9/rope_planter.png" },
                    { 214L, "Detail 1 of Woven Rope Plant Pot Cover.", 115L, "https://mock-cloud-storage.com/images/products/3/9/115_detail_1.jpg" },
                    { 215L, "Main image for artisan product: Round Hand-Braided Seagrass Mat (60cm)", 116L, "https://mock-cloud-storage.com/images/products/3/9/seagrass_mat.png" },
                    { 216L, "Detail 1 of Round Hand-Braided Seagrass Mat (60cm).", 116L, "https://mock-cloud-storage.com/images/products/3/9/116_detail_1.jpg" },
                    { 217L, "Main image for artisan product: Classic Wicker Laundry Basket", 117L, "https://mock-cloud-storage.com/images/products/3/9/wicker_basket.png" },
                    { 218L, "Detail 1 of Classic Wicker Laundry Basket.", 117L, "https://mock-cloud-storage.com/images/products/3/9/117_detail_1.jpg" },
                    { 219L, "Main image for artisan product: Geometric Wicker Pendant Lamp Shade", 118L, "https://mock-cloud-storage.com/images/products/3/9/wicker_pendant_lamp.jpg" },
                    { 220L, "Detail 1 of Geometric Wicker Pendant Lamp Shade.", 118L, "https://mock-cloud-storage.com/images/products/3/9/118_detail_1.jpg" },
                    { 221L, "Main image for artisan product: Hand-Woven Rattan Accent Chair", 119L, "https://mock-cloud-storage.com/images/products/3/9/woven_chair.png" },
                    { 222L, "Detail 1 of Hand-Woven Rattan Accent Chair.", 119L, "https://mock-cloud-storage.com/images/products/3/9/119_detail_1.jpg" },
                    { 223L, "Main image for artisan product: Woven Seagrass Storage Box (Rectangular)", 120L, "https://mock-cloud-storage.com/images/products/3/9/woven_storage_box.jpg" },
                    { 224L, "Detail 1 of Woven Seagrass Storage Box (Rectangular).", 120L, "https://mock-cloud-storage.com/images/products/3/9/120_detail_1.jpg" },
                    { 225L, "Main image for artisan product: Abstract Woven Wall Decor Piece", 121L, "https://mock-cloud-storage.com/images/products/3/9/woven_wall_decor.png" },
                    { 226L, "Detail 1 of Abstract Woven Wall Decor Piece.", 121L, "https://mock-cloud-storage.com/images/products/3/9/121_detail_1.jpg" },
                    { 227L, "Main image for artisan product: Bohemian Woven Wall Hanging (Fringed)", 122L, "https://mock-cloud-storage.com/images/products/3/9/woven_wall_hanging.jpg" },
                    { 228L, "Detail 1 of Bohemian Woven Wall Hanging (Fringed).", 122L, "https://mock-cloud-storage.com/images/products/3/9/122_detail_1.jpg" },
                    { 229L, "Main image for artisan product: Hand-Knit Chunky Throw Blanket (Merino)", 123L, "https://mock-cloud-storage.com/images/products/5/10/chunky_throw_blanket.jpg" },
                    { 230L, "Detail 1 of Hand-Knit Chunky Throw Blanket (Merino).", 123L, "https://mock-cloud-storage.com/images/products/5/10/123_detail_1.jpg" },
                    { 231L, "Main image for artisan product: Hand-Felted Wool Slippers (Indoor)", 124L, "https://mock-cloud-storage.com/images/products/5/10/felted_slippers.png" },
                    { 232L, "Detail 1 of Hand-Felted Wool Slippers (Indoor).", 124L, "https://mock-cloud-storage.com/images/products/5/10/124_detail_1.jpg" },
                    { 233L, "Main image for artisan product: Hand-Knit Fingerless Wool Gloves", 125L, "https://mock-cloud-storage.com/images/products/5/10/hand-knit_gloves.png" },
                    { 234L, "Detail 1 of Hand-Knit Fingerless Wool Gloves.", 125L, "https://mock-cloud-storage.com/images/products/5/10/125_detail_1.jpg" },
                    { 235L, "Main image for artisan product: Long Hand-Knitted Scarf (Alpaca Blend)", 126L, "https://mock-cloud-storage.com/images/products/5/10/hand_knitted_scarf.jpg" },
                    { 236L, "Detail 1 of Long Hand-Knitted Scarf (Alpaca Blend).", 126L, "https://mock-cloud-storage.com/images/products/5/10/126_detail_1.jpg" },
                    { 237L, "Main image for artisan product: Whimsical Knitted Dog Sweater (Various Sizes)", 127L, "https://mock-cloud-storage.com/images/products/5/10/knitted_dog_sweater.png" },
                    { 238L, "Detail 1 of Whimsical Knitted Dog Sweater (Various Sizes).", 127L, "https://mock-cloud-storage.com/images/products/5/10/127_detail_1.jpg" },
                    { 239L, "Main image for artisan product: Classic Knitted Wool Scarf (Plaid)", 128L, "https://mock-cloud-storage.com/images/products/5/10/knitted_scarf.png" },
                    { 240L, "Detail 1 of Classic Knitted Wool Scarf (Plaid).", 128L, "https://mock-cloud-storage.com/images/products/5/10/128_detail_1.jpg" },
                    { 241L, "Main image for artisan product: Handmade Oversized Knitted Sweater (Unisex)", 129L, "https://mock-cloud-storage.com/images/products/5/10/knitted_sweater.png" },
                    { 242L, "Detail 1 of Handmade Oversized Knitted Sweater (Unisex).", 129L, "https://mock-cloud-storage.com/images/products/5/10/129_detail_1.jpg" },
                    { 243L, "Main image for artisan product: Soft Knit Wool Beanie Hat (Slouchy Fit)", 130L, "https://mock-cloud-storage.com/images/products/5/10/wool_beanie.png" },
                    { 244L, "Detail 1 of Soft Knit Wool Beanie Hat (Slouchy Fit).", 130L, "https://mock-cloud-storage.com/images/products/5/10/130_detail_1.jpg" },
                    { 245L, "Main image for artisan product: Pure Wool Throw Blanket (Geometric Pattern)", 131L, "https://mock-cloud-storage.com/images/products/5/10/wool_blanket.png" },
                    { 246L, "Detail 1 of Pure Wool Throw Blanket (Geometric Pattern).", 131L, "https://mock-cloud-storage.com/images/products/5/10/131_detail_1.jpg" },
                    { 247L, "Main image for artisan product: Hand-Knit Wool Headband/Ear Warmer", 132L, "https://mock-cloud-storage.com/images/products/5/10/wool_headband.png" },
                    { 248L, "Detail 1 of Hand-Knit Wool Headband/Ear Warmer.", 132L, "https://mock-cloud-storage.com/images/products/5/10/132_detail_1.jpg" },
                    { 249L, "Main image for artisan product: Thick Woolen Bed Socks (Crew Length)", 133L, "https://mock-cloud-storage.com/images/products/5/10/woolen_socks.png" },
                    { 250L, "Detail 1 of Thick Woolen Bed Socks (Crew Length).", 133L, "https://mock-cloud-storage.com/images/products/5/10/133_detail_1.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cart_client_id",
                schema: "public",
                table: "cart",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_product_id",
                schema: "public",
                table: "cart_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_client_id",
                schema: "public",
                table: "orders",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_currency_code",
                schema: "public",
                table: "orders",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "IX_orders_products_products_product_id",
                schema: "public",
                table: "orders_products",
                column: "products_product_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_order_id",
                schema: "public",
                table: "payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_image_product_id",
                schema: "public",
                table: "product_image",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                schema: "public",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_seller_id",
                schema: "public",
                table: "products",
                column: "seller_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "orders_products",
                schema: "public");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "product_image",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cart",
                schema: "public");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "public");

            migrationBuilder.DropTable(
                name: "products",
                schema: "public");

            migrationBuilder.DropTable(
                name: "clients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "currencies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "sellers",
                schema: "public");
        }
    }
}
