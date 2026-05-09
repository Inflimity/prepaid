# Prepaid Balance API 

This API uses Microsoft SQL Server and stores financial data securely as Integers (Kobo) in the database while exposing Decimals (NGN) to the client to prevent float rounding issues.

**Mapping benefit:** Please run `docker-compose up -d` and your environment will match this project's perfectly. 
- It automatically provisions an MS SQL Server container (`sql_server`) running on port `1433`.
- It automatically accepts the EULA and sets the specific SA password required by the application.

##  Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop) (Optional but highly recommended)
- A Database Client (TablePlus, DBeaver, or Azure Data Studio)

## Quick Start with Docker (Recommended)

To make environment setup as frictionless as possible, i have included a Docker orchestration file. 

Instead of manually installing and configuring Microsoft SQL Server, simply run the following command in the root directory:

```bash
docker-compose up -d
```


## Configuration Setup & Transparency

I have provided an `appsettings.json.example` file. This shows the exact structure of the configuration.

1. Copy the example file:
   ```bash
   cp PrepaidApi/appsettings.json.example PrepaidApi/appsettings.json
   ```
2. Open `appsettings.json` and ensure the `DefaultConnection` matches your Docker SQL Server credentials:
   ```json
   "DefaultConnection": "Server=127.0.0.1,1433;Database=PrepaidDb;User Id=sa;Password=StoicCoder@2026!;TrustServerCertificate=True;MultipleActiveResultSets=true"
   ```

## Database Initialization

I used TablePlus bacause i am using MAC and windows MSsql is not in support of mac, so i used docker to run MSsql in my local machine. 

Before running the API, you must create the database and table schema. Connect to your SQL Server instance using TablePlus (or any client) on `127.0.0.1:1433` (User: `sa`, Password: `StoicCoder@2026!`) and run this SQL script in the query window: 

```sql
CREATE DATABASE PrepaidDb;
GO
USE PrepaidDb;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber BIGINT NOT NULL,
    Balance INT NOT NULL DEFAULT 0,
    LastUpdated DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
CREATE UNIQUE INDEX IX_Users_PhoneNumber ON Users(PhoneNumber);
```

## Running the Application

Once the database is running and configured, start the API:

```bash
cd PrepaidApi
dotnet run
```
The application will automatically launch the **Swagger UI** in your browser at `http://localhost:5193/swagger`.

## Security Features

This API is fully secured for production use:
1. **API Key Authentication**: All endpoints require an `X-Api-Key` header.
2. **Rate Limiting**: Globally restricted to 10 requests per second to prevent abuse.
3. **CORS Policy**: Restricts API access to explicitly whitelisted frontend domains.
4. **Global Exception Handling**: Prevents stack-trace leaks by returning standardized JSON errors.
5. **Precision Storage**: Converts input decimals to integers backend to prevent float rounding errors.

### Testing Endpoints
Since the API is locked down with an API Key, you must pass the key defined in your `appsettings.json` inside the Headers of your HTTP requests (via Postman, curl, or frontend):
- **Key**: `X-Api-Key`
- **Value**: `super_secure_prepaid_api_key_2026`
