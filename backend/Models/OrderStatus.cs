namespace backend.Models
{
    // Define os possíveis status de um pedido.
    public enum OrderStatus
    {
        PendingPayment,
        PaymentConfirmed,
        PaymentFailed,
        Cancelled,
        Processing,
        Shipped,
        OutForDelivery,
        Delivered,
        Completed,
        Returned
    }
}