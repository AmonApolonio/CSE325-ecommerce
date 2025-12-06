// Exemplo de SellerDetailsDto no seu projeto de Backend (Models/DTOs)
public class SellerDetailsDto
{
    public long SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string? AboutMe { get; set; }
    public string? PhoneNumber { get; set; }
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}