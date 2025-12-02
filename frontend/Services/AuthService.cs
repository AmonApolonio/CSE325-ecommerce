using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using frontend.Models; // Para usar LoginRequest e LoginResponse

namespace frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        // O HttpClient será injetado
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                // Este endpoint deve ser o mesmo que você criou no Backend (AuthController)
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                // Lança uma exceção se a resposta não for 2xx (ex: 401 Unauthorized)
                response.EnsureSuccessStatusCode(); 
                
                // Se bem-sucedido, desserializa e retorna a resposta
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result == null)
                {
                    throw new ApplicationException("Invalid response from server.");
                }
                return result;
            }
            catch (HttpRequestException ex)
            {
                // Tratar ou relançar exceções de HTTP (ex: falha de conexão ou 401)
                throw new ApplicationException("Falha na chamada da API de Login.", ex);
            }
        }
        
        // Você pode adicionar aqui métodos futuros, como RegisterAsync, Logout, etc.
    }
}