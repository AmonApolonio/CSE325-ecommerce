using Microsoft.EntityFrameworkCore;
using backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. CONFIGURE DATABASE CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseNpgsql(connectionString)
);




builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 


builder.Services.AddControllers(); 

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

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// --- APPLICATION ENDPOINTS (Your custom API routes will go here) ---


app.MapControllers(); 

// Implementation of the GET /products endpoint to fetch real data
// Nota: Este MapGet duplicará a funcionalidade do GetProducts no seu controller,
// mas vou mantê-lo aqui para consistência com o seu código original.
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
.WithName("GetProductsMinimalApi"); // Renomeado para evitar conflito de nome, se necessário.

app.Run();