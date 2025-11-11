using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.product_image)
    [Table("product_image", Schema = "public")]
    public class ProductImage
    {
        // product_image_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assumes it is auto-generated in the DB
        [Column("product_image_id")]
        public long ProductImageId { get; set; }

        // product_id bigint NOT NULL (Foreign Key)
        [Required]
        [Column("product_id")]
        public long ProductId { get; set; }

        // url character varying(150) NOT NULL
        [Required]
        [StringLength(150)]
        [Column("url")]
        public required string Url { get; set; } // Using C# 11 'required' for NRT

        // alt character varying(200) NOT NULL
        [Required]
        [StringLength(200)]
        [Column("alt")]
        public required string Alt { get; set; } // Using C# 11 'required' for NRT

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)

        // Navigation property back to the parent Product model
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}