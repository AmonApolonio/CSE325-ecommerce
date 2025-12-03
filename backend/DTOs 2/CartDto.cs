
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Cart;

/// <summary>
/// DTO usado para adicionar ou atualizar um item no carrinho via API.
/// </summary>
public class AddItemToCartDto
{
    [Required]
    public long ProductId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser positiva.")]
    public decimal Quantity { get; set; }
}

/// <summary>
/// DTO para retornar informações do carrinho sem referências circulares.
/// </summary>
public class CartDto
{
    public long CartId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO para retornar informações de um item do carrinho.
/// </summary>
public class CartItemDto
{
    public long CartItemId { get; set; }
    public long ProductId { get; set; }
    public decimal Quantity { get; set; }
}
