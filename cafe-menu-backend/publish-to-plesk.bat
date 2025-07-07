@echo off
echo Publishing ASP.NET Core application for Plesk hosting...

cd CafeMenuApi

echo Step 1: Cleaning previous builds...
dotnet clean

echo Step 2: Building application in Release mode...
dotnet build --configuration Release

echo Step 3: Publishing application...
dotnet publish --configuration Release --output ../publish --runtime win-x64 --self-contained false

echo Step 4: Creating deployment package...
cd ../publish
powershell Compress-Archive -Path * -DestinationPath ../CafeMenuApi-Deployment.zip -Force

cd ..
echo.
echo ✅ Deployment package created: CafeMenuApi-Deployment.zip
echo.
echo Next steps:
echo 1. Upload CafeMenuApi-Deployment.zip to your Plesk hosting
echo 2. Extract it to your website root or app folder
echo 3. Update appsettings.Production.json with your database details
echo 4. Configure IIS/Plesk to point to your application
echo.
pause 