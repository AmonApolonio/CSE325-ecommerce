using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for category service - handles all category-related API calls
/// </summary>
public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(long id);
    Task<CategoryDto> CreateCategoryAsync(CategoryDto category);
    Task UpdateCategoryAsync(long id, CategoryDto category);
    Task DeleteCategoryAsync(long id);
}
