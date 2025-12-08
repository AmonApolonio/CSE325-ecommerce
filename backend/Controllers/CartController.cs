using backend.Data.Entities;
using backend.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.BusinessLogic; 
using System; 
using System.Security.Claims;

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

    // Helper method to extract the UserId from the authentication context
    private long? GetAuthenticatedUserId()
    {
        var userIdClaim = HttpContext.User.FindFirstValue("UserId");
        
        if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out long parsedId))
        {
            return parsedId;
        }
        
        return null; 
    }
    
    // =================================================================
    // 0. GET api/carts/user
    // Gets or creates the authenticated user's cart.
    // Returns the cart ID for the authenticated user, or creates one if not found.
    // =================================================================
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<object>> GetUserCart()
    {
        long? userId = GetAuthenticatedUserId();

        if (!userId.HasValue)
        {
            return Unauthorized("Authentication is required to access user cart.");
        }

        // Try to find existing cart for user
        var existingCart = await _context.Carts
            .AsNoTracking()
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value);

        if (existingCart != null)
        {
            decimal total = existingCart.CalculateTotal();
            return Ok(new
            {
                cartId = existingCart.CartId,
                userId = existingCart.UserId,
                createdDate = existingCart.CreatedDate,
                updatedDate = existingCart.UpdatedDate,
                cartItems = (object)(existingCart?.CartItems?.Select(ci => new { ci.CartItemId, ci.ProductId, ci.Quantity }).ToList()) ?? new List<object>(),
                total = total
            });
        }

        // Create new cart if user doesn't have one
        var newCart = new Cart
        {
            UserId = userId.Value,
            CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        _context.Carts.Add(newCart);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            cartId = newCart.CartId,
            userId = newCart.UserId,
            createdDate = newCart.CreatedDate,
            updatedDate = newCart.UpdatedDate,
            cartItems = new List<object>(),
            total = 0
        });
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
        _context.ChangeTracker.Clear();
        
        var updatedCart = await _context.Carts
            .AsNoTracking()
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (updatedCart == null)
        {
            return NotFound("Cart not found.");
        }

        decimal total = updatedCart.CalculateTotal();
        
        return Ok(new { 
            cartId = updatedCart.CartId,
            userId = updatedCart.UserId,
            createdDate = updatedCart.CreatedDate,
            updatedDate = updatedCart.UpdatedDate,
            cartItems = (object)(updatedCart?.CartItems?.Select(ci => new { ci.CartItemId, ci.ProductId, ci.Quantity }).ToList()) ?? new List<object>(),
            total = total
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
        if (dto.Quantity <= 0)
        {
            return BadRequest("The quantity to be added must be greater than zero.");
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (cart == null)
        {
            return NotFound($"Cart with ID {cartId} not found.");
        }

        var productInfo = await _context.Products
            .Select(p => new { p.ProductId, p.Inventory })
            .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

        if (productInfo == null)
        {
            return NotFound($"Product with ID {dto.ProductId} not found.");
        }

        var existingItem = cart.CartItems
            .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            
            if (existingItem.Quantity > productInfo.Inventory)
            {
                existingItem.Quantity -= dto.Quantity;
                return BadRequest($"Insufficient stock. Maximum available: {productInfo.Inventory}.");
            }

            _context.CartItems.Update(existingItem);
        }
        else
        {
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
            
            _context.CartItems.Add(newItem);
        }
        
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();
        var updatedCart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (updatedCart == null)
        {
            return StatusCode(500, "Cart not found after saving");
        }

        decimal total = updatedCart?.CalculateTotal() ?? 0;

        return Ok(new { 
            message = "Item added to cart successfully", 
            cartId = cartId,
            cartItems = (object)(updatedCart?.CartItems?.Select(ci => new { ci.CartItemId, ci.ProductId, ci.Quantity }).ToList()) ?? new List<object>(),
            total = total
        });
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
        long? optionalUserId = GetAuthenticatedUserId();

        if (!optionalUserId.HasValue)
        {
            return Unauthorized("Authentication is required to perform cart merging.");
        }
        
        long userId = optionalUserId.Value;

        var anonymousCart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CartId == anonymousCartId && c.UserId == null);
        
        if (anonymousCart == null)
        {
            var cartExists = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == anonymousCartId);
            
            if (cartExists != null)
            {
                if (cartExists.UserId == userId)
                {
                    return Ok(cartExists);
                }
                
                return BadRequest($"Cart with ID {anonymousCartId} belongs to a different user.");
            }
            
            return NotFound($"Cart with ID {anonymousCartId} not found.");
        }

        var authenticatedCart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (authenticatedCart == null)
        {
            anonymousCart.UserId = userId;
            _context.Carts.Update(anonymousCart);
            await _context.SaveChangesAsync();
            return Ok(anonymousCart);
        }
        else
        {
            foreach (var anonItem in anonymousCart.CartItems.ToList()) 
            {
                var existingAuthItem = authenticatedCart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == anonItem.ProductId);

                if (existingAuthItem != null)
                {
                    existingAuthItem.Quantity += anonItem.Quantity;
                    _context.CartItems.Update(existingAuthItem);
                }
                else
                {
                    authenticatedCart.CartItems.Add(new CartItem
                    {
                        CartId = authenticatedCart.CartId,
                        ProductId = anonItem.ProductId,
                        Quantity = anonItem.Quantity
                    });
                }
                
                _context.CartItems.Remove(anonItem);
            }

            _context.Carts.Remove(anonymousCart);
            _context.Carts.Update(authenticatedCart);

            await _context.SaveChangesAsync();
            
            return Ok(authenticatedCart);
        }
    }
}