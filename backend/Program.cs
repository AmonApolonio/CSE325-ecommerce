using Microsoft.EntityFrameworkCore;
using Npgsql;
using backend.Models; // Your Models namespace
using backend.Data; // Assuming EcommerceDbContext is here

var builder = WebApplication.CreateBuilder(args);

// --- NPGSQL GLOBAL ENUM MAPPING (Crucial Step for PostgreSQL) ---
// This must be done BEFORE the DbContext is configured.
// It maps the C# enums to the custom ENUM types created in your PostgreSQL database.
NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderStatus>("order_status");
NpgsqlConnection.GlobalTypeMapper.MapEnum<PaymentStatus>("payment_status");


// Add services to the container.

// 1. CONFIGURE DATABASE CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// 2. OPEN API/SWAGGER (Metadata configuration)
builder.Services.AddOpenApi();

// 3. CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// --- APPLICATION ENDPOINTS (Your custom API routes will go here) ---

// Placeholder: Example of how a custom endpoint might look (e.g., product listing)
app.MapGet("/products", () =>
{
    // Logic to fetch products from the database using DbContext
    return Results.Ok("E-commerce API is running."); 
})
.WithName("GetProducts");

// The original boilerplate WeatherForecast code was removed.

app.Run();