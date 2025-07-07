# Server Configuration Files for Frontend

The frontend includes server configuration files for different hosting environments to ensure proper Vue.js SPA (Single Page Application) functionality.

## 📁 Files Created

### 1. `web.config` (IIS/Windows Hosting)
- **Location**: `cafe-menu/dist/web.config`
- **Purpose**: Configuration for Microsoft IIS web servers
- **Features**:
  - URL rewriting for SPA routing
  - MIME type mappings
  - Security headers
  - Compression settings
  - Browser caching (1 year for static assets)
  - 404 error handling (redirects to index.html)

### 2. `.htaccess` (Apache Hosting)
- **Location**: `cafe-menu/dist/.htaccess`
- **Purpose**: Configuration for Apache web servers
- **Features**:
  - URL rewriting for client-side routing
  - Security headers
  - Gzip compression
  - Browser caching
  - Content type optimization

## 🔧 What These Files Do

### SPA Routing Support
Both files ensure that when users navigate to routes like `/menu` or `/categories`, the server serves the `index.html` file instead of returning a 404 error. This is essential for Vue.js router to work properly.

### Performance Optimization
- **Compression**: Reduces file sizes for faster loading
- **Caching**: Tells browsers to cache static files for better performance
- **MIME Types**: Ensures files are served with correct content types

### Security Headers
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: SAMEORIGIN`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: strict-origin-when-cross-origin`

## 🚀 Deployment Instructions

### For IIS Servers (Windows Hosting)
1. Upload all files from `dist/` folder to your web root
2. The `web.config` file will be automatically recognized by IIS
3. Ensure URL Rewrite module is installed on the server

### For Apache Servers (Linux/cPanel Hosting)
1. Upload all files from `dist/` folder to your web root  
2. The `.htaccess` file will be automatically processed by Apache
3. Ensure mod_rewrite is enabled on the server

### For Other Servers (Nginx, etc.)
If using Nginx or other servers, you'll need to configure similar rules manually:

#### Nginx Example
```nginx
location / {
  try_files $uri $uri/ /index.html;
}
```

## 🔄 Automatic Build Integration

Both configuration files are automatically copied during the build process:
- Source files: `cafe-menu/public/web.config` and `cafe-menu/public/.htaccess`  
- Destination: `cafe-menu/dist/` (copied during `npm run build`)
- Configuration: Set up in `vue.config.js`

## 🛠️ Testing

To verify the configuration works:
1. Deploy your application
2. Navigate directly to a route like `yoursite.com/menu`
3. The page should load correctly (not show 404)
4. Check browser developer tools for proper compression and caching headers

## 📝 Notes

- Choose the appropriate file for your hosting environment
- You typically only need one (either `web.config` OR `.htaccess`)
- Both files are included for maximum compatibility
- The files are optimized for Vue.js SPA applications with Persian (RTL) content 