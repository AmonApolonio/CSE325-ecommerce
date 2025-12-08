
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Redireciona para HTTPS
app.UseHttpsRedirection();

// Ativa CORS
app.UseCors("AllowBlazor");

// Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
