using frontend.Models;

namespace frontend.Services;

/// <summary>
/// Provides mock data for shopping items and products
/// </summary>
public class MockProductService
{
    private readonly List<ShoppingItem> _mockItems = new()
    {
        new ShoppingItem
        {
            Id = "1",
            ImageAlt = "Chef's Knife",
            ImageUrl = "/images/item-mock/item1.webp",
            ProductName = "Chef's Knife",
            SellerName = "KnifeArtisan",
            Price = 45.00m,
            Rating = 4.8m,
            Category = "Chef Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "2",
            ImageAlt = "Santoku Knife",
            ImageUrl = "/images/item-mock/item2.webp",
            ProductName = "Santoku Knife",
            SellerName = "KnifeMaster",
            Price = 32.00m,
            Rating = 4.9m,
            Category = "Chef Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "3",
            ImageAlt = "Bread Knife",
            ImageUrl = "/images/item-mock/item3.webp",
            ProductName = "Bread Knife",
            SellerName = "KnifeCrafter",
            Price = 78.00m,
            Rating = 4.7m,
            Category = "Chef Knives",
            IsAddedToCart = true
        },
        new ShoppingItem
        {
            Id = "4",
            ImageAlt = "Paring Knife",
            ImageUrl = "/images/item-mock/item4.webp",
            ProductName = "Paring Knife",
            SellerName = "KnifeArtist",
            Price = 95.00m,
            Rating = 5.0m,
            Category = "Chef Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "5",
            ImageAlt = "Utility Knife",
            ImageUrl = "/images/item-mock/item5.webp",
            ProductName = "Utility Knife",
            SellerName = "KnifeArtisan",
            Price = 50.00m,
            Rating = 4.6m,
            Category = "Chef Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "6",
            ImageAlt = "Cleaver",
            ImageUrl = "/images/item-mock/item11.webp",
            ProductName = "Cleaver",
            SellerName = "KnifeWeaver",
            Price = 40.00m,
            Rating = 4.4m,
            Category = "Specialty Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "7",
            ImageAlt = "Boning Knife",
            ImageUrl = "/images/item-mock/item12.webp",
            ProductName = "Boning Knife",
            SellerName = "KnifeArtisan",
            Price = 60.00m,
            Rating = 4.9m,
            Category = "Specialty Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "8",
            ImageAlt = "Fillet Knife",
            ImageUrl = "/images/item-mock/item8.webp",
            ProductName = "Fillet Knife",
            SellerName = "KnifeArtist",
            Price = 25.00m,
            Rating = 4.7m,
            Category = "Specialty Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "9",
            ImageAlt = "Carving Knife",
            ImageUrl = "/images/item-mock/item9.webp",
            ProductName = "Carving Knife",
            SellerName = "KnifeCraftsman",
            Price = 55.00m,
            Rating = 4.8m,
            Category = "Specialty Knives",
            IsAddedToCart = false
        },
        new ShoppingItem
        {
            Id = "10",
            ImageAlt = "Steak Knife Set",
            ImageUrl = "/images/item-mock/item10.webp",
            ProductName = "Steak Knife Set",
            SellerName = "KnifeStudio",
            Price = 35.00m,
            Rating = 4.5m,
            Category = "Knife Sets",
            IsAddedToCart = false
        }
    };

    /// <summary>
    /// Gets all available categories
    /// </summary>
    public Task<List<string>> GetCategoriesAsync()
    {
        var categories = _mockItems.Select(i => i.Category).Distinct().OrderBy(c => c).ToList();
        return Task.FromResult(categories);
    }

    /// <summary>
    /// Gets all mock shopping items
    /// </summary>
    public Task<List<ShoppingItem>> GetShoppingItemsAsync()
    {
        return Task.FromResult(_mockItems);
    }

    /// <summary>
    /// Gets shopping items by category
    /// </summary>
    public Task<List<ShoppingItem>> GetItemsByCategoryAsync(string category)
    {
        // For mock data, return all items (in real app, filter by category)
        return Task.FromResult(_mockItems);
    }

    /// <summary>
    /// Gets a specific product by ID with full details
    /// </summary>
    public Task<ProductDetails?> GetProductDetailsAsync(string productId)
    {
        var item = _mockItems.FirstOrDefault(i => i.Id == productId);
        
        if (item == null)
            return Task.FromResult<ProductDetails?>(null);

        var details = new ProductDetails
        {
            Id = item.Id,
            Name = item.ProductName,
            Price = item.Price,
            Description = "Premium quality kitchen knife, expertly crafted for precision and durability. Sharp blade with comfortable handle. Perfect for professional and home use.",
            Material = "High-carbon stainless steel",
            Capacity = "N/A",
            Dimensions = "8\" blade",
            Care = "Hand wash only",
            Seller = new SellerInfo
            {
                Name = item.SellerName,
                MemberSince = "2019",
                Rating = item.Rating,
                ReviewCount = 156,
                ProfileImageUrl = "/images/user_placeholder.png"
            },
            Images = new List<string>
            {
                item.ImageUrl,
                "/images/item-mock/item1.webp",
                "/images/item-mock/item2.webp",
                "/images/item-mock/item3.webp",
                "/images/item-mock/item4.webp",
                "/images/item-mock/item5.webp",
            },
            Rating = item.Rating,
            Reviews = 24,
            IsAddedToCart = item.IsAddedToCart
        };

        return Task.FromResult<ProductDetails?>(details);
    }

    /// <summary>
    /// Searches products by query
    /// </summary>
    public Task<List<ShoppingItem>> SearchProductsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult(_mockItems);

        var lowerQuery = query.ToLower();
        var results = _mockItems
            .Where(i => i.ProductName.ToLower().Contains(lowerQuery) || 
                       i.SellerName.ToLower().Contains(lowerQuery))
            .ToList();

        return Task.FromResult(results);
    }

    /// <summary>
    /// Filters products based on criteria
    /// </summary>
    public Task<List<ShoppingItem>> FilterProductsAsync(List<ShoppingItem> items, SearchFilters filters)
    {
        var results = items
            .Where(i => i.Price >= filters.PriceRange.Min && i.Price <= filters.PriceRange.Max)
            .Where(i => !filters.SelectedCategories.Any() || filters.SelectedCategories.Contains(i.Category))
            .Where(i => !filters.SelectedRatings.Any() || filters.SelectedRatings.Contains((int)i.Rating))
            .ToList();

        return Task.FromResult(results);
    }
}
