using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(_anonymous);

        // Se houver token, construa o usuário com base nas claims
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwtAuth")));
    }

    public void MarkUserAsAuthenticated(LoginResponse loginResponse)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, loginResponse.Email),
            new Claim(ClaimTypes.Role, loginResponse.Role)
        };
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuth"));
        
        // Notifica o Blazor que o estado mudou
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticatedUser)));
    }

    public void MarkUserAsLoggedOut()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    // Método auxiliar para extrair Claims do JWT (sem validação de assinatura)
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        // Decodifica a parte do payload (segunda parte do token)
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        
        var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        
        // O JWT usa 'role' e 'sub' por padrão, mas o ASP.NET Core usa ClaimTypes.Role, etc.
        // Adicione aqui a lógica de mapeamento se necessário.
        
        return claims;
    }
    
    // Ajuda a decodificar corretamente a string Base64Url
    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}