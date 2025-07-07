# Backend 404 Troubleshooting Guide

## Fresh Backend Published ✅
The backend has been rebuilt and published to `publish/CafeMenuApi/` with the latest image optimization changes.

## Common Causes of 404 Errors

### 1. File Upload Issues
**Problem:** Files not uploaded to correct location
**Check:**
- Ensure all files from `publish/CafeMenuApi/` are uploaded to your hosting root
- Main files to verify: `CafeMenuApi.dll`, `web.config`, `appsettings.Production.json`

### 2. IIS Configuration Issues
**Problem:** ASP.NET Core module not configured
**Solutions:**
- Install ASP.NET Core Hosting Bundle on server
- Restart IIS after installation
- Check Application Pool .NET version is set to "No Managed Code"

### 3. Application Pool Issues
**Problem:** Application pool stopped or crashed
**Check in hosting control panel:**
- Application Pool Status should be "Started"
- If stopped, restart the application pool
- Check Application Pool Identity has proper permissions

### 4. Web.config Issues
**Problem:** Incorrect web.config configuration
**Our web.config includes:**
```xml
<aspNetCore processPath="dotnet" arguments=".\CafeMenuApi.dll" 
            stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" 
            hostingModel="InProcess" />
```

### 5. Missing .NET Runtime
**Problem:** Server doesn't have .NET 8 runtime
**Solution:** Ask hosting provider to install .NET 8 ASP.NET Core Runtime

## Diagnostic Steps

### Step 1: Check Basic Connectivity
Try these URLs:
- `https://menuapi.pishedoostam.ir/` (should show some response, not necessarily 200)
- `https://menuapi.pishedoostam.ir/api/health` (our health check)

### Step 2: Check Server Logs
Look for:
- IIS logs in hosting control panel
- Application logs in `/logs/` directory (if created)
- Windows Event Viewer for ASP.NET Core errors

### Step 3: Verify File Structure
Your server should have:
```
wwwroot/ (or site root)
├── CafeMenuApi.dll          ← Main application
├── web.config               ← IIS configuration
├── appsettings.Production.json ← Production settings
├── CafeMenuApi.deps.json    ← Dependencies
├── CafeMenuApi.runtimeconfig.json ← Runtime config
├── runtimes/                ← Native libraries
└── uploads/                 ← Will be created automatically
```

### Step 4: Test Different Endpoints
After fixing 404, test:
- `GET /api/health` - Health check
- `GET /api/Diagnostics/ping` - Simple ping
- `GET /api/Categories` - Categories endpoint

## Quick Fixes to Try

### Fix 1: Restart Application Pool
In your hosting control panel:
1. Find "Application Pools" or "App Pool"
2. Find your site's app pool
3. Click "Recycle" or "Restart"

### Fix 2: Check File Permissions
Ensure the Application Pool Identity has:
- Read access to all application files
- Write access to create `/logs/` and `/uploads/` directories

### Fix 3: Verify .NET Version
In Application Pool settings:
- .NET CLR Version: "No Managed Code"
- Managed Pipeline Mode: "Integrated"

### Fix 4: Re-upload Key Files
At minimum, re-upload:
1. `CafeMenuApi.dll`
2. `web.config`
3. `appsettings.Production.json`

## Hosting Provider Specific

### If using cPanel/Plesk:
1. Go to "File Manager"
2. Navigate to public_html or httpdocs
3. Upload all files from `publish/CafeMenuApi/`
4. Set folder permissions to 755, files to 644

### If using dedicated hosting:
1. Use FTP/SFTP to upload files
2. Check IIS Manager for application configuration
3. Verify ASP.NET Core module is installed

## Testing the Fix

Once uploaded, test in this order:
1. **Basic connectivity:** `https://menuapi.pishedoostam.ir/`
2. **Health check:** `https://menuapi.pishedoostam.ir/api/health`
3. **Ping test:** `https://menuapi.pishedoostam.ir/api/Diagnostics/ping`
4. **Data endpoint:** `https://menuapi.pishedoostam.ir/api/Categories`

Expected responses:
- Health: `{"status":"Healthy",...}`
- Ping: `{"status":"alive",...}`
- Categories: Array of category objects

## If Still Getting 404

### Contact Hosting Support
Ask them to check:
1. Is .NET 8 ASP.NET Core Runtime installed?
2. Is the application pool running?
3. Are there any errors in the server logs?
4. Is the ASP.NET Core module working?

### Alternative: Self-Contained Deployment
If runtime issues persist, I can create a self-contained deployment that includes all .NET dependencies.

## Files Ready for Upload
All files are in: `publish/CafeMenuApi/`
Key files: 58 files totaling ~7.2MB

---

**Next Step:** Upload all files from `publish/CafeMenuApi/` to your hosting provider's root directory and restart the application pool. 