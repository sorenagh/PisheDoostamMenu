# 🗄️ Database Reset Guide - Fresh Start

## ✅ Database Reset Functionality Added

I've added powerful database management tools to give you a completely fresh start while preserving your admin login.

## 🚀 New Endpoints Available

### 1. Database Status Check
**GET** `https://menuapi.pishedoostam.ir/api/Cleanup/database-status`

Shows current state:
- Number of menu items
- Number of categories (excluding "همه محصولات")
- Admin users count
- Uploaded files count
- Estimated database size

### 2. Complete Database Reset
**POST** `https://menuapi.pishedoostam.ir/api/Cleanup/reset-database`

**⚠️ DANGER ZONE - This will delete ALL data except admin login!**

What it does:
- ✅ **Preserves**: Admin login (username: `admin`, password: `admin123`)
- ❌ **Deletes**: ALL menu items
- ❌ **Deletes**: ALL categories (except "همه محصولات" with ID=0)
- ❌ **Deletes**: ALL uploaded image files from `/uploads/` directory
- 🧹 **Cleans**: File system and database completely

### 3. Image Analysis (Still Available)
**GET** `https://menuapi.pishedoostam.ir/api/Cleanup/analyze-images`

### 4. Base64 Migration (Still Available)
**POST** `https://menuapi.pishedoostam.ir/api/Cleanup/migrate-base64-images`

## 📋 Step-by-Step Reset Process

### Step 1: Deploy Updated Backend ⏳
1. Upload all files from `publish/CafeMenuApi/` to your backend hosting
2. Restart application pool
3. Test: `https://menuapi.pishedoostam.ir/api/health`

### Step 2: Check Current Status 📊
Visit: `https://menuapi.pishedoostam.ir/api/Cleanup/database-status`

Example response:
```json
{
  "menuItems": 25,
  "categories": 8,
  "adminUsers": 1,
  "uploadedFiles": 45,
  "databaseSize": "12.5 MB"
}
```

### Step 3: Reset Database 🗑️
**⚠️ BACKUP WARNING: This action cannot be undone!**

Visit: `https://menuapi.pishedoostam.ir/api/Cleanup/reset-database`

Expected response:
```json
{
  "success": true,
  "message": "Database reset completed successfully",
  "deleted": {
    "menuItems": 25,
    "categories": 8,
    "uploadedFiles": 45
  },
  "note": "Admin login preserved, all other data cleared"
}
```

### Step 4: Verify Clean State ✅
Visit status endpoint again: `https://menuapi.pishedoostam.ir/api/Cleanup/database-status`

Should show:
```json
{
  "menuItems": 0,
  "categories": 0,
  "adminUsers": 1,
  "uploadedFiles": 0,
  "databaseSize": "< 1 MB"
}
```

### Step 5: Deploy New Frontend (Optional) 🎨
If you want the optimized image upload system:
1. Upload all files from `dist/` to frontend hosting
2. This enables the new FormData upload system

## 🎯 What You Get After Reset

### ✅ Preserved:
- **Admin Login**: username `admin`, password `admin123`
- **Database Structure**: All tables and relationships intact
- **Application Code**: All functionality working

### 🧹 Cleaned:
- **Zero Menu Items**: Start adding products from scratch
- **Zero Categories**: Create your own category structure
- **Zero Images**: No old Base64 bloat or uploaded files
- **Minimal Database**: Reduced from potentially MBs to KBs

### 📈 Benefits:
- **Blazing Fast Performance**: No Base64 image bloat
- **Clean Slate**: Design your menu structure from scratch
- **Optimized System**: Start with the new file upload system
- **Professional Setup**: URL-based image management

## 🚨 Safety Notes

### Before Reset:
1. **Backup Important Data**: Save any menu items or categories you want to recreate
2. **Test on Staging**: If you have a test environment, try it there first
3. **Inform Users**: Let users know the menu will be temporarily empty

### After Reset:
1. **Verify Admin Login**: Test login at `https://menu.pishedoostam.ir/?admin=login`
2. **Test Image Upload**: Upload a test image to ensure new system works
3. **Create Categories**: Start building your category structure
4. **Add Menu Items**: Use the new optimized upload system

## 🛠️ Rebuilding Your Menu

### 1. Create Categories First
- Go to admin panel → Categories tab
- Add categories like: قهوه، چای، نوشیدنی سرد، دسر، etc.
- Upload category icons (will use new file system)

### 2. Add Menu Items
- Go to admin panel → Items tab
- Add products with main image + up to 3 gallery images
- All images will be stored as files (no more Base64!)

### 3. Test Performance
- Check frontend loading speed
- Test image upload speed
- Verify no timeouts

## 🔧 Technical Details

### Database Changes:
```sql
-- What gets deleted:
DELETE FROM MenuItems;                    -- All menu items
DELETE FROM Categories WHERE Id != 0;    -- All categories except "همه محصولات"

-- What stays:
SELECT * FROM Admins WHERE Username = 'admin'; -- Admin login preserved
```

### File System Changes:
```bash
# What gets deleted:
uploads/
├── image_abc123.jpg    ← Deleted
├── photo_def456.png    ← Deleted
└── image_ghi789.webp   ← Deleted

# Directory structure preserved:
uploads/                ← Empty but ready for new uploads
```

## 📞 If Something Goes Wrong

### Admin Login Issues:
- Username: `admin`
- Password: `admin123`
- URL: `https://menu.pishedoostam.ir/?admin=login`

### Backend Not Responding:
- Check application pool status
- Restart backend application
- Verify all files uploaded correctly

### Frontend Issues:
- Clear browser cache
- Re-upload frontend files
- Check console for errors

---

**Ready to start fresh? Deploy the updated backend and visit the reset endpoint!** 🚀 