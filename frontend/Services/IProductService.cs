using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for product service - handles all product-related API calls
/// </summary>
public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(long id);
    Task<ProductDto> CreateProductAsync(ProductDto product);
    Task UpdateProductAsync(long id, ProductDto product);
    Task DeleteProductAsync(long id);
}