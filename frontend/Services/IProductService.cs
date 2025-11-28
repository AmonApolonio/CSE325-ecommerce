using frontend.Models;

namespace frontend.Services;

public interface IProductService
{
    Task<List<Product>> GetProductsAsync();
}