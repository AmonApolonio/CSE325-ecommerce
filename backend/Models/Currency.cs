using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.currencies)
    [Table("currencies", Schema = "public")]
    public class Currency
    {
        // currency_code character varying(3) NOT NULL (Primary Key)
        [Key]
        [StringLength(3)]
        [Column("currency_code")]
        public required string CurrencyCode { get; set; } // Using C# 11 'required' for NRT

        // name character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("name")]
        public required string Name { get; set; } // Using C# 11 'required' for NRT

        // symbol character varying(5) NOT NULL
        [Required]
        [StringLength(5)]
        [Column("symbol")]
        public required string Symbol { get; set; } // Using C# 11 'required' for NRT

        // -------------------------------------------------------------
        // Navigation Properties (Optional: used to reference this currency from other models)
        
        // Example: List of Orders using this currency code
        // public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}