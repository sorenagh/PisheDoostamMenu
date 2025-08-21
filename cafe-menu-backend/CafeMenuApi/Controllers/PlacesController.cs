using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using CafeMenuApi.Models;
using CafeMenuApi.DTOs;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly CafeMenuContext _context;

        public PlacesController(CafeMenuContext context)
        {
            _context = context;
        }

        // GET: api/Places
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaceDto>>> GetPlaces()
        {
            var places = await _context.Places
                .Where(p => p.IsActive)
                .Select(p => new PlaceDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Address = p.Address,
                    Phone = p.Phone,
                    Email = p.Email,
                    Logo = p.Logo,
                    CoverImage = p.CoverImage,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(places);
        }

        // GET: api/Places/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlaceDto>> GetPlace(int id)
        {
            var place = await _context.Places
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new PlaceDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Address = p.Address,
                    Phone = p.Phone,
                    Email = p.Email,
                    Logo = p.Logo,
                    CoverImage = p.CoverImage,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (place == null)
            {
                return NotFound();
            }

            return Ok(place);
        }

        // POST: api/Places
        [HttpPost]
        public async Task<ActionResult<PlaceDto>> CreatePlace(CreatePlaceDto createPlaceDto)
        {
            var place = new Place
            {
                Name = createPlaceDto.Name,
                Description = createPlaceDto.Description,
                Address = createPlaceDto.Address,
                Phone = createPlaceDto.Phone,
                Email = createPlaceDto.Email,
                Logo = createPlaceDto.Logo,
                CoverImage = createPlaceDto.CoverImage,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            var placeDto = new PlaceDto
            {
                Id = place.Id,
                Name = place.Name,
                Description = place.Description,
                Address = place.Address,
                Phone = place.Phone,
                Email = place.Email,
                Logo = place.Logo,
                CoverImage = place.CoverImage,
                IsActive = place.IsActive,
                CreatedAt = place.CreatedAt,
                UpdatedAt = place.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPlace), new { id = place.Id }, placeDto);
        }

        // PUT: api/Places/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlace(int id, UpdatePlaceDto updatePlaceDto)
        {
            var place = await _context.Places.FindAsync(id);

            if (place == null)
            {
                return NotFound();
            }

            place.Name = updatePlaceDto.Name;
            place.Description = updatePlaceDto.Description;
            place.Address = updatePlaceDto.Address;
            place.Phone = updatePlaceDto.Phone;
            place.Email = updatePlaceDto.Email;
            place.Logo = updatePlaceDto.Logo;
            place.CoverImage = updatePlaceDto.CoverImage;
            place.IsActive = updatePlaceDto.IsActive;
            place.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Places/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlace(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }

            // Soft delete - just mark as inactive
            place.IsActive = false;
            place.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
