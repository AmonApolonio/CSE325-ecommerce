using frontend.Models.Dtos;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace frontend.Services;

/// <summary>
/// Authentication service implementation - handles JWT token management and login/logout
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private const string TokenKey = "authToken";

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<LoginResponseDto?> LoginAsync(string usuario, string senha)
    {
        try
        {
            var loginRequest = new LoginRequestDto
            {
                Usuario = usuario,
                Senha = senha
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            
            if (loginResponse?.Token != null)
            {
                await SetTokenAsync(loginResponse.Token);
            }

            return loginResponse;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            throw;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _httpClient.PostAsync("/api/Auth/logout", null);
            await ClearAuthenticationAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error during logout: {ex.Message}");
            // Clear token even if logout API call fails
            await ClearAuthenticationAsync();
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        // In a real Blazor app, use local storage or session storage
        // For now, we'll use a simple in-memory approach
        // In production, use Blazor.LocalStorage or Blazor.SessionStorage
        return await Task.FromResult(GetTokenFromMemory());
    }

    public async Task SetTokenAsync(string token)
    {
        SetTokenInMemory(token);
        await Task.CompletedTask;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task ClearAuthenticationAsync()
    {
        ClearTokenFromMemory();
        await Task.CompletedTask;
    }

    // Simple in-memory token storage (should use Blazor.LocalStorage in production)
    private static string? _tokenCache;

    private static string? GetTokenFromMemory() => _tokenCache;

    private static void SetTokenInMemory(string token)
    {
        _tokenCache = token;
    }

    private static void ClearTokenFromMemory()
    {
        _tokenCache = null;
    }
}
