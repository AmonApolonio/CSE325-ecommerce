using backend.Data.Entities;
using System.Linq;

namespace backend.BusinessLogic;

/// <summary>
/// Métodos de Extensão para a Entidade Cart.
/// Adiciona lógica de negócios sem modificar a classe Cart original (Model).
/// </summary>
public static class CartExtensions
{
    /// <summary>
    /// Calcula o valor total do carrinho de compras.
    /// O resultado é em CENTAVOS (int) para manter a precisão financeira.
    /// 
    /// É crucial que a coleção CartItems E Product sejam carregadas (eager loading) 
    /// no Entity Framework Core antes de chamar este método, 
    /// para evitar problemas de N+1 queries.
    /// Exemplo de carregamento: 
    /// context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(...)
    /// </summary>
    /// <param name="cart">A instância do carrinho (this Cart) que será estendida.</param>
    /// <returns>O valor total calculado em CENTAVOS (tipo Decimal).</returns>
    public static decimal CalculateTotal(this Cart cart)
    {
        // Verifica se o carrinho ou seus itens estão nulos
        if (cart == null || cart.CartItems == null)
        {
            return 0m;
        }

        // Usa LINQ para calcular a soma total
        // ci.Product.Price é INT (centavos). 
        // ci.Quantity é DECIMAL.
        // A multiplicação é feita de forma segura: (decimal) * (int) resulta em decimal.
        decimal total = cart.CartItems
            .Sum(ci => ci.Quantity * (decimal)(ci.Product?.Price ?? 0));
            
        // O resultado 'total' é em centavos (ex: 12500 para R$ 125,00)
        return total;
    }

    /// <summary>
    /// Método auxiliar para converter o total em centavos para a representação de moeda (Reais).
    /// </summary>
    /// <param name="cart">A instância do carrinho.</param>
    /// <returns>O valor total calculado em Reais (Decimal, ex: 125.00).</returns>
    public static decimal CalculateTotalInCurrency(this Cart cart)
    {
        return cart.CalculateTotal() / 100m;
    }
}