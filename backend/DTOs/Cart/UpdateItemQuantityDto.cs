namespace backend.DTOs.Cart;

/// <summary>
/// DTO (Data Transfer Object) usado para atualizar a quantidade de um item no carrinho.
/// </summary>
public class UpdateItemQuantityDto
{
    /// <summary>
    /// A nova quantidade desejada para o produto.
    /// Um valor de 0 (zero) deve ser interpretado pelo Controller como uma exclusão do item.
    /// </summary>
    public long NewQuantity { get; set; }
}