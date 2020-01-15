@echo off
dotnet restore

dotnet build --no-restore -c Release

dotnet publish --no-build -c Release src\AzureSqlAppIdentityAuthTool\AzureSqlAppIdentityAuthTool.csproj --output artifacts\AzureSqlAppIdentityAuthTool
