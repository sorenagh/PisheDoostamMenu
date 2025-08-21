namespace CafeMenuApi.DTOs
{
    public class AdminLoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    
    public class AdminLoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public AdminDto? Admin { get; set; }
    }
    
    public class AdminDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "SystemAdmin" or "CafeAdmin"
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int? PlaceId { get; set; }
        public PlaceDto? Place { get; set; }
    }
} 