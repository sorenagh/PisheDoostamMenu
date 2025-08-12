using Microsoft.EntityFrameworkCore;
using CafeMenuApi.Data;
using System.Text.Json;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Log the startup process
Console.WriteLine($"=== CAFE MENU API STARTUP - {DateTime.UtcNow} ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Content Root: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Web Root: {builder.Environment.WebRootPath}");

// Configure logging based on environment
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Set minimum log level based on environment - OPTIMIZED for production
if (builder.Environment.IsProduction())
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning); // Changed from Trace to Warning
    Console.WriteLine("Production environment - enabling WARNING level logging for better performance");
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
    Console.WriteLine("Development environment - enabling DEBUG level logging");
}

try
{
    Console.WriteLine("Adding services to container...");
    
    // Add services to the container
    builder.Services.AddControllers();
    Console.WriteLine("✓ Controllers added");
    
    builder.Services.AddEndpointsApiExplorer();
    Console.WriteLine("✓ API Explorer added");
    
    builder.Services.AddSwaggerGen();
    Console.WriteLine("✓ Swagger added");

    // Add Response Compression for better performance
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });
    Console.WriteLine("✓ Response compression added");

    // Add Response Caching for better performance
    builder.Services.AddResponseCaching();
    Console.WriteLine("✓ Response caching added");

    // Get connection string and log it (without password)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("⚠️ WARNING: No connection string found!");
    }
    else
    {
        var safeConnectionString = connectionString.Replace("Password=", "Password=***");
        Console.WriteLine($"✓ Connection string found: {safeConnectionString}");
    }

    // Add Entity Framework with SQL Server and resiliency
    builder.Services.AddDbContext<CafeMenuContext>(options =>
        options.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            }));
    Console.WriteLine("✓ Entity Framework configured");

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowVueApp", policy =>
        {
            policy
                .SetIsOriginAllowed(_ => true) // allow any origin dynamically
                .AllowAnyHeader()
                .AllowAnyMethod();
            // Note: Avoid AllowCredentials with AllowAnyOrigin; if credentials needed, use specific origins
        });
    });
    Console.WriteLine("✓ CORS configured");

    Console.WriteLine("Building application...");
    var app = builder.Build();
    Console.WriteLine("✓ Application built successfully");

    // Get logger first
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Starting Cafe Menu API...");
    logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    logger.LogInformation("Application started at: {StartTime}", DateTime.UtcNow);
    Console.WriteLine("✓ Logger configured");

    // Simple error handling that shouldn't fail
    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== REQUEST ERROR ===");
            Console.WriteLine($"Time: {DateTime.UtcNow}");
            Console.WriteLine($"Path: {context.Request.Path}");
            Console.WriteLine($"Method: {context.Request.Method}");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.WriteLine($"=== END ERROR ===");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new
            {
                error = "Internal Server Error",
                message = ex.Message,
                stackTrace = ex.StackTrace,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value,
                method = context.Request.Method
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
        }
    });

    // Apply database migrations and seed data with detailed error handling
    Console.WriteLine("Attempting database connection and migrations...");
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CafeMenuContext>();
            logger.LogInformation("Testing database connection...");
            Console.WriteLine("Testing database connection...");
            
            var canConnect = await context.Database.CanConnectAsync();
            if (canConnect)
            {
                Console.WriteLine("✓ Database connection successful");
                logger.LogInformation("Database connection successful");
                
                Console.WriteLine("Applying database migrations...");
                context.Database.Migrate();
                Console.WriteLine("✓ Database migrations applied successfully");
                logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                Console.WriteLine("❌ Cannot connect to database!");
                logger.LogError("Cannot connect to database!");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        logger.LogError(ex, "An error occurred while applying database migrations: {Message}", ex.Message);
        // Continue without database - some endpoints might still work
    }

    // Configure the HTTP request pipeline
    Console.WriteLine("Configuring HTTP pipeline...");
    
    // Enable Response Compression (add early in pipeline)
    app.UseResponseCompression();
    Console.WriteLine("✓ Response compression enabled");
    
    // Enable Response Caching (add early in pipeline)
    app.UseResponseCaching();
    Console.WriteLine("✓ Response caching enabled");
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        logger.LogInformation("Swagger UI enabled for development environment");
        Console.WriteLine("✓ Swagger enabled for development");
    }
    else
    {
        // In production, still enable Swagger for API documentation but log it
        app.UseSwagger();
        app.UseSwaggerUI();
        logger.LogInformation("Swagger UI enabled for production environment");
        Console.WriteLine("✓ Swagger enabled for production");
    }

    // Enable CORS
    app.UseCors("AllowVueApp");
    logger.LogInformation("CORS policy 'AllowVueApp' enabled");
    Console.WriteLine("✓ CORS enabled");

    // Enable static files for image uploads
    var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? app.Environment.ContentRootPath, "uploads");
    if (!Directory.Exists(uploadsPath))
    {
        Directory.CreateDirectory(uploadsPath);
        Console.WriteLine($"✓ Created uploads directory: {uploadsPath}");
    }
    
    // Configure static files for uploads folder
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
        RequestPath = "/uploads"
    });
    
    // Also enable default static files for wwwroot
    app.UseStaticFiles();
    
    logger.LogInformation("Static files enabled for image serving from: {UploadsPath}", uploadsPath);
    Console.WriteLine($"✓ Static files enabled for uploads from: {uploadsPath}");

    app.UseHttpsRedirection();
    Console.WriteLine("✓ HTTPS redirection enabled");
    
    app.UseAuthorization();
    Console.WriteLine("✓ Authorization enabled");
    
    app.MapControllers();
    Console.WriteLine("✓ Controllers mapped");

    // Log successful startup
    logger.LogInformation("Cafe Menu API configuration completed successfully");
    Console.WriteLine($"✓ Application startup completed at {DateTime.UtcNow}");
    Console.WriteLine("=== STARTUP COMPLETE - APPLICATION RUNNING ===");

    app.Run();
}
catch (Exception startupEx)
{
    Console.WriteLine($"❌ FATAL STARTUP ERROR: {startupEx.Message}");
    Console.WriteLine($"Stack trace: {startupEx.StackTrace}");
    Console.WriteLine($"Inner exception: {startupEx.InnerException?.Message}");
    
    // Try to write to a simple log file
    try
    {
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "startup-error.log");
        await System.IO.File.WriteAllTextAsync(logPath, $"{DateTime.UtcNow}: {startupEx}");
        Console.WriteLine($"Error logged to: {logPath}");
    }
    catch
    {
        Console.WriteLine("Could not write error log file");
    }
    
    throw; // Re-throw to ensure the application doesn't start
}
