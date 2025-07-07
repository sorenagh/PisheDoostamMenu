using Microsoft.AspNetCore.Mvc;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsPath;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads");
            
            // Ensure uploads directory exists
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        [HttpPost("image")]
        public async Task<ActionResult<string>> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest("File must be an image");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File size must be less than 5MB");
            }

            try
            {
                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_uploadsPath, fileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return URL path
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var imageUrl = $"{baseUrl}/uploads/{fileName}";

                return Ok(imageUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpPost("images")]
        public async Task<ActionResult<List<string>>> UploadMultipleImages(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded");
            }

            if (files.Count > 3)
            {
                return BadRequest("Maximum 3 files allowed");
            }

            var results = new List<string>();

            try
            {
                foreach (var file in files)
                {
                    if (file == null || file.Length == 0) continue;

                    if (!file.ContentType.StartsWith("image/"))
                    {
                        return BadRequest($"File {file.FileName} must be an image");
                    }

                    if (file.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest($"File {file.FileName} size must be less than 5MB");
                    }

                    // Generate unique filename
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(_uploadsPath, fileName);

                    // Save file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add URL to results
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    var imageUrl = $"{baseUrl}/uploads/{fileName}";
                    results.Add(imageUrl);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading files: {ex.Message}");
            }
        }

        [HttpPost("image/{fileName}/delete")]
        public ActionResult DeleteImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadsPath, fileName);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok("Image deleted successfully");
                }
                else
                {
                    return NotFound("Image not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }
    }
} 