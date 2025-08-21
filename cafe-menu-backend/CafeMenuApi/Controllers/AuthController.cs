using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using CafeMenuApi.DTOs;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CafeMenuContext _context;

        public AuthController(CafeMenuContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AdminLoginResponseDto>> Login(AdminLoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Place)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.Password == loginDto.Password);

            if (user == null)
            {
                return Ok(new AdminLoginResponseDto
                {
                    Success = false,
                    Message = "نام کاربری یا رمز عبور اشتباه است"
                });
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateSimpleToken(user.Id);

            return Ok(new AdminLoginResponseDto
            {
                Success = true,
                Message = "ورود با موفقیت انجام شد",
                Token = token,
                Admin = new AdminDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
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
                    }
                }
            });
        }

        [HttpPost("verify")]
        public async Task<ActionResult<AdminDto>> VerifyToken([FromBody] string token)
        {
            var userId = ExtractUserIdFromToken(token);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.Place)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new AdminDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
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
                }
            });
        }

        private string GenerateSimpleToken(int userId)
        {
            var tokenData = $"{userId}:{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}";
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        private int? ExtractUserIdFromToken(string token)
        {
            try
            {
                var tokenBytes = Convert.FromBase64String(token);
                var tokenData = System.Text.Encoding.UTF8.GetString(tokenBytes);
                var parts = tokenData.Split(':');
                
                if (parts.Length >= 2 && int.TryParse(parts[0], out int userId))
                {
                    return userId;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
} 