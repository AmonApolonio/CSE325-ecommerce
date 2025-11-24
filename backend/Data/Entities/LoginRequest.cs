// LoginRequest.cs (Dados enviados pelo Frontend)

namespace backend.Data.Entities;

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

// LoginResponse.cs (Dados retornados ao Frontend)
public class LoginResponse
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}