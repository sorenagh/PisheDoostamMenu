# Backend API Troubleshooting Guide

## 🚨 Current Issue: API Timeout Error

**Error**: `timeout of 10000ms exceeded`
**Cause**: Backend API at `https://menuapi.pishedoostam.ir/api` is not responding
**Impact**: Frontend cannot load categories, menu items, or admin functions

## 🔍 Diagnostic Steps

### Step 1: Test API Connectivity
Try these URLs in your browser:

1. **Basic Test**: `https://menuapi.pishedoostam.ir/api/test`
2. **Health Check**: `https://menuapi.pishedoostam.ir/api/health`
3. **Categories**: `https://menuapi.pishedoostam.ir/api/categories`

If any return 404, 500, or timeout - the backend is down.

### Step 2: Check Backend Server Status
- Log into your hosting provider control panel
- Check if the .NET application is running
- Look for any error logs
- Verify the application pool is started (if IIS)

### Step 3: Database Connection
- Ensure the database server is running
- Check database connection strings
- Verify database user permissions

## 🚀 Quick Fixes to Try

### Fix 1: Restart the Backend Application
1. Log into your hosting control panel
2. Find your .NET application
3. Stop and restart the application
4. Wait 1-2 minutes for startup
5. Test `https://menuapi.pishedoostam.ir/api/test`

### Fix 2: Check Application Pool (IIS)
If using IIS hosting:
1. Go to IIS Manager (or hosting control panel)
2. Find your application pool
3. Check if it's stopped - restart it
4. Set it to "Always Running" if possible

### Fix 3: Re-deploy Backend
If restart doesn't work:
1. Re-upload your backend files from `publish/CafeMenuApi/`
2. Ensure all dependencies are included
3. Check web.config settings
4. Restart the application

## 🔧 Environment Variables Check

Ensure these are properly configured:
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=[your database connection]
DetailedErrors=true
CaptureStartupErrors=true
```

## 📋 Common Causes & Solutions

### Database Issues
- **Problem**: Database server down or connection string wrong
- **Solution**: Check database hosting status, verify connection string

### Memory/Resource Limits
- **Problem**: Shared hosting resource limits exceeded
- **Solution**: Contact hosting provider, check usage limits

### Application Crash
- **Problem**: Unhandled exception causing app to crash
- **Solution**: Check error logs, re-deploy with latest fixes

### SSL Certificate Issues
- **Problem**: HTTPS certificate expired or misconfigured
- **Solution**: Renew SSL certificate, check HTTPS bindings

## 🧪 Test Commands

### Test with Curl:
```bash
curl -X GET https://menuapi.pishedoostam.ir/api/test
curl -X GET https://menuapi.pishedoostam.ir/api/health
```

### Test from Frontend Console:
```javascript
fetch('https://menuapi.pishedoostam.ir/api/test')
  .then(r => r.text())
  .then(console.log)
  .catch(console.error)
```

## 📞 Contact Hosting Provider

If none of the above work, contact your hosting provider with:
1. Error details (timeout, 500 error, etc.)
2. Application type (.NET 8 Core)
3. Request to check server logs
4. Ask them to restart your application

## ⚡ Emergency Workaround

While fixing the backend, you can:
1. Use the frontend test pages to verify it works
2. Show demo data instead of API data
3. Display a maintenance message to users

## 🔄 Prevention

### For Future:
1. Set up uptime monitoring for your API
2. Enable detailed logging in production
3. Configure automatic application restarts
4. Keep database connection strings updated

The most likely solution is restarting your backend application through your hosting provider's control panel. 