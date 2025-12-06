using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Cart;

/// <summary>
/// DTO usado para adicionar ou atualizar um item no carrinho via API.
/// </summary>
public class AddItemToCartDto
{
    // O ID do produto que está sendo adicionado/atualizado.
    [Required]
    public long ProductId { get; set; }

    // A quantidade a ser adicionada (pode ser 1, -1 para decremento, ou a quantidade total).
    // Usamos Decimal para ser consistente com o modelo CartItem.
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser positiva.")]
    public decimal Quantity { get; set; }
}