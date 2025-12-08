using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for cart service - handles cart operations with backend persistence
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Gets or creates the authenticated user's cart from the server.
    /// This should be called for authenticated users instead of storing cart ID in localStorage.
    /// </summary>
    Task<CartDto> GetUserCartAsync();

    /// <summary>
    /// Creates a new anonymous cart. Must be called before logging in to preserve cart items.
    /// The returned cart ID should be stored and used to merge with user account after login.
    /// </summary>
    Task<long> CreateAnonymousCartAsync();

    /// <summary>
    /// Merges an anonymous cart with the authenticated user's account.
    /// After login, call this with the anonymous cart ID to link it to the user.
    /// </summary>
    Task<CartDto> MergeCartAsync(long anonymousCartId);

    /// <summary>
    /// Gets the current user's cart by their ID.
    /// </summary>
    Task<CartDto?> GetCartAsync(long userId);

    /// <summary>
    /// Adds an item to the cart. Requires a valid cartId.
    /// </summary>
    Task AddToCartAsync(long cartId, long productId, double quantity);

    /// <summary>
    /// Updates the quantity of an item in the cart.
    /// </summary>
    Task UpdateCartItemAsync(long cartId, long productId, double quantity);

    /// <summary>
    /// Removes an item from the cart.
    /// </summary>
    Task RemoveFromCartAsync(long cartId, long productId);

    /// <summary>
    /// Clears all items from the cart.
    /// </summary>
    Task ClearCartAsync(long cartId);
}
