using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; 

namespace backend.Models
{
    // Defines the table name and schema
    [Table("products", Schema = "public")]
    public class Product
    {
        // -------------------------------------------------------------
        // Properties (Columns)
        // -------------------------------------------------------------

        // product_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_id")]
        public long ProductId { get; set; }

        // name character varying(200) NOT NULL
        [Required]
        [StringLength(200)]
        [Column("name")]
        public required string Name { get; set; }

        // description text NOT NULL
        [Required]
        [Column("description")]
        public required string Description { get; set; }

        // price integer DEFAULT 0 NOT NULL
        [Required]
        [Column("price")]
        public int Price { get; set; } = 0; // Armazenado como inteiro (ex: centavos)

        // inventory numeric DEFAULT 0 NOT NULL
        [Required]
        [Column("inventory", TypeName = "numeric")]
        public decimal Inventory { get; set; } = 0; // Agora é 'Inventory'

        // category_id bigint DEFAULT 0 NOT NULL (Foreign Key)
        [Required]
        [Column("category_id")]
        public long CategoryId { get; set; } = 0; // Agora é 'long'

        // seller_id bigint NOT NULL (Foreign Key)
        [Required]
        [Column("seller_id")]
        public long SellerId { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)
        // -------------------------------------------------------------

        // Navigation property to the Category model
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; } = null!;

        // Navigation property to the Seller model
        [ForeignKey(nameof(SellerId))]
        public Seller? Seller { get; set; } = null!;
        
        // Collection for the Many-to-Many relationship with Order via OrderProduct
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        // Collection for the One-to-Many relationship with CartItem
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Collection for the One-to-Many relationship with ProductImage
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}