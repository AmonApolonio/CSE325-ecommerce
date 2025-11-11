using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.cart_items)
    [Table("cart_items", Schema = "public")]
    public class CartItem
    {
        // cart_item bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assumes it is auto-generated in the DB
        [Column("cart_item")]
        public long CartItemId { get; set; }

        // cart_id bigint NOT NULL (Foreign Key to Cart)
        [Required]
        [Column("cart_id")]
        public long CartId { get; set; }

        // product_id bigint NOT NULL (Foreign Key to Product)
        [Required]
        [Column("product_id")]
        public long ProductId { get; set; }

        // quantity numeric NOT NULL
        [Required]
        [Column("quantity", TypeName = "numeric")] // Mapped to decimal for precision
        public decimal Quantity { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)

        // Navigation property to the parent Cart model (Assuming Cart.cs exists)
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; } = null!;

        // Navigation property to the parent Product model
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}