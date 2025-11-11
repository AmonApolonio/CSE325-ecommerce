using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.orders)
    [Table("orders", Schema = "public")]
    public class Order
    {
        // order_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assumes it is auto-generated in the DB
        [Column("order_id")]
        public long OrderId { get; set; }

        // client_id bigint NOT NULL (Foreign Key)
        [Required]
        [Column("client_id")]
        public long ClientId { get; set; }

        // sub_total_cents numeric NOT NULL
        // Mapped to decimal for precision (stores value in cents to avoid floating point issues)
        [Required]
        [Column("sub_total_cents", TypeName = "numeric")]
        public decimal SubTotalCents { get; set; }

        // tax_cents numeric NOT NULL
        [Required]
        [Column("tax_cents", TypeName = "numeric")]
        public decimal TaxCents { get; set; }

        // freight_cents numeric (Nullable)
        [Column("freight_cents", TypeName = "numeric")]
        public decimal? FreightCents { get; set; } // Nullable decimal

        // created_at date NOT NULL
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // updated_at date (Nullable)
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } // Nullable DateTime

        // status public.order_status DEFAULT 'Pending Payment' NOT NULL
        [Required]
        [Column("status")]
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment; // Uses OrderStatus enum and sets default

        // currency_code character varying(3) DEFAULT 'BRL' NOT NULL
        [Required]
        [StringLength(3)]
        [Column("currency_code")]
        public required string CurrencyCode { get; set; } = "BRL"; // Uses C# 'required' and sets default

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)

        // Navigation property to the Client model
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
    }
}