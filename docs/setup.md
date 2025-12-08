# Setup Guide

This guide will help you set up the CSE325 E-Commerce project locally for development.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) (version 12 or later)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension
- [Node.js](https://nodejs.org/) (for frontend tooling, if needed)

## Database Setup

1. Install PostgreSQL and create a database:
   ```sql
   CREATE DATABASE cse325_ecommerce;
   ```

2. Update connection string in `backend/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=cse325_ecommerce;Username=your_username;Password=your_password"
     }
   }
   ```

## Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run database migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the backend API:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).

## Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update API base URL in `frontend/appsettings.json` if needed:
   ```json
   {
     "ApiBaseUrl": "https://localhost:5001"
   }
   ```

4. Run the frontend application:
   ```bash
   dotnet run
   ```

The application will be available at `https://localhost:5026`.

## Running Both Projects

You can run both backend and frontend simultaneously:

### Option 1: Using Visual Studio
1. Open `CSE325-ecommerce.sln` in Visual Studio
2. Set multiple startup projects (backend and frontend)
3. Press F5 to run

### Option 2: Using Terminal
1. Open two terminal windows
2. In first terminal: `cd backend && dotnet run`
3. In second terminal: `cd frontend && dotnet run`

## Development Workflow

1. Make changes to code
2. Backend changes: Restart the API server
3. Frontend changes: Hot reload should work automatically in Blazor
4. Test API endpoints using Swagger UI at `https://localhost:5001/swagger`

## Troubleshooting

### Common Issues

1. **Database connection errors**:
   - Ensure PostgreSQL is running
   - Verify connection string in `appsettings.json`

2. **CORS errors**:
   - Check that the frontend URL is allowed in backend CORS policy
   - Default allows `http://localhost:5026`

3. **Authentication issues**:
   - Ensure JWT configuration in `appsettings.json` is correct
   - Check token expiration and validation parameters

### Logs

- Backend logs are output to the console
- Frontend logs can be viewed in browser developer tools

## Additional Configuration

### Environment Variables

You can override settings using environment variables:

- `ASPNETCORE_ENVIRONMENT`: Set to `Development` or `Production`
- `ConnectionStrings__DefaultConnection`: Database connection string
- `Jwt__Key`: JWT signing key
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audiences`: Comma-separated list of audiences

### HTTPS Certificates

For development, .NET automatically creates self-signed certificates. If you encounter certificate issues:

1. Trust the development certificate:
   ```bash
   dotnet dev-certs https --trust
   ```

2. Clear browser cache or use incognito mode

## Next Steps

Once set up, you can:
- Explore the API using Swagger
- Navigate the frontend application
- Start developing new features
- Run tests (when available)

For more information, see the [Architecture Guide](architecture.md) and [API Reference](api.md).