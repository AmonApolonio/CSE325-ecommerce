// Program.cs
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer; // NOVO
using Microsoft.IdentityModel.Tokens; // NOVO
using System.Text; // NOVO

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1. AUTENTICAÇÃO JWT BEARER (Substitui Cookie)
// Configure as chaves JWT no arquivo appsettings.json
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

        // ESTES VALORES DEVEM SER LIDOS DO appsettings.json ou variáveis de ambiente
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddScoped<ITokenService, TokenService>();


// ***************************************************************
// 2. CONFIGURE DATABASE CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// ***************************************************************

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 
builder.Services.AddControllers(); 

// 3. CORS Policy (Mantenha o AllowAnyOrigin APENAS para desenvolvimento)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsBuilder =>
    {
        // Em produção, substitua AllowAnyOrigin() por WithOrigins("https://seublazorfrontend.com")
        corsBuilder.AllowAnyOrigin() 
                   .AllowAnyMethod()
                   .AllowAnyHeader();
    });
});

var app = builder.Build();

// A ordem é crucial: Autenticação antes da Autorização
app.UseAuthentication();
app.UseAuthorization();

// =======================================================
// Call SeedData on initialization (Mantenha esta seção)
// =======================================================
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            // Seu código de Seeding...
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "❌ Ocorreu um erro durante o seeding da base de dados.");
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}
// =======================================================

// Redirecionamento obrigatório para HTTPS (Muito importante!)
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapControllers(); 

app.Run();