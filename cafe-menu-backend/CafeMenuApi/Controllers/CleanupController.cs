using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using CafeMenuApi.Models;
using System.Text.RegularExpressions;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CleanupController : ControllerBase
    {
        private readonly CafeMenuContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsPath;

        public CleanupController(CafeMenuContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads");
            
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        [HttpPost("reset-database")]
        public async Task<IActionResult> ResetDatabase([FromQuery] int? placeId = null)
        {
            try
            {
                var deletedCounts = new
                {
                    menuItems = 0,
                    categories = 0,
                    uploadedFiles = 0
                };

                // Delete menu items for the specified place (or all if not specified)
                var menuItems = await _context.MenuItems
                    .Where(m => !placeId.HasValue || m.PlaceId == placeId.Value)
                    .ToListAsync();
                if (menuItems.Any())
                {
                    _context.MenuItems.RemoveRange(menuItems);
                    deletedCounts = deletedCounts with { menuItems = menuItems.Count };
                }

                // Delete categories for the specified place (or all if not specified), except ID 0
                var categories = await _context.Categories
                    .Where(c => c.Id != 0)
                    .Where(c => !placeId.HasValue || c.PlaceId == placeId.Value)
                    .ToListAsync();
                if (categories.Any())
                {
                    _context.Categories.RemoveRange(categories);
                    deletedCounts = deletedCounts with { categories = categories.Count };
                }

                // Save database changes
                await _context.SaveChangesAsync();

                // Clean up uploaded files (not scoped by place as files are not linked)
                var uploadedFiles = 0;
                if (Directory.Exists(_uploadsPath))
                {
                    var files = Directory.GetFiles(_uploadsPath);
                    foreach (var file in files)
                    {
                        try
                        {
                            System.IO.File.Delete(file);
                            uploadedFiles++;
                        }
                        catch
                        {
                            // Ignore file deletion errors
                        }
                    }
                }

                deletedCounts = deletedCounts with { uploadedFiles = uploadedFiles };

                return Ok(new
                {
                    success = true,
                    message = "Database reset completed successfully",
                    deleted = deletedCounts,
                    note = "System/User accounts preserved, other data cleared",
                    placeScoped = placeId.HasValue,
                    placeId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error resetting database",
                    error = ex.Message
                });
            }
        }

        [HttpGet("database-status")]
        public async Task<IActionResult> GetDatabaseStatus([FromQuery] int? placeId = null)
        {
            try
            {
                var menuItemsCount = await _context.MenuItems
                    .Where(m => !placeId.HasValue || m.PlaceId == placeId.Value)
                    .CountAsync();
                var categoriesCount = await _context.Categories
                    .Where(c => c.Id != 0)
                    .Where(c => !placeId.HasValue || c.PlaceId == placeId.Value)
                    .CountAsync();
                var systemAdmins = await _context.Users
                    .Where(u => u.Role == UserRole.SystemAdmin)
                    .CountAsync();
                var cafeAdmins = await _context.Users
                    .Where(u => u.Role == UserRole.CafeAdmin)
                    .Where(u => !placeId.HasValue || u.PlaceId == placeId.Value)
                    .CountAsync();
                
                var uploadedFiles = 0;
                if (Directory.Exists(_uploadsPath))
                {
                    uploadedFiles = Directory.GetFiles(_uploadsPath).Length;
                }

                return Ok(new
                {
                    menuItems = menuItemsCount,
                    categories = categoriesCount,
                    systemAdmins = systemAdmins,
                    cafeAdmins = cafeAdmins,
                    uploadedFiles = uploadedFiles,
                    databaseSize = await EstimateDatabaseSize(placeId),
                    placeScoped = placeId.HasValue,
                    placeId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error getting database status",
                    error = ex.Message
                });
            }
        }

        [HttpPost("migrate-base64-images")]
        public async Task<IActionResult> MigrateBase64Images([FromQuery] int? placeId = null)
        {
            try
            {
                var menuItems = await _context.MenuItems
                    .Where(m => !placeId.HasValue || m.PlaceId == placeId.Value)
                    .ToListAsync();
                var migratedCount = 0;
                var errors = new List<string>();

                foreach (var item in menuItems)
                {
                    try
                    {
                        // Check and convert main image
                        if (!string.IsNullOrEmpty(item.Image) && item.Image.StartsWith("data:image/"))
                        {
                            var newImageUrl = await ConvertBase64ToFile(item.Image, "image");
                            if (!string.IsNullOrEmpty(newImageUrl))
                            {
                                item.Image = newImageUrl;
                                migratedCount++;
                            }
                        }

                        // Check and convert photos array
                        if (!string.IsNullOrEmpty(item.Photos))
                        {
                            var photos = System.Text.Json.JsonSerializer.Deserialize<string[]>(item.Photos);
                            if (photos != null && photos.Length > 0)
                            {
                                var newPhotos = new List<string>();
                                foreach (var photo in photos)
                                {
                                    if (!string.IsNullOrEmpty(photo) && photo.StartsWith("data:image/"))
                                    {
                                        var newPhotoUrl = await ConvertBase64ToFile(photo, "photo");
                                        if (!string.IsNullOrEmpty(newPhotoUrl))
                                        {
                                            newPhotos.Add(newPhotoUrl);
                                            migratedCount++;
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(photo))
                                    {
                                        newPhotos.Add(photo); // Keep existing URL
                                    }
                                }
                                item.Photos = System.Text.Json.JsonSerializer.Serialize(newPhotos.ToArray());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing item {item.Id}: {ex.Message}");
                    }
                }

                // Save changes to database
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Base64 images migration completed",
                    migratedCount = migratedCount,
                    totalItems = menuItems.Count,
                    errors = errors,
                    placeScoped = placeId.HasValue,
                    placeId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error during migration",
                    error = ex.Message
                });
            }
        }

        [HttpGet("analyze-images")]
        public async Task<IActionResult> AnalyzeImages([FromQuery] int? placeId = null)
        {
            try
            {
                var menuItems = await _context.MenuItems
                    .Where(m => !placeId.HasValue || m.PlaceId == placeId.Value)
                    .ToListAsync();
                var base64Count = 0;
                var urlCount = 0;
                var emptyCount = 0;
                var totalSize = 0L;
                var base64Items = new List<object>();

                foreach (var item in menuItems)
                {
                    var itemAnalysis = new
                    {
                        id = item.Id,
                        name = item.Name,
                        hasBase64Image = false,
                        hasBase64Photos = false,
                        imageSize = 0L,
                        photosSize = 0L
                    };

                    // Analyze main image
                    if (!string.IsNullOrEmpty(item.Image))
                    {
                        if (item.Image.StartsWith("data:image/"))
                        {
                            base64Count++;
                            var imageSize = EstimateBase64Size(item.Image);
                            totalSize += imageSize;
                            itemAnalysis = itemAnalysis with { hasBase64Image = true, imageSize = imageSize };
                        }
                        else
                        {
                            urlCount++;
                        }
                    }
                    else
                    {
                        emptyCount++;
                    }

                    // Analyze photos
                    if (!string.IsNullOrEmpty(item.Photos))
                    {
                        try
                        {
                            var photos = System.Text.Json.JsonSerializer.Deserialize<string[]>(item.Photos);
                            if (photos != null)
                            {
                                var photosSize = 0L;
                                var hasBase64Photos = false;
                                foreach (var photo in photos)
                                {
                                    if (!string.IsNullOrEmpty(photo) && photo.StartsWith("data:image/"))
                                    {
                                        hasBase64Photos = true;
                                        var photoSize = EstimateBase64Size(photo);
                                        photosSize += photoSize;
                                        totalSize += photoSize;
                                    }
                                }
                                if (hasBase64Photos)
                                {
                                    itemAnalysis = itemAnalysis with { hasBase64Photos = true, photosSize = photosSize };
                                }
                            }
                        }
                        catch
                        {
                            // Ignore JSON parsing errors
                        }
                    }

                    if (itemAnalysis.hasBase64Image || itemAnalysis.hasBase64Photos)
                    {
                        base64Items.Add(itemAnalysis);
                    }
                }

                return Ok(new
                {
                    totalItems = menuItems.Count,
                    base64Images = base64Count,
                    urlImages = urlCount,
                    emptyImages = emptyCount,
                    estimatedTotalSize = FormatBytes(totalSize),
                    estimatedTotalSizeBytes = totalSize,
                    itemsWithBase64 = base64Items,
                    placeScoped = placeId.HasValue,
                    placeId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error analyzing images",
                    error = ex.Message
                });
            }
        }

        private async Task<string?> ConvertBase64ToFile(string base64Data, string prefix)
        {
            try
            {
                // Extract the base64 part and content type
                var match = Regex.Match(base64Data, @"data:image/(\w+);base64,(.+)");
                if (!match.Success)
                    return null;

                var extension = match.Groups[1].Value;
                var base64String = match.Groups[2].Value;
                var imageBytes = Convert.FromBase64String(base64String);

                // Generate unique filename
                var fileName = $"{prefix}_{Guid.NewGuid()}.{extension}";
                var filePath = Path.Combine(_uploadsPath, fileName);

                // Save file
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Return URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                return $"{baseUrl}/uploads/{fileName}";
            }
            catch
            {
                return null;
            }
        }

        private long EstimateBase64Size(string base64Data)
        {
            if (string.IsNullOrEmpty(base64Data))
                return 0;

            // Base64 encoding increases size by ~33%, so reverse calculate
            var base64Length = base64Data.Length;
            if (base64Data.StartsWith("data:"))
            {
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex > 0)
                    base64Length = base64Data.Length - commaIndex - 1;
            }

            return (long)(base64Length * 0.75); // Approximate original file size
        }

        private async Task<string> EstimateDatabaseSize(int? placeId)
        {
            try
            {
                var totalSize = 0L;
                
                // Estimate from menu items (mainly images)
                var menuItems = await _context.MenuItems
                    .Where(m => !placeId.HasValue || m.PlaceId == placeId.Value)
                    .ToListAsync();
                foreach (var item in menuItems)
                {
                    if (!string.IsNullOrEmpty(item.Image))
                        totalSize += EstimateBase64Size(item.Image);
                    if (!string.IsNullOrEmpty(item.Photos))
                        totalSize += item.Photos.Length;
                    if (!string.IsNullOrEmpty(item.Description))
                        totalSize += item.Description.Length * 2; // Unicode
                }

                return FormatBytes(totalSize);
            }
            catch
            {
                return "Unknown";
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
} 