using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using CafeMenuApi.Models;
using CafeMenuApi.DTOs;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CafeMenuContext _context;

        public CategoriesController(CafeMenuContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        [ResponseCache(Duration = 600)] // Cache for 10 minutes
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery] int? placeId = null)
        {
            var query = _context.Categories
                .Include(c => c.MenuItems)
                .Include(c => c.Place)
                .AsNoTracking(); // Optimize for read-only queries

            // Filter by place if specified
            if (placeId.HasValue)
            {
                query = query.Where(c => c.PlaceId == placeId.Value);
            }

            var categories = await query
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    Description = c.Description,
                    ItemCount = c.MenuItems.Count,
                    PlaceId = c.PlaceId,
                    Place = new PlaceDto
                    {
                        Id = c.Place.Id,
                        Name = c.Place.Name,
                        Description = c.Place.Description,
                        Address = c.Place.Address,
                        Phone = c.Place.Phone,
                        Email = c.Place.Email,
                        Logo = c.Place.Logo,
                        CoverImage = c.Place.CoverImage,
                        IsActive = c.Place.IsActive,
                        CreatedAt = c.Place.CreatedAt,
                        UpdatedAt = c.Place.UpdatedAt
                    }
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 600)] // Cache for 10 minutes
        public async Task<ActionResult<CategoryDto>> GetCategory(int id, [FromQuery] int? placeId = null)
        {
            var category = await _context.Categories
                .Include(c => c.MenuItems)
                .Include(c => c.Place)
                .AsNoTracking() // Optimize for read-only queries
                .FirstOrDefaultAsync(c => c.Id == id && (!placeId.HasValue || c.PlaceId == placeId.Value));

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon,
                Description = category.Description,
                ItemCount = category.MenuItems.Count,
                PlaceId = category.PlaceId,
                Place = new PlaceDto
                {
                    Id = category.Place.Id,
                    Name = category.Place.Name,
                    Description = category.Place.Description,
                    Address = category.Place.Address,
                    Phone = category.Place.Phone,
                    Email = category.Place.Email,
                    Logo = category.Place.Logo,
                    CoverImage = category.Place.CoverImage,
                    IsActive = category.Place.IsActive,
                    CreatedAt = category.Place.CreatedAt,
                    UpdatedAt = category.Place.UpdatedAt
                }
            };

            return Ok(categoryDto);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createDto)
        {
            var category = new Category
            {
                Name = createDto.Name,
                Icon = createDto.Icon,
                Description = createDto.Description,
                PlaceId = createDto.PlaceId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon,
                Description = category.Description,
                ItemCount = 0,
                PlaceId = category.PlaceId
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
        }

        // POST: api/Categories/5/update
        [HttpPost("{id}/update")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = updateDto.Name;
            category.Icon = updateDto.Icon;
            category.Description = updateDto.Description;
            category.PlaceId = updateDto.PlaceId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/Categories/5/delete
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
} 