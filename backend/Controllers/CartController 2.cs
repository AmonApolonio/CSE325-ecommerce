
using backend.Data.Entities;
using backend.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    private long? GetAuthenticatedUserId()
    {
        var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(userIdClaim, out long parsedId) ? parsedId : (long?)null;
    }

    // =================================================================
    // 1. POST api/carts
    // Cria um novo carrinho ou retorna o existente do usuário autenticado
    // =================================================================
    [HttpPost]
    public async Task<ActionResult<CartDto>> CreateCart()
    {
        long? userId = GetAuthenticatedUserId();

        if (userId.HasValue)
        {
            var existingCart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (existingCart != null)
                return Ok(MapToCartDto(existingCart));
        }

        var newCart = new Cart
        {
            UserId = userId,
            CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            UpdatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _context.Carts.Add(newCart);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCart), new { cartId = newCart.CartId }, MapToCartDto(newCart));
    }

    // =================================================================
    // 2. GET api/carts/{cartId}
    // Retorna um carrinho com seus itens
    // =================================================================
    [HttpGet("{cartId}")]
    public async Task<ActionResult<CartDto>> GetCart(long cartId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CartId == cartId);

        if (cart == null)
            return NotFound("Carrinho não encontrado.");

        return Ok(MapToCartDto(cart));
    }

    // =================================================================
    // 3. POST api/carts/{cartId}/items
    // Adiciona um item ao carrinho
    // =================================================================
    [HttpPost("{cartId}/items")]
    public async Task<ActionResult<CartItemDto>> AddItemToCart(long cartId, [FromBody] AddItemToCartDto dto)
    {
        if (dto.Quantity <= 0)
            return BadRequest("A quantidade deve ser maior que zero.");

        var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartId == cartId);
        if (cart == null)
            return NotFound($"Carrinho com ID {cartId} não encontrado.");

        var productInfo = await _context.Products
            .Select(p => new { p.ProductId, p.Inventory })
            .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

        if (productInfo == null)
            return NotFound($"Produto com ID {dto.ProductId} não encontrado.");

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            if (existingItem.Quantity > productInfo.Inventory)
            {
                existingItem.Quantity -= dto.Quantity;
                return BadRequest($"Estoque insuficiente. Máximo disponível: {productInfo.Inventory}.");
            }
            _context.CartItems.Update(existingItem);
        }
        else
        {
            if (dto.Quantity > productInfo.Inventory)
                return BadRequest($"Estoque insuficiente. Máximo disponível: {productInfo.Inventory}.");

            var newItem = new CartItem
            {
                CartId = cartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            cart.CartItems.Add(newItem);
        }

        await _context.SaveChangesAsync();

        var item = existingItem ?? cart.CartItems.Last();
        return Ok(new CartItemDto
        {
            CartItemId = item.CartItemId,
            ProductId = item.ProductId,
            Quantity = item.Quantity
        });
    }

    // =================================================================
    // 4. DELETE api/carts/{cartId}/items/{productId}
    // Remove um item do carrinho
    // =================================================================
    [HttpDelete("{cartId}/items/{productId}")]
    public async Task<IActionResult> RemoveItemFromCart(long cartId, long productId)
    {
        var existingItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        if (existingItem == null)
            return NotFound("Item não encontrado no carrinho.");

        _context.CartItems.Remove(existingItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // =================================================================
    // Helper para mapear entidade para DTO
    // =================================================================
    
    
    private CartDto MapToCartDto(Cart cart)
    {
        return new CartDto
        {
            CartId = cart.CartId,

            // CreatedDate é obrigatório, então converte direto
            CreatedDate = cart.CreatedDate.ToDateTime(TimeOnly.MinValue),

            // UpdatedDate é opcional, então verifica se tem valor
            UpdatedDate = cart.UpdatedDate.HasValue
                ? cart.UpdatedDate.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null,

            Items = cart.CartItems.Select(ci => new CartItemDto
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity
            }).ToList()
        };
    }


}
