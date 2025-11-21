using frontend.Models.Dtos;
using System.Net.Http.Json;

namespace frontend.Services;

/// <summary>
/// Order service implementation - handles order creation and retrieval
/// </summary>
public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<OrderDto?> GetOrderByIdAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<OrderDto>($"/api/Orders/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching order {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<OrderDto>> GetOrdersByClientAsync(long clientId)
    {
        try
        {
            var orders = await _httpClient.GetFromJsonAsync<List<OrderDto>>($"/api/Orders/client/{clientId}");
            return orders ?? new List<OrderDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching orders for client {clientId}: {ex.Message}");
            throw;
        }
    }

    public async Task<OrderDto> CreateOrderAsync(OrderDto order)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Orders", order);
            response.EnsureSuccessStatusCode();
            var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
            return createdOrder ?? throw new InvalidOperationException("Failed to create order");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error creating order: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateOrderAsync(long id, OrderDto order)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Orders/{id}", order);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error updating order {id}: {ex.Message}");
            throw;
        }
    }
}
