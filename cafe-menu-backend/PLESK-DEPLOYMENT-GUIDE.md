# ASP.NET Core Cafe Menu API - Plesk Deployment Guide

## 📦 Deployment Package
Your deployment package `CafeMenuApi-Deployment.zip` has been created and is ready for upload.

## 🚀 Step-by-Step Deployment to Plesk

### Step 1: Prepare Your Plesk Environment

1. **Create a new domain/subdomain** in Plesk (e.g., `api.yourdomain.com`)
2. **Enable ASP.NET Core support** in your Plesk hosting plan
3. **Create a SQL Server database** in Plesk:
   - Go to `Databases` → `Add Database`
   - Note down: Server name, Database name, Username, Password

### Step 2: Upload and Extract Files

1. **Upload the deployment package**:
   - Go to `File Manager` in Plesk
   - Navigate to your domain's `httpdocs` folder
   - Upload `CafeMenuApi-Deployment.zip`
   - Extract the zip file contents directly to `httpdocs`

2. **Alternative via FTP**:
   - Use FTP client (FileZilla, WinSCP, etc.)
   - Upload and extract to your domain's root directory

### Step 3: Database Connection (Pre-configured)

✅ **Database connection is already configured** with your specific details:
- **Server**: `.\MSSQLSERVER2022`
- **Database**: `pishedoo_MenuDb`
- **Username**: `pishedoo_MenuDbUser`
- **Password**: `_wYTY8l*9vkh8zsq`

The `appsettings.Production.json` file already contains:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\MSSQLSERVER2022;Database=pishedoo_MenuDb;User Id=pishedoo_MenuDbUser;Password=_wYTY8l*9vkh8zsq;TrustServerCertificate=true;"
  }
}
```

**No additional database configuration needed!** The application will automatically connect to your SQL Server instance.

### Step 4: Configure ASP.NET Core in Plesk

1. **Set the application**:
   - Go to your domain settings in Plesk
   - Click on `ASP.NET Core Settings`
   - Set the following:
     - **Application Startup File**: `CafeMenuApi.dll`
     - **Application Root Path**: `/httpdocs`
     - **Arguments**: (leave empty)
     - **Environment Variables**: `ASPNETCORE_ENVIRONMENT=Production`

2. **Set the .NET version**:
   - Ensure `.NET 8.0` is selected

### Step 5: CORS Configuration (Pre-configured)

✅ **CORS is already configured to allow any origin** - No additional configuration needed!

The application will accept requests from any domain, making it easy to integrate with your frontend regardless of where it's hosted.

### Step 6: Run Database Migrations

1. **Option A - Via Plesk terminal** (if available):
   ```bash
   cd /var/www/vhosts/yourdomain.com/httpdocs
   dotnet ef database update
   ```

2. **Option B - Pre-migration** (recommended):
   - The application will automatically run migrations on startup
   - Database tables and seed data will be created automatically

### Step 7: Test Your Deployment

1. **Test the API endpoints**:
   - `https://api.yourdomain.com/api/categories`
   - `https://api.yourdomain.com/api/menuitems`
   - `https://api.yourdomain.com/swagger` (if enabled)

2. **Test authentication**:
   ```bash
   curl -X POST https://api.yourdomain.com/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"admin123"}'
   ```

## 🔧 Configuration Files Summary

### Required Files in httpdocs:
- `CafeMenuApi.dll` (main application)
- `appsettings.json`
- `appsettings.Production.json` (with your database settings)
- `web.config` (automatically generated)
- All other DLL dependencies

### Environment Variables in Plesk:
- `ASPNETCORE_ENVIRONMENT=Production`

## 🐛 Troubleshooting

### Common Issues:

1. **500 Internal Server Error**:
   - Check the connection string in `appsettings.Production.json`
   - Verify database credentials
   - Check Plesk error logs

2. **Database Connection Failed**:
   - Verify SQL Server instance name
   - Check if database exists
   - Test connection string manually

3. **CORS Errors**:
   - Update CORS policy in `Program.cs`
   - Ensure your frontend domain is included

4. **File Permissions**:
   - Ensure IIS_IUSRS has read/write access to the application folder

### Checking Logs:
- Plesk Control Panel → Logs → Error Logs
- Or check the `logs` folder in your application directory

## 📱 Frontend Configuration

After successful backend deployment, update your Vue.js frontend:

1. **Update `src/services/api.js`**:
   ```javascript
   const API_BASE_URL = 'https://api.yourdomain.com/api';
   ```

2. **Build and deploy frontend**:
   ```bash
   npm run build
   # Upload dist/ contents to your main domain
   ```

## 🎉 Success!

Your ASP.NET Core Cafe Menu API should now be running on Plesk with:
- ✅ SQL Server database persistence
- ✅ Production-ready configuration
- ✅ CORS configured for your domain
- ✅ Automatic database migrations
- ✅ Admin authentication working

**Default Admin Credentials:**
- Username: `admin`
- Password: `admin123`

## 📞 Support

If you encounter issues:
1. Check Plesk error logs
2. Verify database connection
3. Ensure ASP.NET Core 8.0 is available
4. Contact your hosting provider for ASP.NET Core support 