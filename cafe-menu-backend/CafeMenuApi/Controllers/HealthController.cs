using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using System.Reflection;
using System.Diagnostics;

namespace CafeMenuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly CafeMenuContext _context;
        private readonly ILogger<HealthController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HealthController(CafeMenuContext context, ILogger<HealthController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("Health check requested at {Timestamp}", DateTime.UtcNow);

                var healthStatus = new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    environment = _environment.EnvironmentName,
                    version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                    uptime = DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss"),
                    checks = new
                    {
                        database = await CheckDatabaseConnection(),
                        application = "OK"
                    }
                };

                _logger.LogInformation("Health check completed successfully");
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    environment = _environment.EnvironmentName
                });
            }
        }

        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailed()
        {
            try
            {
                _logger.LogInformation("Detailed health check requested at {Timestamp}", DateTime.UtcNow);

                var detailedStatus = new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    environment = _environment.EnvironmentName,
                    version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                    uptime = DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss"),
                    server = new
                    {
                        machineName = Environment.MachineName,
                        osVersion = Environment.OSVersion.ToString(),
                        frameworkVersion = Environment.Version.ToString(),
                        workingSet = Environment.WorkingSet,
                        processorCount = Environment.ProcessorCount
                    },
                    database = await GetDatabaseInfo(),
                    configuration = new
                    {
                        allowedHosts = HttpContext.RequestServices.GetService<IConfiguration>()?["AllowedHosts"],
                        detailedErrors = HttpContext.RequestServices.GetService<IConfiguration>()?["DetailedErrors"]
                    }
                };

                return Ok(detailedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    stackTrace = _environment.IsDevelopment() ? ex.StackTrace : null
                });
            }
        }

        private async Task<string> CheckDatabaseConnection()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                return "Connected";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection check failed: {Message}", ex.Message);
                return $"Failed: {ex.Message}";
            }
        }

        private async Task<object> GetDatabaseInfo()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

                return new
                {
                    canConnect = canConnect,
                    pendingMigrations = pendingMigrations.ToList(),
                    appliedMigrationsCount = appliedMigrations.Count(),
                    connectionString = _context.Database.GetConnectionString()?.Replace("Password=", "Password=***")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get database info: {Message}", ex.Message);
                return new
                {
                    error = ex.Message,
                    canConnect = false
                };
            }
        }
    }
} 