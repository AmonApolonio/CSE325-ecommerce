using Microsoft.EntityFrameworkCore;
using backend.Data;
// using Npgsql.EntityFrameworkCore.PostgreSQL; removido pois EnableEnumMapping não será mais usado aqui

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

// 1. CONFIGURE DATABASE CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EcommerceDbContext>(options =>
    // A configuração do ENUM será feita via OnModelCreating no DbContext
    options.UseNpgsql(connectionString) 
);

// 2. OPEN API/SWAGGER (Metadata configuration)
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 

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
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// --- APPLICATION ENDPOINTS (Your custom API routes will go here) ---

// Implementation of the GET /products endpoint to fetch real data
app.MapGet("/products", async (EcommerceDbContext dbContext) =>
{
    // Use ToListAsync() to fetch all products from the database asynchronously
    var products = await dbContext.Products.ToListAsync();
    
    // Check if the list is empty
    if (!products.Any())
    {
        return Results.NotFound("No products found in the database.");
    }

    return Results.Ok(products); 
})
.WithName("GetProducts");

app.Run();