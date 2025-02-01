# Simple ASP.NET Core Web Application

This is a basic ASP.NET Core 8.0 web application with static file support.

## Project Structure
- `Program.cs` - Main application entry point and configuration
- `wwwroot/` - Static files directory
- `appsettings.json` - Application configuration
- `appsettings.Development.json` - Development-specific configuration

## Running the Application

1. Ensure you have .NET 8.0 SDK installed
2. Open a terminal in the project directory
3. Run the following commands:
   ```
   dotnet restore
   dotnet run
   ```
4. Access the application:
   - Static page: `https://localhost:5001/index.html`
   - API endpoint: `https://localhost:5001/`

## Development
- Static files should be placed in the `wwwroot` directory
- The application uses the default development configuration when running locally