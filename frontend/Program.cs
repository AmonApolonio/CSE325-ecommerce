using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;

using frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

<<<<<<< HEAD
<<<<<<< HEAD
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:5028") 
});
=======
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
<<<<<<< HEAD
builder.Services.AddScoped<IProductService, ProductService>();
>>>>>>> a52d2c9 (feat: initial wireframe for the homescreen)
=======

=======
>>>>>>> aa7ef36 (Ajuste de login)
// Register mock services for demo data
builder.Services.AddScoped<MockProductService>();
builder.Services.AddScoped<MockCategoryService>();
builder.Services.AddScoped<MockCartService>();
builder.Services.AddScoped<MockReviewService>();
>>>>>>> 642b06d (feat: initial frontend screens using mockup data)

// --- CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ---

builder.Services.AddAuthorizationCore();

// 1. Registra o Handler JWT (Interceptor que anexa o token)
builder.Services.AddScoped<JwtHandler>();

// 2. Registra o Provedor de Estado de Autenticação (Lê o token e define o usuário)
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthStateProvider>());

// 3. Registra o HttpClient Nomeado ("BackendApi") com o JwtHandler
builder.Services.AddHttpClient("BackendApi", client => 
    client.BaseAddress = new Uri("https://localhost:7128/"))
    .AddHttpMessageHandler<JwtHandler>();

// 4. Registra o AuthService, garantindo que ele injete o HttpClient Nomeado
builder.Services.AddScoped<AuthService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new AuthService(clientFactory.CreateClient("BackendApi"));
});


await builder.Build().RunAsync();