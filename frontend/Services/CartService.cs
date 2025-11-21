using frontend.Models.Dtos;
using System.Net.Http.Json;

namespace frontend.Services;

/// <summary>
/// Cart service implementation - handles shopping cart operations with backend persistence
/// </summary>
public class CartService : ICartService
{
    private readonly HttpClient _httpClient;

    public CartService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<CartDto?> GetCartAsync(long userId)
    {
        try
        {
            // GET cart for user - assumes endpoint exists or we derive from carts list
            return await _httpClient.GetFromJsonAsync<CartDto>($"/api/Carts/{userId}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching cart for user {userId}: {ex.Message}");
            throw;
        }
    }

    public async Task<CartDto> CreateCartAsync(long userId)
    {
        try
        {
            var cartRequest = new { userId };
            var response = await _httpClient.PostAsJsonAsync("/api/Carts", cartRequest);
            response.EnsureSuccessStatusCode();
            var cart = await response.Content.ReadFromJsonAsync<CartDto>();
            return cart ?? throw new InvalidOperationException("Failed to create cart");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error creating cart: {ex.Message}");
            throw;
        }
    }

    public async Task<CartItemDto> AddToCartAsync(long cartId, long productId, double quantity)
    {
        try
        {
            var cartItem = new { cartId, productId, quantity };
            var response = await _httpClient.PostAsJsonAsync("/api/CartItems", cartItem);
            response.EnsureSuccessStatusCode();
            var item = await response.Content.ReadFromJsonAsync<CartItemDto>();
            return item ?? throw new InvalidOperationException("Failed to add item to cart");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error adding item to cart: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateCartItemAsync(long cartId, long cartItemId, double quantity)
    {
        try
        {
            var updateRequest = new { quantity };
            var response = await _httpClient.PutAsJsonAsync($"/api/CartItems/{cartItemId}", updateRequest);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error updating cart item {cartItemId}: {ex.Message}");
            throw;
        }
    }

    public async Task RemoveFromCartAsync(long cartId, long cartItemId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/CartItems/{cartItemId}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error removing item from cart: {ex.Message}");
            throw;
        }
    }

    public async Task ClearCartAsync(long cartId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/Carts/{cartId}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error clearing cart: {ex.Message}");
            throw;
        }
    }
}
