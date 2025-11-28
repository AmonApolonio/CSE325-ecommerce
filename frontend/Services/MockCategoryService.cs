using frontend.Models;

namespace frontend.Services;

/// <summary>
/// Provides mock category data
/// </summary>
public class MockCategoryService
{
    private readonly List<string> _mockCategories = new()
    {
        "Jewelry",
        "Home Decor",
        "Clothing",
        "Art"
    };

    /// <summary>
    /// Gets all available categories
    /// </summary>
    public Task<List<string>> GetCategoriesAsync()
    {
        return Task.FromResult(new List<string>(_mockCategories));
    }
}
