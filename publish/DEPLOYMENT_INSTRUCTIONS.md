# Cafe Menu API - Deployment Instructions

This folder contains the production-ready build of your Cafe Menu API with comprehensive error handling and logging.

## 📦 What's Included

- **Complete backend API** with all dependencies
- **Global exception handling** for error visibility  
- **Health check endpoints** for monitoring
- **Comprehensive logging** (console + file-based)
- **Production-optimized configuration**
- **IIS/hosting server configuration**

## 🚀 Deployment Steps

### 1. **Upload Files**
Upload the entire `CafeMenuApi/` folder to your hosting provider:
- Via FTP/SFTP to your domain's root or subdirectory
- Via hosting control panel file manager
- Via deployment tools (if supported)

### 2. **Update Configuration** 
Edit `appsettings.Production.json` with your production database connection:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_PRODUCTION_DATABASE_CONNECTION_STRING"
  }
}
```

### 3. **Set Folder Permissions**
Ensure the application has:
- **Read** permissions on all files
- **Write** permissions on `logs/` folder (for log files)
- **Execute** permissions on `CafeMenuApi.exe`

### 4. **Configure Web Server**

#### **IIS (Windows Hosting)**
- The `web.config` is already configured
- Ensure ASP.NET Core Runtime is installed
- Point your site to the `CafeMenuApi/` folder

#### **Apache/Nginx (Linux Hosting)**
- Configure reverse proxy to `http://localhost:5000`
- Ensure .NET 8 Runtime is installed
- Run: `dotnet CafeMenuApi.dll`

#### **Shared Hosting (cPanel/Plesk)**
- Upload to `public_html/` or subdirectory
- Configure web.config if supported
- May need to contact hosting support for .NET Core setup

## 🔍 Testing & Verification

### **1. Basic Health Check**
Visit: `https://yourdomain.com/api/health`

**Expected Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-12-30T...",
  "environment": "Production",
  "checks": {
    "database": "Connected",
    "application": "OK"
  }
}
```

### **2. Detailed Health Check**
Visit: `https://yourdomain.com/api/health/detailed`

This provides comprehensive system information for troubleshooting.

### **3. API Documentation**
Visit: `https://yourdomain.com/swagger`

Access the Swagger UI to test all API endpoints.

## 📊 Monitoring & Logs

### **Where to Find Logs**

| Platform | Log Location |
|----------|-------------|
| **IIS** | `logs/` folder + IIS logs |
| **Azure App Service** | Log Stream in Azure Portal |
| **Linux/Docker** | Console output + `logs/` folder |
| **Shared Hosting** | Check hosting control panel |

### **Log Files Created**
- `logs/cafe-menu-api-{date}.log` - All application logs
- `logs/cafe-menu-api-errors-{date}.log` - Error logs only
- `logs/nlog-internal.log` - Logging system diagnostics

## 🚨 Troubleshooting

### **Application Won't Start**
1. Check health endpoint: `/api/health`
2. Verify .NET 8 Runtime is installed
3. Check file permissions
4. Review hosting platform logs

### **Database Connection Issues**
1. Verify connection string in `appsettings.Production.json`
2. Check database server accessibility
3. Visit `/api/health/detailed` for database status

### **500 Internal Server Error**
1. Check `logs/cafe-menu-api-errors-{date}.log`
2. Enable detailed errors (already configured)
3. Check stdout logs in hosting platform

### **404 Not Found for API Endpoints**
1. Verify correct URL: `https://yourdomain.com/api/health`
2. Check hosting configuration (subfolder setup)
3. Ensure routing is working via Swagger

## 🔧 Common Hosting Configurations

### **Subdirectory Hosting**
If hosting in a subdirectory (e.g., `yourdomain.com/api/`):
1. Upload files to `public_html/api/`
2. API endpoints become: `yourdomain.com/api/api/health`
3. Update frontend API base URL accordingly

### **Custom Domain**
1. Point domain to hosting folder
2. Update CORS settings if needed
3. Update frontend API base URL

### **HTTPS Configuration**
1. Ensure SSL certificate is configured
2. API should automatically redirect HTTP to HTTPS
3. Update frontend to use HTTPS URLs

## 📞 Support Information

When reporting issues, please include:
- Output from: `/api/health/detailed`
- Error messages from hosting platform logs
- Contents of `logs/cafe-menu-api-errors-{date}.log`
- Hosting platform details (IIS, Apache, shared hosting, etc.)

## 🔄 Future Updates

To republish after code changes:
1. Run `publish.bat` in the backend folder
2. Upload the new `CafeMenuApi/` folder
3. Test the health check endpoint
4. Monitor logs for any issues

---

**🎉 Your Cafe Menu API is now ready for production with comprehensive error visibility and monitoring!** 