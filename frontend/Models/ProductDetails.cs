namespace frontend.Models;

/// <summary>
/// Represents detailed product information
/// </summary>
public class ProductDetails
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public string Dimensions { get; set; } = string.Empty;
    public string Care { get; set; } = string.Empty;
    public SellerInfo Seller { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public decimal Rating { get; set; }
    public int Reviews { get; set; }
    public bool IsAddedToCart { get; set; }
}

/// <summary>
/// Seller information for product details
/// </summary>
public class SellerInfo
{
    public string Name { get; set; } = string.Empty;
    public string MemberSince { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? ProfileImageUrl { get; set; }
}
