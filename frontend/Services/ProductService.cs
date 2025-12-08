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

    /// <summary>
    /// Retrieves all products from the API.
    /// </summary>
    /// <returns>List of all products.</returns>
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

    /// <summary>
    /// Retrieves products with optional filtering.
    /// </summary>
    /// <param name="name">Optional product name filter.</param>
    /// <param name="sellerId">Optional seller ID filter.</param>
    /// <param name="categoryId">Optional category ID filter.</param>
    /// <returns>List of filtered products.</returns>
    public async Task<List<ProductDto>> GetProductsAsync(string? name = null, long? sellerId = null, long? categoryId = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name))
                queryParams.Add($"name={Uri.EscapeDataString(name)}");
            if (sellerId.HasValue)
                queryParams.Add($"sellerId={sellerId.Value}");
            if (categoryId.HasValue)
                queryParams.Add($"categoryId={categoryId.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>($"/api/Products{queryString}");
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
            var productToCreate = new
            {
                name = product.Name,
                description = product.Description,
                price = product.Price,
                inventory = product.Inventory,
                categoryId = product.CategoryId,
                sellerId = product.SellerId,
                cartItems = new List<CartItemDto>(),
                ordersProducts = new List<OrdersProductDto>(),
                seller = (SellerDto?)null
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Products", productToCreate);
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
