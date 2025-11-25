// Model no Blazor (pode ser o mesmo do backend)

public class LoginRequest 
{ 
    public string Email { get; set; } 
    public string Password { get; set; } 
}

public class LoginResponse 
{ 
    public string Token { get; set; } 
    public string Email { get; set; } 
    public string Role { get; set; } 
}