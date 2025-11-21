using frontend.Models.Dtos;
using System.Net.Http.Json;

namespace frontend.Services;

/// <summary>
/// Real product service implementation - communicates with backend API
/// </summary>
public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        try
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("/api/Products");
            return products ?? new List<ProductDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching products: {ex.Message}");
            throw;
        }
    }

    public async Task<ProductDto?> GetProductByIdAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>($"/api/Products/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching product {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto product)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Products", product);
            response.EnsureSuccessStatusCode();
            var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
            return createdProduct ?? throw new InvalidOperationException("Failed to create product");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error creating product: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateProductAsync(long id, ProductDto product)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Products/{id}", product);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error updating product {id}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteProductAsync(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/Products/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error deleting product {id}: {ex.Message}");
            throw;
        }
    }
}
