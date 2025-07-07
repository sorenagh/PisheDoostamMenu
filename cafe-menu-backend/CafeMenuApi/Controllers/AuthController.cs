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
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == loginDto.Username && a.Password == loginDto.Password);

            if (admin == null)
            {
                return Ok(new AdminLoginResponseDto
                {
                    Success = false,
                    Message = "نام کاربری یا رمز عبور اشتباه است"
                });
            }

            admin.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateSimpleToken(admin.Id);

            return Ok(new AdminLoginResponseDto
            {
                Success = true,
                Message = "ورود با موفقیت انجام شد",
                Token = token,
                Admin = new AdminDto
                {
                    Id = admin.Id,
                    Username = admin.Username,
                    CreatedAt = admin.CreatedAt,
                    LastLoginAt = admin.LastLoginAt
                }
            });
        }

        [HttpPost("verify")]
        public async Task<ActionResult<AdminDto>> VerifyToken([FromBody] string token)
        {
            var adminId = ExtractAdminIdFromToken(token);
            if (adminId == null)
            {
                return Unauthorized();
            }

            var admin = await _context.Admins.FindAsync(adminId);
            if (admin == null)
            {
                return Unauthorized();
            }

            return Ok(new AdminDto
            {
                Id = admin.Id,
                Username = admin.Username,
                CreatedAt = admin.CreatedAt,
                LastLoginAt = admin.LastLoginAt
            });
        }

        private string GenerateSimpleToken(int adminId)
        {
            var tokenData = $"{adminId}:{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}";
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        private int? ExtractAdminIdFromToken(string token)
        {
            try
            {
                var tokenBytes = Convert.FromBase64String(token);
                var tokenData = System.Text.Encoding.UTF8.GetString(tokenBytes);
                var parts = tokenData.Split(':');
                
                if (parts.Length >= 2 && int.TryParse(parts[0], out int adminId))
                {
                    return adminId;
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