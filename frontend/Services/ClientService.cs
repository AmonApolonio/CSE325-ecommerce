using frontend.Models.Dtos;
using System.Net.Http.Json;

namespace frontend.Services;

/// <summary>
/// Client service implementation - handles client/user management via API
/// </summary>
public class ClientService : IClientService
{
    private readonly HttpClient _httpClient;

    public ClientService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<ClientDto>> GetAllClientsAsync()
    {
        try
        {
            var clients = await _httpClient.GetFromJsonAsync<List<ClientDto>>("/api/Clients");
            return clients ?? new List<ClientDto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching clients: {ex.Message}");
            throw;
        }
    }

    public async Task<ClientDto?> GetClientByIdAsync(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ClientDto>($"/api/Clients/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching client {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<ClientDto> CreateClientAsync(ClientDto client)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Clients", client);
            response.EnsureSuccessStatusCode();
            var createdClient = await response.Content.ReadFromJsonAsync<ClientDto>();
            return createdClient ?? throw new InvalidOperationException("Failed to create client");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error creating client: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateClientAsync(long id, ClientDto client)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Clients/{id}", client);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error updating client {id}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteClientAsync(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/Clients/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error deleting client {id}: {ex.Message}");
            throw;
        }
    }
}
