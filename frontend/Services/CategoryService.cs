using frontend.Models.Dtos;
using System.Net.Http.Json;

namespace frontend.Services;

/// <summary>
/// Category service implementation - communicates with backend API for category operations
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        try
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("/api/Categories");
            return categories ?? new List<CategoryDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching categories: {ex.Message}");
            throw;
        }
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CategoryDto>($"/api/Categories/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching category {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto category)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Categories", category);
            response.EnsureSuccessStatusCode();
            var createdCategory = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return createdCategory ?? throw new InvalidOperationException("Failed to create category");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error creating category: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateCategoryAsync(long id, CategoryDto category)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Categories/{id}", category);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error updating category {id}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteCategoryAsync(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/Categories/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error deleting category {id}: {ex.Message}");
            throw;
        }
    }
}
