# کافه پیش‌دوستام منو - Pisheh Dostam Cafe Menu

A complete cafe menu system with Vue.js frontend and ASP.NET Core backend.

## Features

- 🎨 **Beautiful Vue.js Frontend** - Modern, responsive Persian/Farsi interface
- 🔧 **ASP.NET Core Backend API** - RESTful API with Entity Framework
- 📱 **PWA Support** - Progressive Web App capabilities
- 🔐 **Admin Panel** - Full CRUD operations for categories and menu items
- 🌍 **CORS Enabled** - Frontend and backend integration
- 💾 **In-Memory Database** - Quick setup with pre-seeded data

## Architecture

```
Frontend (Vue.js)  ←→  Backend (ASP.NET Core API)
    Port: 8080            Port: 5268
       ↓                     ↓
  Static Assets         In-Memory Database
                       (Categories & Menu Items)
```

## Quick Start

### Prerequisites
- Node.js (v14+)
- .NET 8.0 SDK
- npm or yarn

### 1. Start the Backend API

```bash
cd cafe-menu-backend/CafeMenuApi
dotnet run
```

The API will be available at: `http://localhost:5268`

### 2. Start the Frontend

```bash
cd cafe-menu
npm install  # If not already installed
npm run serve
```

The frontend will be available at: `http://localhost:8080`

## Usage

### Public Menu
- Visit `http://localhost:8080` to view the public cafe menu
- Browse categories: قهوه (Coffee), چای (Tea), نوشیدنی سرد (Cold Drinks), etc.
- Click on items to view details and photo galleries

### Admin Panel
- Visit `http://localhost:8080?admin=login` to access the admin login
- **Username**: `admin`
- **Password**: `admin123`
- Manage categories and menu items with full CRUD operations

## API Endpoints

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Menu Items
- `GET /api/menuitems` - Get all menu items
- `GET /api/menuitems/{id}` - Get menu item by ID
- `GET /api/menuitems/category/{categoryId}` - Get items by category
- `POST /api/menuitems` - Create new menu item
- `PUT /api/menuitems/{id}` - Update menu item
- `DELETE /api/menuitems/{id}` - Delete menu item

### Authentication
- `POST /api/auth/login` - Admin login
- `POST /api/auth/verify` - Verify token

## Project Structure

```
PisheDoostamMenu/
├── cafe-menu/                 # Vue.js Frontend
│   ├── src/
│   │   ├── services/api.js    # API integration
│   │   ├── App.vue           # Main component
│   │   └── assets/           # Static assets
│   └── package.json
├── cafe-menu-backend/         # ASP.NET Core Backend
│   └── CafeMenuApi/
│       ├── Controllers/       # API controllers
│       ├── Models/           # Data models
│       ├── DTOs/             # Data transfer objects
│       └── Data/             # Database context
└── README.md
```

## Technology Stack

### Frontend
- **Vue.js 3** - Progressive JavaScript framework
- **Axios** - HTTP client for API calls
- **Tailwind CSS** - Utility-first CSS framework
- **PWA** - Progressive Web App features

### Backend
- **ASP.NET Core 8** - Cross-platform web framework
- **Entity Framework Core** - Object-relational mapping
- **In-Memory Database** - For development and testing
- **Swagger/OpenAPI** - API documentation

## Development Notes

- The backend uses an in-memory database, so data resets on server restart
- CORS is configured to allow the frontend to communicate with the backend
- Pre-seeded data includes sample categories and menu items
- File uploads are handled as base64 data URLs
- Authentication uses simple token-based system (not production-ready)

## Production Considerations

For production deployment:
1. Replace in-memory database with SQL Server/PostgreSQL
2. Implement proper password hashing
3. Use JWT tokens with proper expiration
4. Add input validation and sanitization
5. Configure proper CORS origins
6. Implement file upload to cloud storage
7. Add logging and error handling
8. Set up SSL/HTTPS

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License. 