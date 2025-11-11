using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.clients)
    [Table("clients", Schema = "public")]
    public class Client
    {
        // user_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assumes it is auto-generated in the DB
        [Column("user_id")]
        public long UserId { get; set; }

        // name character varying(255) NOT NULL
        [Required]
        [StringLength(255)]
        [Column("name")]
        public required string Name { get; set; } // Using C# 11 'required' for Non-Nullable Reference Types (NRT)

        // email character varying(100) NOT NULL
        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column("email")]
        public required string Email { get; set; } // Using C# 11 'required' for NRT

        // password_hash character varying(500) NOT NULL
        [Required]
        [StringLength(500)]
        [Column("password_hash")]
        public required string PasswordHash { get; set; } // Using C# 11 'required' for NRT

        // phone_number character varying(50) (Nullable)
        [StringLength(50)]
        [Column("phone_number")]
        public string? PhoneNumber { get; set; } // string? indicates nullable

        // address1 character varying(255) NOT NULL
        [Required]
        [StringLength(255)]
        [Column("address1")]
        public required string Address1 { get; set; } // Using C# 11 'required' for NRT

        // address2 character varying(255) (Nullable)
        [StringLength(255)]
        [Column("address2")]
        public string? Address2 { get; set; } // string? indicates nullable

        // city character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("city")]
        public required string City { get; set; } // Using C# 11 'required' for NRT

        // state character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("state")]
        public required string State { get; set; } // Using C# 11 'required' for NRT

        // country character varying(50) NOT NULL
        [Required]
        [StringLength(50)]
        [Column("country")]
        public required string Country { get; set; } // Using C# 11 'required' for NRT

        // zip_code character varying(20) (Nullable)
        [StringLength(20)]
        [Column("zip_code")]
        public string? ZipCode { get; set; } // string? indicates nullable
    }
}