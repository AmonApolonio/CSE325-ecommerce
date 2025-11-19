using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

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

// Register mock services for demo data
builder.Services.AddScoped<MockProductService>();
builder.Services.AddScoped<MockCategoryService>();
builder.Services.AddScoped<MockCartService>();
builder.Services.AddScoped<MockReviewService>();
>>>>>>> 642b06d (feat: initial frontend screens using mockup data)

await builder.Build().RunAsync();
