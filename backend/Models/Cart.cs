using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Sets the table name and schema if it differs from the default (optional, but recommended for clarity)
    [Table("cart", Schema = "public")]
    public class Cart
    {
        // cart_id bigint NOT NULL
        // [Key] defines this property as the Primary Key (PK)
        [Key]
        [Column("cart_id")]
        public long CartId { get; set; }

        // user_id bigint NOT NULL
        [Column("user_id")]
        public long UserId { get; set; }

        // created_date date NOT NULL
        // Mapped to DateTime in C#
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        // updated_date date (Can be NULL)
        // The question mark (?) indicates that the type is nullable
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        // status public.cart_status (Custom SQL ENUM type)
        [Column("status")]
        public CartStatus Status { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Optional, but common in ORMs like EF Core)
        // [ForeignKey(nameof(UserId))]
        // public User User { get; set; }
    }
}