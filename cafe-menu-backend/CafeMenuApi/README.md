# Cafe Menu API

A .NET 8 Web API backend for the Cafe Pish Doostam Menu application.

## Features

- **Categories Management**: CRUD operations for menu categories
- **Menu Items Management**: Full menu item management with image gallery support
- **Admin Authentication**: Simple token-based authentication
- **File Upload**: Image upload with base64 conversion
- **CORS Support**: Configured for Vue.js frontend
- **In-Memory Database**: Uses Entity Framework with in-memory storage

## API Endpoints

### Categories
- `GET /api/categories` - Get all categories with item counts
- `GET /api/categories/{id}` - Get specific category by ID
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Menu Items
- `GET /api/menuitems` - Get all menu items (optional: ?categoryId=1)
- `GET /api/menuitems/{id}` - Get specific menu item by ID
- `GET /api/menuitems/category/{categoryId}` - Get menu items by category
- `POST /api/menuitems` - Create new menu item
- `PUT /api/menuitems/{id}` - Update menu item
- `DELETE /api/menuitems/{id}` - Delete menu item

### Authentication
- `POST /api/auth/login` - Admin login
- `POST /api/auth/verify` - Verify authentication token

### File Upload
- `POST /api/upload/image` - Upload single image (returns base64 data URL)
- `POST /api/upload/images` - Upload multiple images (max 3, returns array of base64 data URLs)

## Data Models

### Category
```json
{
  "id": 1,
  "name": "قهوه",
  "icon": "data:image/jpeg;base64,/9j/4AAQ...",
  "description": "انواع قهوه‌های تازه و خوشمزه",
  "itemCount": 5
}
```

### Menu Item
```json
{
  "id": 1,
  "name": "اسپرسو",
  "price": 25000,
  "image": "data:image/jpeg;base64,/9j/4AAQ...",
  "description": "قهوه تلخ و قوی",
  "categoryId": 1,
  "categoryName": "قهوه",
  "photos": [
    "data:image/jpeg;base64,/9j/4AAQ...",
    "data:image/jpeg;base64,/9j/4AAQ...",
    "data:image/jpeg;base64,/9j/4AAQ..."
  ]
}
```

### Admin Login
```json
{
  "username": "admin",
  "password": "admin123"
}
```

### Admin Login Response
```json
{
  "success": true,
  "message": "ورود با موفقیت انجام شد",
  "token": "base64-encoded-token",
  "admin": {
    "id": 1,
    "username": "admin",
    "createdAt": "2025-01-01T00:00:00Z",
    "lastLoginAt": "2025-01-01T12:00:00Z"
  }
}
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio or VS Code

### Installation

1. Clone the repository
2. Navigate to the API directory:
   ```bash
   cd cafe-menu-backend/CafeMenuApi
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### Default Admin Credentials
- Username: `admin`
- Password: `admin123`

## Configuration

### CORS Settings
The API is configured to accept requests from:
- `http://localhost:8080` (Vue.js dev server)
- `http://localhost:3000` (Alternative frontend port)
- `http://localhost:5173` (Vite dev server)

### Database
Uses Entity Framework with In-Memory database. Data is seeded automatically on startup with:
- 6 default categories
- 5 sample menu items
- 1 admin user

## Development Notes

### File Upload
- Supports image files only
- Maximum file size: 5MB
- Returns base64 data URLs for storage
- Supports both single and multiple file uploads

### Authentication
- Uses simple base64 token encoding (for demo purposes)
- In production, implement proper JWT tokens
- Passwords are stored in plain text (for demo purposes)
- In production, use proper password hashing

### Database
- Currently uses in-memory database
- Data is lost when application restarts
- For production, configure SQL Server or PostgreSQL

## Production Considerations

1. **Security**:
   - Implement proper JWT authentication
   - Hash passwords with bcrypt or similar
   - Add input validation and sanitization
   - Implement rate limiting

2. **Database**:
   - Use persistent database (SQL Server, PostgreSQL)
   - Add database migrations
   - Implement proper error handling

3. **Performance**:
   - Add caching for frequently accessed data
   - Implement pagination for large datasets
   - Optimize database queries

4. **Deployment**:
   - Configure for your hosting environment
   - Set up environment-specific configurations
   - Add logging and monitoring

## Testing

The API includes comprehensive endpoints for all frontend functionality:
- Category management for admin panel
- Menu item CRUD operations
- Image upload for admin forms
- Authentication for admin access
- Public menu data for customer view

## Support

For issues or questions, please check the Swagger documentation at `/swagger` when running the application. 