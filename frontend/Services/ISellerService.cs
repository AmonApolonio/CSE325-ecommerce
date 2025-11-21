using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for seller service - handles seller information retrieval
/// </summary>
public interface ISellerService
{
    Task<List<SellerDto>> GetAllSellersAsync();
    Task<SellerDto?> GetSellerByIdAsync(long id);
}
