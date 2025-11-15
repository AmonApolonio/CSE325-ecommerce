using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Adicionar para ICollection

namespace backend.Models
{
    // Define o nome da tabela e o esquema (public.currencies)
    [Table("currencies", Schema = "public")]
    public class Currency
    {
        // -------------------------------------------------------------
        // Propriedades (Colunas)
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

        // exchange_rate_to_brl numeric NOT NULL
        // CAMPO OBRIGATÓRIO: Armazena a taxa de câmbio em relação à moeda base (BRL).
        [Required]
        [Column("exchange_rate_to_brl", TypeName = "numeric")]
        public decimal ExchangeRateToBRL { get; set; } = 1.00m; 

        // -------------------------------------------------------------
        // Propriedades de Navegação (Relacionamentos)
        // -------------------------------------------------------------
        
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}