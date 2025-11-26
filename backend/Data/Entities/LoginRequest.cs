// LoginRequest.cs (Dados enviados pelo Frontend)

namespace backend.Data.Entities;


public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // "Client" ou "Seller"
}


// LoginResponse.cs (Dados retornados ao Frontend)
public class LoginResponse
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}