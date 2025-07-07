using Microsoft.AspNetCore.Mvc;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("simple")]
        public IActionResult SimpleTest()
        {
            return Ok(new { 
                message = "Backend is working!", 
                timestamp = DateTime.UtcNow,
                status = "success"
            });
        }

        [HttpGet("cors")]
        public IActionResult CorsTest()
        {
            return Ok(new { 
                message = "CORS is working!", 
                timestamp = DateTime.UtcNow,
                origin = Request.Headers["Origin"].FirstOrDefault() ?? "No origin",
                method = Request.Method
            });
        }

        [HttpPost("update-test")]
        public IActionResult UpdateTest([FromBody] object data)
        {
            return Ok(new { 
                message = "POST update method is working!", 
                timestamp = DateTime.UtcNow,
                receivedData = data
            });
        }

        [HttpPost("post-test")]
        public IActionResult PostTest([FromBody] object data)
        {
            return Ok(new { 
                message = "POST method is working!", 
                timestamp = DateTime.UtcNow,
                receivedData = data
            });
        }

        [HttpPost("delete-test")]
        public IActionResult DeleteTest()
        {
            return Ok(new { 
                message = "POST delete method is working!", 
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("uploads-info")]
        public IActionResult UploadsInfo()
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var files = new List<object>();
            
            if (Directory.Exists(uploadsPath))
            {
                var fileInfos = Directory.GetFiles(uploadsPath);
                files = fileInfos.Select(f => new
                {
                    filename = Path.GetFileName(f),
                    size = new FileInfo(f).Length,
                    created = new FileInfo(f).CreationTime,
                    url = $"{Request.Scheme}://{Request.Host}/uploads/{Path.GetFileName(f)}"
                }).ToList<object>();
            }

            return Ok(new
            {
                message = "Uploads folder info",
                uploadsPath = uploadsPath,
                exists = Directory.Exists(uploadsPath),
                fileCount = files.Count,
                files = files,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("routes")]
        public IActionResult RoutesInfo()
        {
            try
            {
                return Ok(new
                {
                    message = "Available test routes",
                    routes = new[]
                    {
                        "GET /api/test/simple",
                        "GET /api/test/cors", 
                        "POST /api/test/update-test",
                        "POST /api/test/post-test",
                        "POST /api/test/delete-test",
                        "GET /api/test/uploads-info",
                        "GET /api/test/routes",
                        "GET /api/menuitems",
                        "GET /api/menuitems/{id}",
                        "POST /api/menuitems/{id}/update",
                        "POST /api/menuitems",
                        "POST /api/menuitems/{id}/delete",
                        "GET /api/categories",
                        "GET /api/categories/{id}",
                        "POST /api/categories/{id}/update",
                        "POST /api/categories",
                        "POST /api/categories/{id}/delete",
                        "POST /api/upload/image",
                        "POST /api/upload/images",
                        "POST /api/upload/image/{fileName}/delete"
                    },
                    timestamp = DateTime.UtcNow,
                    requestInfo = new
                    {
                        method = Request.Method,
                        path = Request.Path,
                        origin = Request.Headers["Origin"].FirstOrDefault() ?? "No origin",
                        userAgent = Request.Headers["User-Agent"].FirstOrDefault() ?? "No user agent",
                        headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
} 