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
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.MenuItems)
                .AsNoTracking() // Optimize for read-only queries
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    Description = c.Description,
                    ItemCount = c.MenuItems.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 600)] // Cache for 10 minutes
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.MenuItems)
                .AsNoTracking() // Optimize for read-only queries
                .FirstOrDefaultAsync(c => c.Id == id);

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
                ItemCount = category.MenuItems.Count
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
                Description = createDto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon,
                Description = category.Description,
                ItemCount = 0
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