// Program.cs
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Nome do cookie que ficará no navegador
        options.Cookie.Name = "MeuApp.Auth"; 
        
        // SEGURANÇA MÁXIMA: Impede que o JS leia o cookie (Proteção XSS)
        options.Cookie.HttpOnly = true; 
        
        // Só envia em conexões HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
        
        // Protege contra ataques de falsificação de requisição (CSRF)
        // Use 'Strict' se Frontend e Backend estiverem no mesmo domínio exato
        // Use 'Lax' se houver navegação entre subdomínios
        options.Cookie.SameSite = SameSiteMode.Strict; 
        
        // Tempo de vida da sessão
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true; // Renova o tempo se o usuário estiver ativo

        // O que acontece se tentar acessar sem logar? (Retorna 401 em vez de redirecionar para página HTML)
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

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

app.UseAuthentication();
app.UseAuthorization();

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