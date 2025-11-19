using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register mock services for demo data
builder.Services.AddScoped<MockProductService>();
builder.Services.AddScoped<MockCategoryService>();
builder.Services.AddScoped<MockCartService>();
builder.Services.AddScoped<MockReviewService>();

await builder.Build().RunAsync();
