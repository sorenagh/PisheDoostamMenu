@echo off
echo ========================================
echo  Cafe Menu API - Production Publish
echo ========================================
echo.

cd CafeMenuApi

echo [1/3] Cleaning previous build...
dotnet clean -c Release
if errorlevel 1 (
    echo ERROR: Clean failed!
    pause
    exit /b 1
)

echo [2/3] Building in Release mode...
dotnet build -c Release
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo [3/3] Publishing to ../publish/CafeMenuApi...
dotnet publish -c Release -o ../publish/CafeMenuApi --self-contained false
if errorlevel 1 (
    echo ERROR: Publish failed!
    pause
    exit /b 1
)

cd ..

echo.
echo ========================================
echo  PUBLISH COMPLETED SUCCESSFULLY! 
echo ========================================
echo.
echo Published files are in: publish/CafeMenuApi/
echo.
echo NEXT STEPS:
echo 1. Upload the entire 'publish/CafeMenuApi/' folder to your host
echo 2. Update connection string in appsettings.Production.json if needed
echo 3. Test health check: https://yourdomain.com/api/health
echo 4. Check logs if any issues occur
echo.
echo For troubleshooting, see: CafeMenuApi/DEPLOYMENT_TROUBLESHOOTING.md
echo.
pause 