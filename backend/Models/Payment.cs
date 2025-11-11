using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.payments)
    [Table("payments", Schema = "public")]
    public class Payment
    {
        // payment_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assumes it is auto-generated in the DB
        [Column("payment_id")]
        public long PaymentId { get; set; }

        // order_id bigint NOT NULL (Foreign Key)
        [Required]
        [Column("order_id")]
        public long OrderId { get; set; }

        // payment_date date NOT NULL
        [Required]
        [Column("payment_date")]
        public DateTime PaymentDate { get; set; }

        // amount numeric NOT NULL
        // Mapped to decimal for precision
        [Required]
        [Column("amount", TypeName = "numeric")]
        public decimal Amount { get; set; }

        // payment_method character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("payment_method")]
        public required string PaymentMethod { get; set; } // Using C# 11 'required' for NRT

        // transaction_status public.payment_status DEFAULT 'Pending' NOT NULL
        [Required]
        [Column("transaction_status")]
        // Uses PaymentStatus enum and sets the default value
        public PaymentStatus TransactionStatus { get; set; } = PaymentStatus.Pending; 

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)

        // Navigation property to the Order model
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;
    }
}