
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing"))
        )
    };
});

builder.Services.AddScoped<ITokenService, TokenService>();

// =======================================================
// 2. DATABASE CONTEXT
// =======================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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
