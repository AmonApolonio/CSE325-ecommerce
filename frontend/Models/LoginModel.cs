// Model no Blazor (pode ser o mesmo do backend)


public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}


public class LoginResponse 
{ 
    public string Token { get; set; } 
    public string Email { get; set; } 
    public string Role { get; set; } 
}