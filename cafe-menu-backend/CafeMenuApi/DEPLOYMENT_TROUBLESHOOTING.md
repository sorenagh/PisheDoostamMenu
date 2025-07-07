# Deployment Troubleshooting Guide

This guide helps you identify and troubleshoot errors when the Cafe Menu API is deployed to a hosting environment.

## Error Visibility Features Added

### 1. Health Check Endpoints
The application now includes health check endpoints to monitor application status:

- **Basic Health Check**: `GET /api/health`
  - Returns application status, database connectivity, and basic system info
  - Use this for quick health verification

- **Detailed Health Check**: `GET /api/health/detailed`
  - Returns comprehensive system information including server details, database info, and configuration
  - Use this for detailed troubleshooting

### 2. Comprehensive Logging

#### Console Logging
- Application logs are written to console output
- Visible in hosting platform logs (Azure App Service, IIS, etc.)

#### File Logging (if NLog is configured)
- Logs are written to `logs/` directory
- **All logs**: `logs/cafe-menu-api-{date}.log`
- **Error logs only**: `logs/cafe-menu-api-errors-{date}.log`
- **Internal logs**: `logs/nlog-internal.log`

#### Log Levels
- **Information**: Normal operations, startup, database connections
- **Warning**: Non-critical issues
- **Error**: Exceptions and failures
- **Critical**: Application-level failures

### 3. Global Exception Handling
- All unhandled exceptions are caught and logged
- Detailed error responses in development
- Sanitized error responses in production
- Full stack traces logged to error files

### 4. IIS/Hosting Configuration
- **stdout logging enabled**: Application output captured by IIS
- **Detailed errors enabled**: IIS shows detailed error pages
- **Custom errors disabled**: Shows actual error details instead of generic pages

## How to View Errors After Deployment

### 1. Check Health Endpoints
```
GET https://yourdomain.com/api/health
GET https://yourdomain.com/api/health/detailed
```

### 2. Azure App Service
- Go to Azure Portal → Your App Service → Log stream
- Or use Kudu console: `https://yourapp.scm.azurewebsites.net`
- Check `LogFiles/Application` folder

### 3. IIS Hosting
- Check IIS logs in: `C:\inetpub\logs\LogFiles\W3SVC1\`
- Application logs in: `{app-directory}/logs/`
- stdout logs in: `{app-directory}/logs/stdout`

### 4. Shared Hosting (cPanel, Plesk, etc.)
- Look for "Error Logs" in hosting control panel
- Check application's `logs/` folder if accessible via file manager
- stdout logs typically in `logs/stdout` or similar

### 5. Docker Containers
```bash
# View container logs
docker logs [container-name]

# View application logs inside container
docker exec -it [container-name] cat /app/logs/cafe-menu-api-errors-{date}.log
```

## Common Error Scenarios

### Database Connection Issues
- **Error**: "Cannot connect to database"
- **Check**: Connection string in `appsettings.Production.json`
- **Debug**: Visit `/api/health/detailed` to see database status

### Missing Dependencies
- **Error**: "Could not load file or assembly"
- **Check**: Ensure all NuGet packages are included in publish
- **Debug**: Check application startup logs

### Configuration Issues
- **Error**: "Configuration value not found"
- **Check**: Environment variables and `appsettings.Production.json`
- **Debug**: Health endpoint shows configuration values

### File Permissions
- **Error**: "Access denied" or "Cannot write to logs"
- **Check**: Application has write permissions to `logs/` folder
- **Fix**: Set appropriate folder permissions on hosting server

## Environment Variables for Enhanced Debugging

Set these in your hosting environment for maximum error visibility:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_DETAILEDERRORS=true
ASPNETCORE_CAPTURESTARTUPERRORS=true
```

## Log File Locations by Platform

| Platform | Log Location |
|----------|-------------|
| Azure App Service | `/home/LogFiles/Application/` |
| IIS | `{app-root}/logs/` |
| Linux/Docker | `/app/logs/` |
| Shared Hosting | Usually `{domain}/logs/` or check control panel |

## Quick Troubleshooting Checklist

1. ✅ Check health endpoint: `/api/health`
2. ✅ Verify database connectivity in detailed health check
3. ✅ Check hosting platform's error logs
4. ✅ Look for stdout logs in hosting console
5. ✅ Verify file permissions for logs folder
6. ✅ Check that all configuration files are uploaded
7. ✅ Ensure database connection string is correct for production

## Getting Support

When reporting issues, please include:
- URL to health check endpoint output
- Error messages from hosting platform logs
- Contents of latest error log file (if accessible)
- Environment details from detailed health check

This information will help quickly identify and resolve deployment issues. 