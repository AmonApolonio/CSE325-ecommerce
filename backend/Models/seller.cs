using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Define the table name and schema (public.sellers)
    [Table("sellers", Schema = "public")]
    public class Seller
    {
        // seller_id bigint NOT NULL
        [Key] // Defines this property as the Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assuming it's auto-generated in the DB
        [Column("seller_id")]
        public long SellerId { get; set; }

        // name character varying(255) NOT NULL
        [Required]
        [StringLength(255)]
        [Column("name")]
        public required string Name { get; set; }

        // photo_url character varying(150) NOT NULL
        [Required]
        [StringLength(150)]
        [Column("photo_url")]
        public required string PhotoUrl { get; set; }

        // about_me text
        [Column("about_me")]
        public string? AboutMe { get; set; } // string? is nullable

        // address1 character varying(255) NOT NULL
        [Required]
        [StringLength(255)]
        [Column("address1")]
        public required string Address1 { get; set; }

        // address2 character varying(255) NOT NULL
        [Required]
        [StringLength(255)]
        [Column("address2")]
        public required string Address2 { get; set; }

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

        // zip_code character varying(20) NOT NULL
        [Required]
        [StringLength(20)]
        [Column("zip_code")]
        public required string ZipCode { get; set; }

        // phone_number character varying(50) (Pode ser NULL)
        [StringLength(50)]
        [Column("phone_number")]
        public string? PhoneNumber { get; set; } // string? is nullable

        // commision_rate numeric DEFAULT 5 NOT NULL
        // Mapped to decimal for precision
        [Required]
        [Column("commision_rate", TypeName = "numeric")] // Ensures correct SQL type mapping for Npgsql
        public decimal CommisionRate { get; set; } = 5; // Set the C# default to match the DB default

        // email character varying(150) NOT NULL
        [Required]
        [EmailAddress] // Provides useful metadata for validation
        [StringLength(150)]
        [Column("email")]
        public required string Email { get; set; }

        // password_hash text NOT NULL
        // IMPORTANT: Never store plain passwords; store only secure hashes.
        [Required]
        [Column("password_hash")]
        public required string PasswordHash { get; set; }
    }
}