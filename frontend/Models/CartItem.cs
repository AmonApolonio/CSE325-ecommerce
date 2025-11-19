namespace frontend.Models;

/// <summary>
/// Represents an item in the shopping cart
/// </summary>
public class CartItem
{
    public string Id { get; set; } = string.Empty;
    public ShoppingItem Product { get; set; } = new();
    public int Quantity { get; set; } = 1;
    public DateTime AddedDate { get; set; }
}
