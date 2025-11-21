using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for order service - handles order creation and retrieval
/// </summary>
public interface IOrderService
{
    Task<OrderDto?> GetOrderByIdAsync(long id);
    Task<List<OrderDto>> GetOrdersByClientAsync(long clientId);
    Task<OrderDto> CreateOrderAsync(OrderDto order);
    Task UpdateOrderAsync(long id, OrderDto order);
}
