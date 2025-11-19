using frontend.Models;

namespace frontend.Services;

/// <summary>
/// Provides mock review data for products
/// </summary>
public class MockReviewService
{
    private readonly ProductReviewsResponse _mockReviews = new()
    {
        RatingBreakdown = new RatingBreakdown
        {
            TotalReviews = 24,
            AverageRating = 4.3m,
            RatingsCount = new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 1 },
                { 4, 4 },
                { 5, 19 }
            }
        },
        Reviews = new List<ProductReview>
        {
            new ProductReview
            {
                Id = "1",
                ReviewerName = "Jane Doe",
                ReviewerProfileImageUrl = "/images/user_placeholder.png",
                Rating = 5,
                ReviewDate = "2 days ago",
                ReviewText = "Absolutely love this item! The craftsmanship is excellent and it feels perfect in my hands. Highly recommend!"
            },
            new ProductReview
            {
                Id = "2",
                ReviewerName = "Mike Smith",
                ReviewerProfileImageUrl = "/images/user_placeholder.png",
                Rating = 5,
                ReviewDate = "1 week ago",
                ReviewText = "Great quality item. Arrived quickly and was packaged very well. Perfect addition to my collection."
            },
            new ProductReview
            {
                Id = "3",
                ReviewerName = "Sarah Johnson",
                ReviewerProfileImageUrl = "/images/user_placeholder.png",
                Rating = 4,
                ReviewDate = "2 weeks ago",
                ReviewText = "Very nice piece. The only minor issue was a small blemish, but overall excellent quality."
            },
            new ProductReview
            {
                Id = "4",
                ReviewerName = "Tom Anderson",
                ReviewerProfileImageUrl = "/images/user_placeholder.png",
                Rating = 5,
                ReviewDate = "1 month ago",
                ReviewText = "Exactly as described. Beautiful handcrafted work. Will definitely order again!"
            }
        }
    };

    /// <summary>
    /// Gets reviews for a specific product
    /// </summary>
    public Task<ProductReviewsResponse> GetProductReviewsAsync(string productId)
    {
        // In a real app, filter by productId
        return Task.FromResult(_mockReviews);
    }
}
