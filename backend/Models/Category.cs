using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    // Defines the table name and schema (public.category)
    [Table("category", Schema = "public")]
    public class Category
    {
        // category_id bigint NOT NULL (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Assumes it is auto-generated in the DB
        [Column("category_id")]
        public long CategoryId { get; set; }

        // name character varying(100) NOT NULL
        [Required]
        [StringLength(100)]
        [Column("name")]
        public required string Name { get; set; } // Using C# 11 'required' for Non-Nullable Reference Types (NRT)

        // description character varying(500) (Nullable)
        [StringLength(500)]
        [Column("description")]
        public string? Description { get; set; } // string? indicates nullable
    }
}