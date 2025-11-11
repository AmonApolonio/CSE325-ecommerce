namespace backend.Models
{
    // Define os possíveis status de um pedido.
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}