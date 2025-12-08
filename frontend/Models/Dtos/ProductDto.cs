namespace frontend.Models.Dtos;

/// <summary>
/// Data Transfer Object for Product API responses
/// Maps API response to frontend model with type conversions
/// </summary>
public class ProductDto
{
    public long ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    /// <summary>
    /// Price in cents from API - will be converted to decimal
    /// </summary>
    public int Price { get; set; }
    public double Inventory { get; set; }
    public string? Url { get; set; }
    public long CategoryId { get; set; }
    public long SellerId { get; set; }
    public SellerDto? Seller { get; set; }
    public List<CartItemDto>? CartItems { get; set; }
    public List<OrdersProductDto>? OrdersProducts { get; set; }

    /// <summary>
    /// Convert price from cents to decimal currency format
    /// </summary>
    public decimal GetPriceAsDecimal() => Price / 100m;
}

/// <summary>
/// DTO for Category API responses
/// </summary>
public class CategoryDto
{
    public long CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for Cart API responses
/// </summary>
public class CartDto
{
    public long CartId { get; set; }
    public long? UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<CartItemDto>? CartItems { get; set; }
    public ClientDto? User { get; set; }
}

/// <summary>
/// DTO for CartItem API responses
/// </summary>
public class CartItemDto
{
    public long CartItemId { get; set; }
    public long CartId { get; set; }
    public long ProductId { get; set; }
    public double Quantity { get; set; }
    public CartDto? Cart { get; set; }
    public ProductDto? Product { get; set; }
}

/// <summary>
/// DTO for Client/User API responses
/// </summary>
public class ClientDto
{
    public long UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public List<CartDto>? Carts { get; set; }
}

/// <summary>
/// DTO for Seller API responses
/// </summary>
public class SellerDto
{
    public long SellerId { get; set; }
    public string? Name { get; set; }
    public string? PhotoUrl { get; set; }
    public string? AboutMe { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public double CommisionRate { get; set; }
    public string? Email { get; set; }
    public List<ProductDto>? Products { get; set; }
}

/// <summary>
/// DTO for Order API responses
/// </summary>
public class OrderDto
{
    public long OrderId { get; set; }
    public long ClientId { get; set; }
    public double SubTotalCents { get; set; }
    public double TaxCents { get; set; }
    public double? FreightCents { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CurrencyCode { get; set; }
    public CurrencyDto? CurrencyCodeNavigation { get; set; }
    public List<OrdersProductDto>? OrdersProducts { get; set; }
    public List<PaymentDto>? Payments { get; set; }

    /// <summary>
    /// Calculate total including tax and freight
    /// </summary>
    public decimal GetTotalAsDecimal()
    {
        var total = (decimal)(SubTotalCents + TaxCents + (FreightCents ?? 0)) / 100m;
        return total;
    }

    /// <summary>
    /// Get subtotal as decimal
    /// </summary>
    public decimal GetSubTotalAsDecimal() => (decimal)SubTotalCents / 100m;
}

/// <summary>
/// DTO for OrdersProduct (junction table) API responses
/// </summary>
public class OrdersProductDto
{
    public long OrdersOrderId { get; set; }
    public long ProductsProductId { get; set; }
    public double Quantity { get; set; }
    public OrderDto? OrdersOrder { get; set; }
    public ProductDto? Product { get; set; }
}

/// <summary>
/// DTO for Payment API responses
/// </summary>
public class PaymentDto
{
    public long PaymentId { get; set; }
    public long OrderId { get; set; }
    public DateTime PaymentDate { get; set; }
    public double Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public OrderDto? Order { get; set; }

    /// <summary>
    /// Convert amount to decimal
    /// </summary>
    public decimal GetAmountAsDecimal() => (decimal)Amount / 100m;
}

/// <summary>
/// DTO for Currency API responses
/// </summary>
public class CurrencyDto
{
    public string? CurrencyCode { get; set; }
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public double? ExchangeRateToBrl { get; set; }
    public List<OrderDto>? Orders { get; set; }
}

/// <summary>
/// DTO for Login request
/// </summary>
public class LoginRequestDto
{
    public string? Usuario { get; set; }
    public string? Senha { get; set; }
}

/// <summary>
/// DTO for Login response - contains JWT token
/// </summary>
public class LoginResponseDto
{
    public string? Token { get; set; }
    public ClientDto? User { get; set; }
    public string? Message { get; set; }
}
