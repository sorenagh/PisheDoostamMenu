using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using CafeMenuApi.DTOs;
using CafeMenuApi.Models;

namespace CafeMenuApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly CafeMenuContext _context;

		public UsersController(CafeMenuContext context)
		{
			_context = context;
		}

		private async Task<User?> GetCurrentUser()
		{
			var authHeader = Request.Headers["Authorization"].FirstOrDefault();
			if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ")) return null;
			var token = authHeader.Substring("Bearer ".Length).Trim();
			var userId = ExtractUserIdFromToken(token);
			if (userId == null) return null;
			return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
		}

		private bool IsSystemAdmin(User? user) => user != null && user.Role == UserRole.SystemAdmin;

		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] string? role = null, [FromQuery] int? placeId = null)
		{
			var me = await GetCurrentUser();
			if (!IsSystemAdmin(me)) return Unauthorized();

			var query = _context.Users.Include(u => u.Place).AsNoTracking().AsQueryable();
			if (!string.IsNullOrWhiteSpace(role))
			{
				if (Enum.TryParse<UserRole>(role, true, out var parsed))
					query = query.Where(u => u.Role == parsed);
			}
			if (placeId.HasValue)
				query = query.Where(u => u.PlaceId == placeId.Value);

			var users = await query.Select(u => new UserDto
			{
				Id = u.Id,
				Username = u.Username,
				Role = u.Role.ToString(),
				PlaceId = u.PlaceId,
				Place = u.Place == null ? null : new PlaceDto
				{
					Id = u.Place.Id,
					Name = u.Place.Name,
					Description = u.Place.Description,
					Address = u.Place.Address,
					Phone = u.Place.Phone,
					Email = u.Place.Email,
					Logo = u.Place.Logo,
					CoverImage = u.Place.CoverImage,
					IsActive = u.Place.IsActive,
					CreatedAt = u.Place.CreatedAt,
					UpdatedAt = u.Place.UpdatedAt
				},
				CreatedAt = u.CreatedAt,
				LastLoginAt = u.LastLoginAt
			}).ToListAsync();

			return Ok(users);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserDto>> GetUser(int id)
		{
			var me = await GetCurrentUser();
			if (!IsSystemAdmin(me)) return Unauthorized();

			var u = await _context.Users.Include(x => x.Place).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
			if (u == null) return NotFound();
			return Ok(new UserDto
			{
				Id = u.Id,
				Username = u.Username,
				Role = u.Role.ToString(),
				PlaceId = u.PlaceId,
				Place = u.Place == null ? null : new PlaceDto
				{
					Id = u.Place.Id,
					Name = u.Place.Name,
					Description = u.Place.Description,
					Address = u.Place.Address,
					Phone = u.Place.Phone,
					Email = u.Place.Email,
					Logo = u.Place.Logo,
					CoverImage = u.Place.CoverImage,
					IsActive = u.Place.IsActive,
					CreatedAt = u.Place.CreatedAt,
					UpdatedAt = u.Place.UpdatedAt
				},
				CreatedAt = u.CreatedAt,
				LastLoginAt = u.LastLoginAt
			});
		}

		[HttpPost]
		public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
		{
			var me = await GetCurrentUser();
			if (!IsSystemAdmin(me)) return Unauthorized();

			if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
				return BadRequest("Invalid role");

			if (role == UserRole.CafeAdmin && !dto.PlaceId.HasValue)
				return BadRequest("CafeAdmin must have PlaceId");

			if (role == UserRole.SystemAdmin)
				dto.PlaceId = null;

			var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
			if (usernameExists) return Conflict("Username already exists");

			if (dto.PlaceId.HasValue)
			{
				var placeExists = await _context.Places.AnyAsync(p => p.Id == dto.PlaceId.Value);
				if (!placeExists) return BadRequest("Place not found");
			}

			var user = new User
			{
				Username = dto.Username,
				Password = dto.Password,
				Role = role,
				PlaceId = dto.PlaceId,
				CreatedAt = DateTime.UtcNow
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			await _context.Entry(user).Reference(u => u.Place).LoadAsync();

			return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
			{
				Id = user.Id,
				Username = user.Username,
				Role = user.Role.ToString(),
				PlaceId = user.PlaceId,
				Place = user.Place == null ? null : new PlaceDto
				{
					Id = user.Place.Id,
					Name = user.Place.Name,
					Description = user.Place.Description,
					Address = user.Place.Address,
					Phone = user.Place.Phone,
					Email = user.Place.Email,
					Logo = user.Place.Logo,
					CoverImage = user.Place.CoverImage,
					IsActive = user.Place.IsActive,
					CreatedAt = user.Place.CreatedAt,
					UpdatedAt = user.Place.UpdatedAt
				},
				CreatedAt = user.CreatedAt,
				LastLoginAt = user.LastLoginAt
			});
		}

		[HttpPost("{id}/update")]
		public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
		{
			var me = await GetCurrentUser();
			if (!IsSystemAdmin(me)) return Unauthorized();

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
			if (user == null) return NotFound();

			if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
				return BadRequest("Invalid role");

			if (role == UserRole.CafeAdmin && !dto.PlaceId.HasValue)
				return BadRequest("CafeAdmin must have PlaceId");

			if (role == UserRole.SystemAdmin)
				dto.PlaceId = null;

			if (!string.Equals(user.Username, dto.Username, StringComparison.OrdinalIgnoreCase))
			{
				var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id);
				if (usernameExists) return Conflict("Username already exists");
			}

			if (dto.PlaceId.HasValue)
			{
				var placeExists = await _context.Places.AnyAsync(p => p.Id == dto.PlaceId.Value);
				if (!placeExists) return BadRequest("Place not found");
			}

			user.Username = dto.Username;
			if (!string.IsNullOrEmpty(dto.Password)) user.Password = dto.Password;
			user.Role = role;
			user.PlaceId = dto.PlaceId;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPost("{id}/delete")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var me = await GetCurrentUser();
			if (!IsSystemAdmin(me)) return Unauthorized();

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
			if (user == null) return NotFound();

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		private int? ExtractUserIdFromToken(string token)
		{
			try
			{
				var tokenBytes = Convert.FromBase64String(token);
				var tokenData = System.Text.Encoding.UTF8.GetString(tokenBytes);
				var parts = tokenData.Split(':');
				if (parts.Length >= 2 && int.TryParse(parts[0], out int userId)) return userId;
				return null;
			}
			catch { return null; }
		}
	}
}
