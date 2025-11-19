namespace frontend.Models;

/// <summary>
/// Represents a product item displayed in shopping grids
/// </summary>
public class ShoppingItem
{
    public string Id { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageAlt { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAddedToCart { get; set; }
}
