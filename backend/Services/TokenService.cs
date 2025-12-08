
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface ITokenService
{
    string GenerateToken(string email, string role, long userId);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(string email, string role, long userId)
    {
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing");
        var issuer = _config["Jwt:Issuer"] ?? "ecommerce";
        var audiences = _config.GetSection("Jwt:Audiences").Get<string[]>() ?? new[] { issuer };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims do usuário
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Cria o token com o primeiro audience como padrão
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audiences.FirstOrDefault(), // usa o primeiro audience
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        // Adiciona todos os audiences no payload
        token.Payload["aud"] = audiences;

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
