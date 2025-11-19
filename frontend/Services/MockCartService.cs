using frontend.Models;

namespace frontend.Services;

/// <summary>
/// Provides mock cart management
/// </summary>
public class MockCartService
{
    private List<CartItem> _cartItems = new()
    {
        new CartItem
        {
            Id = "1",
            Product = new ShoppingItem
            {
                Id = "3",
                ImageAlt = "Wooden Jewelry Box",
                ImageUrl = "/images/item-mock/item3.webp",
                ProductName = "Wooden Jewelry Box",
                SellerName = "WoodCrafter",
                Price = 78.00m,
                Rating = 4.7m,
                IsAddedToCart = true
            },
            Quantity = 1,
            AddedDate = DateTime.Now.AddDays(-2)
        }
    };

    /// <summary>
    /// Gets all items in the cart
    /// </summary>
    public Task<List<CartItem>> GetCartItemsAsync()
    {
        return Task.FromResult(new List<CartItem>(_cartItems));
    }

    /// <summary>
    /// Adds an item to the cart
    /// </summary>
    public Task<bool> AddToCartAsync(ShoppingItem product)
    {
        var existingItem = _cartItems.FirstOrDefault(i => i.Product.Id == product.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            _cartItems.Add(new CartItem
            {
                Id = Guid.NewGuid().ToString(),
                Product = product,
                Quantity = 1,
                AddedDate = DateTime.Now
            });
        }

        return Task.FromResult(true);
    }

    /// <summary>
    /// Removes an item from the cart
    /// </summary>
    public Task<bool> RemoveFromCartAsync(string productId)
    {
        var item = _cartItems.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            _cartItems.Remove(item);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// Updates quantity of a cart item
    /// </summary>
    public Task<bool> UpdateQuantityAsync(string productId, int quantity)
    {
        var item = _cartItems.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                _cartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets the cart total
    /// </summary>
    public Task<decimal> GetCartTotalAsync()
    {
        var total = _cartItems.Sum(i => i.Product.Price * i.Quantity);
        return Task.FromResult(total);
    }

    /// <summary>
    /// Gets the cart item count
    /// </summary>
    public Task<int> GetCartCountAsync()
    {
        return Task.FromResult(_cartItems.Sum(i => i.Quantity));
    }

    /// <summary>
    /// Clears the cart
    /// </summary>
    public Task<bool> ClearCartAsync()
    {
        _cartItems.Clear();
        return Task.FromResult(true);
    }
}
