# IIS Deployment Guide - Frontend Fix

## 🎯 Current Situation
- **Server**: IIS (confirmed)
- **Working**: `https://menu.pishedoostam.ir/index.html` ✅
- **Not Working**: `https://menu.pishedoostam.ir/` ❌
- **Issue**: Missing or incorrect web.config for IIS

## 🔧 Solution Applied

### Minimal IIS web.config Created
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <defaultDocument>
      <files>
        <clear />
        <add value="index.html" />
      </files>
    </defaultDocument>
    
    <rewrite>
      <rules>
        <rule name="SPA Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/(api)" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

## 🚀 Deployment Steps

### Step 1: Upload the New web.config
1. Upload the updated `web.config` from `dist/web.config`
2. Replace any existing web.config on your server
3. Ensure it's in the root directory with index.html

### Step 2: Verify IIS Modules
The web.config requires these IIS modules (usually pre-installed):
- **URL Rewrite Module** - for SPA routing
- **Default Document Module** - for index.html as default

### Step 3: Test the Fix
1. Visit `https://menu.pishedoostam.ir/` (should now work)
2. Test internal navigation within the Vue.js app
3. Verify API calls work properly

## 🔍 What This Config Does

### Default Document
- Sets `index.html` as the default file for root directory
- When someone visits `/`, IIS serves `/index.html`

### URL Rewriting
- Catches all requests that aren't files or directories
- Excludes API calls (preserves `/api/` routes)
- Redirects everything else to `index.html` for Vue.js routing

## 🚨 Troubleshooting

### If Still Getting 500 Error:

**Option 1: Check URL Rewrite Module**
```
- Ask hosting provider if URL Rewrite module is installed
- Some shared hosting doesn't include this module
```

**Option 2: Try Even Simpler Config**
If the current web.config still causes issues, try this ultra-minimal version:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <defaultDocument>
      <files>
        <add value="index.html" />
      </files>
    </defaultDocument>
  </system.webServer>
</configuration>
```

**Option 3: Contact Hosting Provider**
- Ask them to check IIS error logs
- Verify URL Rewrite module is available
- Request help with SPA configuration

## ✅ Expected Results

After uploading the corrected web.config:
- ✅ `https://menu.pishedoostam.ir/` loads the Vue.js app
- ✅ Direct navigation to app routes works
- ✅ Browser refresh on any route works
- ✅ API calls to `https://menuapi.pishedoostam.ir/api` function

## 📁 Files in Your dist/ Folder

Correct files for IIS deployment:
- ✅ `index.html` - Main application
- ✅ `web.config` - IIS configuration (updated)
- ✅ `css/` - Stylesheets
- ✅ `js/` - JavaScript files
- ✅ `favicon.ico` - Site icon
- ❌ `.htaccess` - Removed (Apache-only)

## 🔄 Future Builds

The build system is now configured for IIS:
- `web.config` automatically copied from `public/web.config`
- `.htaccess` no longer generated
- Compatible with IIS hosting requirements

Upload the new `web.config` and test - the root URL should work immediately! 🎉 