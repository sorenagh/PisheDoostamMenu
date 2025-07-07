using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeMenuApi.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required]
        public string Image { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        // Foreign key
        public int CategoryId { get; set; }
        
        // Navigation property
        public virtual Category Category { get; set; } = null!;
        
        // Photos stored as JSON or comma-separated string
        public string Photos { get; set; } = string.Empty;
        
        // Helper property to get photos as list
        [NotMapped]
        public List<string> PhotosList
        {
            get => string.IsNullOrEmpty(Photos) ? new List<string>() : Photos.Split(',').ToList();
            set => Photos = string.Join(",", value);
        }
    }
} 