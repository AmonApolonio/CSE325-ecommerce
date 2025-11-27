using backend.Data.Entities;
using backend.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.BusinessLogic; 
using System; 
using System.Security.Claims; // Required for ClaimTypes

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartsController(AppDbContext context)
    {
        _context = context;
    }

    // Helper method to extract the UserId from the authentication context (Real Standard)
    private long? GetAuthenticatedUserId()
    {
        // Tries to find the identification claim (usually NameIdentifier, which holds the user ID)
        var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out long parsedId))
        {
            return parsedId;
        }
        
        // Returns null if there is no authentication or if the ID is invalid
        return null; 
    }
    
    // =================================================================
    // 1. POST api/carts
    // Creates a new cart. Unified for anonymous or logged-in users.
    // =================================================================
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Cart>> CreateCart()
    {
        // REAL STANDARD: The ID is read from the authentication token, not from a parameter.
        long? userId = GetAuthenticatedUserId();

        if (userId.HasValue)
        {
            // Logged-in Flow: Tries to find an existing cart
            var existingCart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (existingCart != null)
            {
                // If the cart already exists, return it (prevents multiple carts per user)
                return Ok(existingCart);
            }
        }
        
        // Anonymous Flow (userId == null) or Logged-in without Cart
        var newCart = new Cart
        {
            UserId = userId, // Will be null or the logged-in user ID
            CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        _context.Carts.Add(newCart);
        await _context.SaveChangesAsync();

        // Returns the newly created cart with status 201 Created
        return CreatedAtAction(nameof(GetCart), new { cartId = newCart.CartId }, newCart);
    }
    
    // Helper to get a cart (now returns Cart and Total)
    [HttpGet("{cartId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetCart(long cartId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (cart == null)
        {
            return NotFound("Cart not found.");
        }

        // Calculates the cart total using the extension method
        decimal total = cart.CalculateTotal();
        
        // Returns the cart and total in an anonymous object (ideally it would be a DTO)
        return Ok(new { 
            Cart = cart, 
            Total = total 
        });
    }

    // =================================================================
    // 2. POST api/carts/{cartId}/items
    // Adds or increments the quantity of an item in the cart.
    // =================================================================
    [HttpPost("{cartId}/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddItemToCart(long cartId, [FromBody] AddItemToCartDto dto)
    {
        // 1. Initial validations
        if (dto.Quantity <= 0)
        {
            return BadRequest("The quantity to be added must be greater than zero.");
        }

        // 2. Load the cart
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (cart == null)
        {
            return NotFound($"Cart with ID {cartId} not found.");
        }

        // 3. Check if the product exists and get its inventory
        var productInfo = await _context.Products
            .Select(p => new { p.ProductId, p.Inventory })
            .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

        if (productInfo == null)
        {
            return NotFound($"Product with ID {dto.ProductId} not found.");
        }

        // 4. Find or create the CartItem
        var existingItem = cart.CartItems
            .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            // Existing item: Increment the quantity
            existingItem.Quantity += dto.Quantity;
            
            // 5. Inventory Validation (Example)
            if (existingItem.Quantity > productInfo.Inventory)
            {
                existingItem.Quantity -= dto.Quantity; // Revert the change
                return BadRequest($"Insufficient stock. Maximum available: {productInfo.Inventory}.");
            }

            _context.CartItems.Update(existingItem);
        }
        else
        {
            // New item: Create a new CartItem
            
            // 5. Inventory Validation for new item
            if (dto.Quantity > productInfo.Inventory)
            {
                return BadRequest($"Insufficient stock. Maximum available: {productInfo.Inventory}.");
            }
            
            var newItem = new CartItem
            {
                CartId = cartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            
            cart.CartItems.Add(newItem); 
        }
        
        // 6. Save changes
        await _context.SaveChangesAsync();

        return Ok(cart);
    }
    
    // =================================================================
    // 3. PUT api/carts/{cartId}/items/{productId}
    // Explicitly sets the quantity of an item in the cart.
    // =================================================================
    [HttpPut("{cartId}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateItemQuantity(long cartId, long productId, [FromBody] UpdateItemQuantityDto dto)
    {
        // 1. DTO Validation
        if (dto.NewQuantity < 0)
        {
             return BadRequest("The new quantity cannot be negative.");
        }
        
        // 2. Try to find the item in the cart
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

        if (existingItem == null)
        {
            return NotFound("Item not found in the specified cart.");
        }

        // 3. If the new quantity is zero, remove the item
        if (dto.NewQuantity == 0)
        {
            _context.CartItems.Remove(existingItem);
        }
        else
        {
            // 4. Inventory Validation
            var productInfo = await _context.Products
                .Select(p => new {p.ProductId, p.Inventory })
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (productInfo == null)
            {
                return NotFound($"Product associated with ID {productId} not found.");
            }

            if (dto.NewQuantity > productInfo.Inventory)
            {
                return BadRequest($"Insufficient stock. Maximum available: {productInfo.Inventory}.");
            }

            // 5. Update the quantity
            existingItem.Quantity = dto.NewQuantity;
            _context.CartItems.Update(existingItem);
        }

        // 6. Save changes
        await _context.SaveChangesAsync();

        return Ok(new { message = "Item quantity updated successfully." });
    }

    // =================================================================
    // 4. DELETE api/carts/{cartId}/items/{productId}
    // Completely removes an item from the cart.
    // =================================================================
    [HttpDelete("{cartId}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveItemFromCart(long cartId, long productId)
    {
        // 1. Try to find the item in the cart
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

        if (existingItem == null)
        {
            return NotFound("Item not found in the cart.");
        }

        // 2. Remove the item
        _context.CartItems.Remove(existingItem);

        // 3. Save changes
        await _context.SaveChangesAsync();

        // Returns status 204 No Content
        return NoContent();
    }

    // =================================================================
    // 5. PUT api/carts/merge/{anonymousCartId}
    // Associates an anonymous cart with the user ID after login.
    // =================================================================
    [HttpPut("merge/{anonymousCartId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Cart>> MergeCarts(long anonymousCartId)
    {
        // 1. REAL ID RETRIEVAL: This method MUST be called by an authenticated user. 
        long? optionalUserId = GetAuthenticatedUserId();

        if (!optionalUserId.HasValue)
        {
            // If there is no userId, we cannot perform the merge, as we don't know who is logging in
            return Unauthorized("Authentication is required to perform cart merging.");
        }
        
        long userId = optionalUserId.Value;

        // 2. Try to load the anonymous cart (must have UserId == null)
        var anonymousCart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CartId == anonymousCartId && c.UserId == null);

        if (anonymousCart == null)
        {
            return NotFound($"Anonymous cart with ID {anonymousCartId} not found or already associated.");
        }

        // 3. Try to find an existing (authenticated) cart for this user
        var authenticatedCart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (authenticatedCart == null)
        {
            // SCENARIO A: User does not have a cart. Just associate the anonymous one.
            anonymousCart.UserId = userId;
            _context.Carts.Update(anonymousCart);
            await _context.SaveChangesAsync();
            return Ok(anonymousCart);
        }
        else
        {
            // SCENARIO B: User already has a cart. Merge the items.

            foreach (var anonItem in anonymousCart.CartItems.ToList()) 
            {
                var existingAuthItem = authenticatedCart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == anonItem.ProductId);

                if (existingAuthItem != null)
                {
                    // Duplicate item: Sum the quantities
                    existingAuthItem.Quantity += anonItem.Quantity;
                    _context.CartItems.Update(existingAuthItem);
                }
                else
                {
                    // New item: Add to the authenticated cart
                    authenticatedCart.CartItems.Add(new CartItem
                    {
                        CartId = authenticatedCart.CartId,
                        ProductId = anonItem.ProductId,
                        Quantity = anonItem.Quantity
                    });
                }
                
                // Remove the item from the anonymous cart
                _context.CartItems.Remove(anonItem);
            }

            // 4. Delete the anonymous cart and save all changes
            _context.Carts.Remove(anonymousCart);
            _context.Carts.Update(authenticatedCart);

            await _context.SaveChangesAsync();
            
            return Ok(authenticatedCart);
        }
    }
}