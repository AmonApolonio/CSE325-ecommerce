using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the join table between Orders and Products (also known as Order Detail)
    [Table("orders_products", Schema = "public")]
    public class OrderProduct
    {
        // orders_order_id bigint NOT NULL (Part of Composite Primary Key and Foreign Key to Order)
        [Required]
        [Column("orders_order_id")]
        public long OrderId { get; set; }

        // products_product_id bigint NOT NULL (Part of Composite Primary Key and Foreign Key to Product)
        [Required]
        [Column("products_product_id")]
        public long ProductId { get; set; }

        // quantity numeric NOT NULL
        [Required]
        [Column("quantity", TypeName = "numeric")] // Mapped to decimal for precision
        public decimal Quantity { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)
        
        // Navigation property to the parent Order
        public Order Order { get; set; } = null!;

        // Navigation property to the related Product
        public Product Product { get; set; } = null!;
    }
}