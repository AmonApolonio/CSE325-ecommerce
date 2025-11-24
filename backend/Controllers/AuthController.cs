using Microsoft.AspNetCore.Mvc;
using backend.Data.Entities; // Para acessar AppDbContext, Client, Seller
// Usings para os Models LoginRequest e LoginResponse
// Usings para ITokenService
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous] // Este endpoint deve ser acessível por qualquer um
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Verificar Client
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == request.Email);
        
        if (client != null && VerifyPasswordHash(request.Password, client.PasswordHash))
        {
            var token = _tokenService.GenerateToken(client.Email, "client");
            return Ok(new LoginResponse { Token = token, Email = client.Email, Role = "client" });
        }

        // 2. Verificar Seller
        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.Email == request.Email);

        if (seller != null && VerifyPasswordHash(request.Password, seller.PasswordHash))
        {
            var token = _tokenService.GenerateToken(seller.Email, "seller");
            return Ok(new LoginResponse { Token = token, Email = seller.Email, Role = "seller" });
        }

        // Falha na autenticação
        return Unauthorized(new { Message = "Credenciais inválidas." });
    }

    // ⚠️ ATENÇÃO: Você precisa implementar uma função segura de verificação de hash
    private bool VerifyPasswordHash(string password, string storedHash)
    {
        // **IMPORTANTE**: Use uma biblioteca robusta como BCrypt.Net ou Argon2. 
        // Nunca use hashing simples como SHA256 ou métodos obsoletos.
        
        // Exemplo: return BCrypt.Net.BCrypt.Verify(password, storedHash);
        
        // Substitua esta linha pela sua lógica real de verificação de hash segura!
        return false; 
    }
}