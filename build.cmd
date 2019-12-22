dotnet restore

dotnet build -c Release

dotnet publish -c Release src\AzureSqlAppIdentityAuthTool\AzureSqlAppIdentityAuthTool.csproj --output artifacts\AzureSqlAppIdentityAuthTool
