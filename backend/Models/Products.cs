using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.products)
    [Table("products", Schema = "public")]
    public class Product
    {
        // product_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assumes it is auto-generated in the DB
        [Column("product_id")]
        public long ProductId { get; set; }

        // name character varying(200) NOT NULL
        [Required]
        [StringLength(200)]
        [Column("name")]
        public required string Name { get; set; } // Using C# 11 'required' for NRT

        // description text NOT NULL
        [Required]
        [Column("description")]
        public required string Description { get; set; } // Mapped to string (text type)

        // price integer DEFAULT 0 NOT NULL
        [Required]
        [Column("price")]
        public int Price { get; set; } = 0; // Mapped to int, set default value

        // inventory numeric DEFAULT 0 NOT NULL
        [Required]
        [Column("inventory", TypeName = "numeric")] // Mapped to decimal for precision
        public decimal Inventory { get; set; } = 0; // Set default value

        // category_id bigint DEFAULT 0 NOT NULL (Foreign Key)
        [Required]
        [Column("category_id")]
        public long CategoryId { get; set; } = 0; // Set default value

        // seller_id bigint NOT NULL (Foreign Key)
        [Required]
        [Column("seller_id")]
        public long SellerId { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)

        // Navigation property to the Category model
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        // Navigation property to the Seller model
        [ForeignKey(nameof(SellerId))]
        public Seller Seller { get; set; } = null!;
    }
}