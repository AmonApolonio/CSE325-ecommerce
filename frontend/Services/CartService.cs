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

    private async Task<long> GetOrCreateCartIdAsync(long userId)
    {
        try
        {
            // Get client to find existing cart
            var client = await _httpClient.GetFromJsonAsync<ClientDto>($"/api/Clients/{userId}");
            if (client?.Carts != null && client.Carts.Any())
            {
                return client.Carts.First().CartId;
            }
            else
            {
                // Create new cart
                var cart = await CreateCartAsync(userId);
                return cart.CartId;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error getting or creating cart for user {userId}: {ex.Message}");
            throw;
        }
    }

    public async Task<CartDto?> GetCartAsync(long userId)
    {
        try
        {
            var cartId = await GetOrCreateCartIdAsync(userId);
            return await _httpClient.GetFromJsonAsync<CartDto>($"/api/Carts/{cartId}");
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

    public async Task<CartItemDto> AddToCartAsync(long userId, long productId, double quantity)
    {
        try
        {
            var cartId = await GetOrCreateCartIdAsync(userId);
            var cartItem = new { productId, quantity };
            var response = await _httpClient.PostAsJsonAsync($"/api/Carts/{cartId}/items", cartItem);
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

    public async Task UpdateCartItemAsync(long cartId, long productId, double quantity)
    {
        try
        {
            var updateRequest = new { newQuantity = (int)quantity };
            var response = await _httpClient.PutAsJsonAsync($"/api/Carts/{cartId}/items/{productId}", updateRequest);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error updating cart item {productId}: {ex.Message}");
            throw;
        }
    }

    public async Task RemoveFromCartAsync(long cartId, long productId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/Carts/{cartId}/items/{productId}");
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
            var cart = await _httpClient.GetFromJsonAsync<CartDto>($"/api/Carts/{cartId}");
            if (cart?.CartItems != null)
            {
                foreach (var item in cart.CartItems)
                {
                    await _httpClient.DeleteAsync($"/api/Carts/{cartId}/items/{item.ProductId}");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error clearing cart: {ex.Message}");
            throw;
        }
    }
}
