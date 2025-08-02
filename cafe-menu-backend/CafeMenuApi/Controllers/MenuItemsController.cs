using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using CafeMenuApi.Models;
using CafeMenuApi.DTOs;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemsController : ControllerBase
    {
        private readonly CafeMenuContext _context;
        private readonly ILogger<MenuItemsController> _logger;

        public MenuItemsController(CafeMenuContext context, ILogger<MenuItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/MenuItems
        [HttpGet]
        [ResponseCache(Duration = 300)] // Cache for 5 minutes
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems(int? categoryId = null)
        {
            try
            {
                _logger.LogInformation("Getting menu items, categoryId: {CategoryId}", categoryId);
                
                var query = _context.MenuItems
                    .Include(m => m.Category)
                    .AsNoTracking() // Optimize for read-only queries
                    .AsQueryable();

                if (categoryId.HasValue)
                {
                    query = query.Where(m => m.CategoryId == categoryId.Value);
                }

                var menuItems = await query
                    .Select(m => new MenuItemDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Price = m.Price,
                        Image = m.Image,
                        Description = m.Description,
                        CategoryId = m.CategoryId,
                        CategoryName = m.Category.Name,
                        Photos = m.PhotosList
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} menu items", menuItems.Count);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting menu items with categoryId: {CategoryId}", categoryId);
                return StatusCode(500, new { error = "An error occurred while retrieving menu items", details = ex.Message });
            }
        }

        // GET: api/MenuItems/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 600)] // Cache for 10 minutes
        public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .AsNoTracking() // Optimize for read-only queries
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                Image = menuItem.Image,
                Description = menuItem.Description,
                CategoryId = menuItem.CategoryId,
                CategoryName = menuItem.Category.Name,
                Photos = menuItem.PhotosList
            };

            return Ok(menuItemDto);
        }

        // GET: api/MenuItems/category/5
        [HttpGet("category/{categoryId}")]
        [ResponseCache(Duration = 300)] // Cache for 5 minutes
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItemsByCategory(int categoryId)
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category)
                .AsNoTracking() // Optimize for read-only queries
                .Where(m => m.CategoryId == categoryId)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Image = m.Image,
                    Description = m.Description,
                    CategoryId = m.CategoryId,
                    CategoryName = m.Category.Name,
                    Photos = m.PhotosList
                })
                .ToListAsync();

            return Ok(menuItems);
        }

        // POST: api/MenuItems
        [HttpPost]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem(CreateMenuItemDto createDto)
        {
            // Verify category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == createDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category not found");
            }

            var menuItem = new MenuItem
            {
                Name = createDto.Name,
                Price = createDto.Price,
                Image = createDto.Image,
                Description = createDto.Description,
                CategoryId = createDto.CategoryId,
                PhotosList = createDto.Photos
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            // Load the category for the response
            await _context.Entry(menuItem)
                .Reference(m => m.Category)
                .LoadAsync();

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                Image = menuItem.Image,
                Description = menuItem.Description,
                CategoryId = menuItem.CategoryId,
                CategoryName = menuItem.Category.Name,
                Photos = menuItem.PhotosList
            };

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItemDto);
        }

        // POST: api/MenuItems/5/update
        [HttpPost("{id}/update")]
        public async Task<IActionResult> UpdateMenuItem(int id, UpdateMenuItemDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating menu item with ID: {Id}", id);
                _logger.LogInformation("Update data: {UpdateDto}", System.Text.Json.JsonSerializer.Serialize(updateDto));

                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                {
                    _logger.LogWarning("Menu item with ID {Id} not found", id);
                    return NotFound(new { error = "Menu item not found", id });
                }

                // Verify category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == updateDto.CategoryId);
                if (!categoryExists)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found", updateDto.CategoryId);
                    return BadRequest(new { error = "Category not found", categoryId = updateDto.CategoryId });
                }

                menuItem.Name = updateDto.Name;
                menuItem.Price = updateDto.Price;
                menuItem.Image = updateDto.Image;
                menuItem.Description = updateDto.Description;
                menuItem.CategoryId = updateDto.CategoryId;
                menuItem.PhotosList = updateDto.Photos;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated menu item with ID: {Id}", id);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating menu item with ID: {Id}", id);
                if (!MenuItemExists(id))
                {
                    return NotFound(new { error = "Menu item no longer exists", id });
                }
                return Conflict(new { error = "Concurrency conflict", details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating menu item with ID: {Id}", id);
                return StatusCode(500, new { error = "An error occurred while updating the menu item", details = ex.Message });
            }
        }

        // POST: api/MenuItems/5/delete
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.Id == id);
        }
    }
} 