namespace frontend.Models;

/// <summary>
/// Represents customer reviews for a product
/// </summary>
public class ProductReview
{
    public string Id { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;
    public string? ReviewerProfileImageUrl { get; set; }
    public int Rating { get; set; }
    public string ReviewDate { get; set; } = string.Empty;
    public string ReviewText { get; set; } = string.Empty;
}

/// <summary>
/// Rating breakdown statistics
/// </summary>
public class RatingBreakdown
{
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public Dictionary<int, int> RatingsCount { get; set; } = new();
}

/// <summary>
/// Complete review response with statistics
/// </summary>
public class ProductReviewsResponse
{
    public RatingBreakdown RatingBreakdown { get; set; } = new();
    public List<ProductReview> Reviews { get; set; } = new();
}
