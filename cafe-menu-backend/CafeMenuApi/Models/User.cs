using System.ComponentModel.DataAnnotations;

namespace CafeMenuApi.Models
{
    public enum UserRole
    {
        SystemAdmin = 1,
        CafeAdmin = 2
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.CafeAdmin;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        // Optional for SystemAdmin; required for CafeAdmin
        public int? PlaceId { get; set; }

        public virtual Place? Place { get; set; }
    }
}
