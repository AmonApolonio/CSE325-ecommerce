using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for cart service - handles cart operations with backend persistence
/// </summary>
public interface ICartService
{
    Task<CartDto?> GetCartAsync(long userId);
    Task<CartDto> CreateCartAsync(long userId);
    Task<CartItemDto> AddToCartAsync(long userId, long productId, double quantity);
    Task UpdateCartItemAsync(long cartId, long productId, double quantity);
    Task RemoveFromCartAsync(long cartId, long productId);
    Task ClearCartAsync(long cartId);
}
