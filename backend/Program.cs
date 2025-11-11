using Microsoft.EntityFrameworkCore;
using Npgsql;
using backend.Models;
using backend.Data;
using Microsoft.AspNetCore.OpenApi; 

var builder = WebApplication.CreateBuilder(args);

// --- NPGSQL GLOBAL ENUM MAPPING (Crucial Step for PostgreSQL) ---
NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderStatus>("order_status");
NpgsqlConnection.GlobalTypeMapper.MapEnum<PaymentStatus>("payment_status");


// Add services to the container.

// 1. CONFIGURE DATABASE CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// 2. OPEN API/SWAGGER (Metadata configuration)
// 🔑 CORREÇÃO: Adicione AddEndpointsApiExplorer()
builder.Services.AddEndpointsApiExplorer(); 
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
    // 🔑 CORREÇÃO: Adicione UseSwagger() para gerar o arquivo JSON da documentação
    app.UseSwagger(); 
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// --- APPLICATION ENDPOINTS (Your custom API routes will go here) ---

app.MapGet("/products", () =>
{
    // Logic to fetch products from the database using DbContext
    return Results.Ok("E-commerce API is running."); 
})
.WithName("GetProducts");

app.Run();