
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Configuration is automatically loaded by CreateBuilder in this order:
// 1. appsettings.json
// 2. appsettings.{Environment}.json
// 3. Environment variables
// 4. Command line arguments
// Just ensure we're reading from environment variables as well
builder.Configuration.AddEnvironmentVariables();

// =======================================================
// 1. AUTENTICAÇÃO JWT BEARER
// =======================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    
    // If JWT Key is not configured, disable JWT validation
    if (string.IsNullOrEmpty(jwtKey))
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };
        return;
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        // Lê Issuer e múltiplos Audiences do appsettings.json
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudiences = builder.Configuration.GetSection("Jwt:Audiences").Get<string[]>(),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };
});

builder.Services.AddScoped<ITokenService, TokenService>();

// =======================================================
// 2. DATABASE CONTEXT
// =======================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If connection string is not found in appsettings, try environment variable
if (string.IsNullOrEmpty(connectionString))
{
    var envConnStr = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_DEFAULTCONNECTION") 
                     ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    if (!string.IsNullOrEmpty(envConnStr))
    {
        connectionString = envConnStr;
    }
}

// If still not found, try to build from individual env vars
if (string.IsNullOrEmpty(connectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? builder.Configuration["DB_HOST"];
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? builder.Configuration["DB_NAME"];
    var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? builder.Configuration["DB_USER"];
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? builder.Configuration["DB_PASSWORD"];
    
    if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbName) && !string.IsNullOrEmpty(dbUser))
    {
        connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPassword ?? ""};Port=5432";
    }
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// =======================================================
// 3. CORS Policy (Permitir Blazor durante DEV)
// =======================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://localhost:5026") // URL do Blazor WebAssembly
                   .AllowAnyHeader()
                   .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =======================================================
// MIDDLEWARE
// =======================================================

// Swagger apenas em DEV
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Error handling middleware - to catch and log exceptions
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            message = "Internal Server Error",
            detail = exception?.Message,
            stackTrace = exception?.StackTrace,
            innerException = exception?.InnerException?.Message
        };
        
        await context.Response.WriteAsJsonAsync(response);
    });
});

// Redireciona para HTTPS
app.UseHttpsRedirection();

// Ativa CORS
app.UseCors("AllowBlazor");

// Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Diagnostics endpoint
app.MapGet("/diagnostics", (IConfiguration config) =>
{
    var connStr = config.GetConnectionString("DefaultConnection");
    var dbHost = config["DB_HOST"];
    var dbName = config["DB_NAME"];
    var dbUser = config["DB_USER"];
    var jwtKey = config["Jwt:Key"];
    
    return Results.Ok(new
    {
        environment = builder.Environment.EnvironmentName,
        connectionStringConfigured = !string.IsNullOrEmpty(connStr),
        connectionStringPreview = connStr != null ? $"{connStr[..Math.Min(30, connStr.Length)]}..." : "NOT FOUND",
        dbEnvVarsConfigured = !string.IsNullOrEmpty(dbHost),
        dbHost = dbHost,
        dbName = dbName,
        dbUser = dbUser,
        jwtKeyConfigured = !string.IsNullOrEmpty(jwtKey)
    });
});

// Health check endpoint
app.MapGet("/health", async (AppDbContext context) =>
{
    try
    {
        // Test database connection
        await context.Database.ExecuteSqlAsync($"SELECT 1");
        return Results.Ok(new { status = "healthy", database = "connected" });
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "unhealthy", error = ex.Message }, statusCode: 500);
    }
});

// Test Products endpoint
app.MapGet("/test-products", async (AppDbContext context) =>
{
    try
    {
        var count = await context.Products.CountAsync();
        var sample = await context.Products.Take(1).ToListAsync();
        return Results.Ok(new { count, sample });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message, stackTrace = ex.StackTrace }, statusCode: 500);
    }
});

app.Run();
