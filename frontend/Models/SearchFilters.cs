namespace frontend.Models;

/// <summary>
/// Search filter criteria
/// </summary>
public class SearchFilters
{
    public (decimal Min, decimal Max) PriceRange { get; set; } = (0, 1000);
    public List<string> SelectedCategories { get; set; } = new();
    public List<int> SelectedRatings { get; set; } = new();
}
