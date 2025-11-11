using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // 💡 Necessário para ICollection

namespace backend.Models
{
    // Defines the table name and schema (public.orders)
    [Table("orders", Schema = "public")]
    public class Order
    {
        // -------------------------------------------------------------
        // Properties (Columns)
        // -------------------------------------------------------------

        // order_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("order_id")]
        public long OrderId { get; set; }

        // client_id bigint NOT NULL (Foreign Key)
        [Required]
        [Column("client_id")]
        public long ClientId { get; set; }

        // sub_total_cents numeric NOT NULL
        [Required]
        [Column("sub_total_cents", TypeName = "numeric")]
        public decimal SubTotalCents { get; set; }

        // tax_cents numeric NOT NULL
        [Required]
        [Column("tax_cents", TypeName = "numeric")]
        public decimal TaxCents { get; set; }

        // freight_cents numeric (Nullable)
        [Column("freight_cents", TypeName = "numeric")]
        public decimal? FreightCents { get; set; }

        // created_at date NOT NULL
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // updated_at date (Nullable)
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // status public.order_status DEFAULT 'Pending Payment' NOT NULL
        [Required]
        [Column("status")]
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

        // currency_code character varying(3) DEFAULT 'BRL' NOT NULL
        [Required]
        [StringLength(3)]
        [Column("currency_code")]
        public required string CurrencyCode { get; set; } = "BRL";

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)

        // Navigation property to the Client model (One-to-Many: Client -> Orders)
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
        
        // 🔑 1. COLEÇÃO FALTANTE: Pagamentos (One-to-Many: Order -> Payments)
        // Isso é necessário para configurar corretamente a linha em Payment: WithMany(o => o.Payments)
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        // 🔑 2. COLEÇÃO FALTANTE: Produtos do Pedido (Many-to-Many via OrderProduct)
        // Isso é necessário para configurar corretamente a linha no DbContext: WithMany(o => o.OrderProducts)
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}