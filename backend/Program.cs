// Program.cs
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. CONFIGURE DATABASE CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// ***************************************************************


builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 


builder.Services.AddControllers(); 

// 3. CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
    });
});

var app = builder.Build();


// =======================================================
//Call SeedData on initialization***
// =======================================================
// Seeding should only run in a development/test environment.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            // O Initialize contém o EnsureDeleted() e EnsureCreated()
            //SeedData.Initialize(services); 
            //Console.WriteLine("✅ Seed Data executado com sucesso: base de dados reconstruída e preenchida.");
        }
        catch (Exception ex)
        {
            // Loga o erro em caso de falha no seeding
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "❌ Ocorreu um erro durante o seeding da base de dados.");
        }
    }

    // Configuração do Swagger para desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}
// =======================================================


app.UseHttpsRedirection();

app.UseCors("AllowAll");

// --- APPLICATION ENDPOINTS (Your custom API routes will go here) ---

app.MapControllers(); 

app.Run();