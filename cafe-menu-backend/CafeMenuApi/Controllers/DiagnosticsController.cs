using Microsoft.AspNetCore.Mvc;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            // Most basic response possible - no dependencies
            return Ok(new
            {
                status = "alive",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                message = "API is responding"
            });
        }

        [HttpGet("simple")]
        public string Simple()
        {
            // Even simpler - just return a string
            return $"API Working - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            try
            {
                var info = new
                {
                    timestamp = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    machineName = Environment.MachineName,
                    osVersion = Environment.OSVersion.ToString(),
                    processId = Environment.ProcessId,
                    workingDirectory = Directory.GetCurrentDirectory(),
                    requestUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}",
                    userAgent = Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"].ToString() : "Unknown"
                };

                return Ok(info);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Info endpoint failed",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            try
            {
                var configuration = HttpContext.RequestServices.GetService<IConfiguration>();
                
                var config = new
                {
                    timestamp = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    settings = new
                    {
                        allowedHosts = configuration?["AllowedHosts"] ?? "Not found",
                        detailedErrors = configuration?["DetailedErrors"] ?? "Not found",
                        captureStartupErrors = configuration?["CaptureStartupErrors"] ?? "Not found",
                        connectionStringExists = !string.IsNullOrEmpty(configuration?.GetConnectionString("DefaultConnection")),
                        connectionStringLength = configuration?.GetConnectionString("DefaultConnection")?.Length ?? 0
                    },
                    configurationFiles = new
                    {
                        currentDirectory = Directory.GetCurrentDirectory(),
                        appsettingsExists = System.IO.File.Exists("appsettings.json"),
                        appsettingsProdExists = System.IO.File.Exists("appsettings.Production.json"),
                        webConfigExists = System.IO.File.Exists("web.config")
                    }
                };

                return Ok(config);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Config endpoint failed",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("This is a test exception to verify error handling is working");
        }

        [HttpGet("files")]
        public IActionResult GetFiles()
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                var files = Directory.GetFiles(currentDir, "*.*")
                    .Select(f => new { 
                        name = Path.GetFileName(f), 
                        size = new FileInfo(f).Length,
                        lastModified = new FileInfo(f).LastWriteTime
                    })
                    .ToList();

                return Ok(new
                {
                    currentDirectory = currentDir,
                    filesCount = files.Count,
                    files = files.Take(20), // Limit to first 20 files
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Files endpoint failed",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
} 