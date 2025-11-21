using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for client service - handles client/user management
/// </summary>
public interface IClientService
{
    Task<List<ClientDto>> GetAllClientsAsync();
    Task<ClientDto?> GetClientByIdAsync(long id);
    Task<ClientDto> CreateClientAsync(ClientDto client);
    Task UpdateClientAsync(long id, ClientDto client);
    Task DeleteClientAsync(long id);
}
