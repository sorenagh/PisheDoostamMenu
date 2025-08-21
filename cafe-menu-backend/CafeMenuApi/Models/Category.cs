using System.ComponentModel.DataAnnotations;

namespace CafeMenuApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Icon { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // Foreign key for Place
        public int PlaceId { get; set; }
        
        // Navigation properties
        public virtual Place Place { get; set; } = null!;
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
} 