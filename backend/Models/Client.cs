using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; 

namespace backend.Models
{
    // Defines the table name and schema (public.clients)
    [Table("clients", Schema = "public")]
    public class Client
    {
        // -------------------------------------------------------------
        // Properties (Columns)
        // -------------------------------------------------------------

        // user_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_id")]
        public long UserId { get; set; }

        // name character varying(255) NOT NULL
        [Required]
        [StringLength(255)]
        [Column("name")]
        public required string Name { get; set; }

        // email character varying(100) NOT NULL
        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column("email")]
        public required string Email { get; set; }

        // password_hash character varying(500) NOT NULL
        [Required]
        [StringLength(500)]
        [Column("password_hash")]
        public required string PasswordHash { get; set; }

        // phone_number character varying(50) (Nullable)
        [StringLength(50)]
        [Column("phone_number")]
        public string? PhoneNumber { get; set; }

        // address1 character varying(255) NOT NULL
        [Required]
        [StringLength(255)]
        [Column("address1")]
        public required string Address1 { get; set; }

        // address2 character varying(255) (Nullable)
        [StringLength(255)]
        [Column("address2")]
        public string? Address2 { get; set; }

        // city character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("city")]
        public required string City { get; set; }

        // state character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("state")]
        public required string State { get; set; }

        // country character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("country")]
        public required string Country { get; set; }

        // zip_code character varying(20) (Nullable)
        [StringLength(20)]
        [Column("zip_code")]
        public string? ZipCode { get; set; }

        // -------------------------------------------------------------
        // Navigation Properties (Relationships)
        // -------------------------------------------------------------

        // 🔑 PROPRIEDADE FALTANTE: Coleção de Pedidos (Orders)
        // Isso resolve o erro CS1061, mapeando o relacionamento Client (One) para Order (Many).
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}