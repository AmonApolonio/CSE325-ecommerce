using frontend.Models.Dtos;

namespace frontend.Services;

/// <summary>
/// Interface for authentication service - handles login, logout, and token management
/// </summary>
public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(string usuario, string senha);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task<bool> IsAuthenticatedAsync();
    Task ClearAuthenticationAsync();
}
