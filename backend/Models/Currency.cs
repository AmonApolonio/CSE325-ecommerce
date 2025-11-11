using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // 💡 Adicionar para ICollection

namespace backend.Models
{
    // Defines the table name and schema (public.currencies)
    [Table("currencies", Schema = "public")]
    public class Currency
    {
        // -------------------------------------------------------------
        // Properties (Columns)
        // -------------------------------------------------------------
        
        // currency_code character varying(3) NOT NULL (Primary Key)
        [Key]
        [StringLength(3)]
        [Column("currency_code")]
        public required string CurrencyCode { get; set; }

        // name character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("name")]
        public required string Name { get; set; }

        // symbol character varying(5) NOT NULL
        [Required]
        [StringLength(5)]
        [Column("symbol")]
        public required string Symbol { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)
        // -------------------------------------------------------------
        
        // 🔑 PROPRIEDADE FALTANTE/COMENTADA: Coleção de Pedidos (Orders)
        // Isso resolve o erro CS1061. Uma moeda pode ser usada em VÁRIOS pedidos.
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}