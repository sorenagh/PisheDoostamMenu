namespace CafeMenuApi.DTOs
{
	public class UserDto
	{
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public int? PlaceId { get; set; }
		public PlaceDto? Place { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? LastLoginAt { get; set; }
	}

	public class CreateUserDto
	{
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string Role { get; set; } = "CafeAdmin"; // SystemAdmin or CafeAdmin
		public int? PlaceId { get; set; }
	}

	public class UpdateUserDto
	{
		public string Username { get; set; } = string.Empty;
		public string? Password { get; set; }
		public string Role { get; set; } = "CafeAdmin";
		public int? PlaceId { get; set; }
	}
}
