using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with API base address
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5028";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Register real API services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<IClientService, ClientService>();

// Keep mock services for backward compatibility with existing components
// These can be removed once all components are migrated to real services
builder.Services.AddScoped<MockProductService>();
builder.Services.AddScoped<MockCategoryService>();
builder.Services.AddScoped<MockCartService>();
builder.Services.AddScoped<MockReviewService>();

await builder.Build().RunAsync();
