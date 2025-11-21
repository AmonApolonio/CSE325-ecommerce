using frontend.Models.Dtos;
using System.Net.Http.Json;

namespace frontend.Services;

/// <summary>
/// Seller service implementation - retrieves seller information from API
/// </summary>
public class SellerService : ISellerService
{
    private readonly HttpClient _httpClient;

    public SellerService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<SellerDto>> GetAllSellersAsync()
    {
        try
        {
            var sellers = await _httpClient.GetFromJsonAsync<List<SellerDto>>("/api/Sellers");
            return sellers ?? new List<SellerDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching sellers: {ex.Message}");
            throw;
        }
    }

    public async Task<SellerDto?> GetSellerByIdAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<SellerDto>($"/api/Sellers/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching seller {id}: {ex.Message}");
            throw;
        }
    }
}
