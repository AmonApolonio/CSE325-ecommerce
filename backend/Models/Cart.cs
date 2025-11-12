using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Necessário para ICollection

namespace backend.Models
{
    // Sets the table name and schema if it differs from the default (optional, but recommended for clarity)
    [Table("cart", Schema = "public")]
    public class Cart
    {
        // cart_id bigint NOT NULL
        [Key]
        [Column("cart_id")]
        public long CartId { get; set; }

        // client_id bigint NOT NULL (Renomeado de user_id)
        [Column("client_id")]
        public long ClientId { get; set; }

        // created_date date NOT NULL
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        // updated_date date (Can be NULL)
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        // status public.cart_status (Custom SQL ENUM type)
        [Column("status")]
        public CartStatus Status { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)
        
        // 🔑 1. Relacionamento Um-para-Um: Cart possui UM Client
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
        
        // 🔑 2. Relacionamento Um-para-Muitos: Cart possui MÚLTIPLOS CartItems (Items)
        // Isso resolve o erro CS1061: 'Cart' does not contain a definition for 'Items'
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}