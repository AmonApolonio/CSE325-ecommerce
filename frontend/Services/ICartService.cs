using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for cart service - handles cart operations with backend persistence
/// </summary>
public interface ICartService
{
    Task<CartDto?> GetCartAsync(long userId);
    Task<CartDto> CreateCartAsync(long userId);
    Task<CartItemDto> AddToCartAsync(long cartId, long productId, double quantity);
    Task UpdateCartItemAsync(long cartId, long cartItemId, double quantity);
    Task RemoveFromCartAsync(long cartId, long cartItemId);
    Task ClearCartAsync(long cartId);
}
