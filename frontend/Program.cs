using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using frontend.Services;

using frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register mock services for demo data
builder.Services.AddScoped<MockProductService>();
builder.Services.AddScoped<MockCategoryService>();
builder.Services.AddScoped<MockCartService>();
builder.Services.AddScoped<MockReviewService>();


builder.Services.AddScoped<JwtHandler>();
builder.Services.AddScoped(sp =>
{
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    var handler = new JwtHandler(jsRuntime)
    {
        InnerHandler = new HttpClientHandler()
    };
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7198/") 
    };
});

builder.Services.AddAuthorizationCore();

// 1. Registra o Handler JWT (Interceptor que anexa o token)
builder.Services.AddScoped<JwtHandler>();

// 2. Registra o Provedor de Estado de Autenticação (Lê o token e define o usuário)
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthStateProvider>());

// 3. Registra o HttpClient Nomeado ("BackendApi") com o JwtHandler
builder.Services.AddHttpClient("BackendApi", client => 
    client.BaseAddress = new Uri("https://localhost:7198/"))
    .AddHttpMessageHandler<JwtHandler>();

// 4. Registra o AuthService, garantindo que ele injete o HttpClient Nomeado
builder.Services.AddScoped<AuthService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new AuthService(clientFactory.CreateClient("BackendApi"));
});

builder.Services.AddScoped<MockProductService>();
builder.Services.AddScoped<MockCategoryService>();
builder.Services.AddScoped<MockCartService>();
builder.Services.AddScoped<MockReviewService>();

await builder.Build().RunAsync();
