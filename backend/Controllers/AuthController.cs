using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SeuProjeto.Controllers // <--- IMPORTANTE: Ajuste para o namespace do seu projeto
{
    [Route("api/[controller]")] // A rota será: seusever.com/api/auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            // 1. Validação (Simulação - aqui você consultaria seu Banco de Dados)
            if (login.Usuario != "admin" || login.Senha != "1234")
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            // 2. Criar a Identidade do Usuário (Claims)
            // Claims são dados que você quer deixar "tatuados" no cookie (Id, Nome, Permissões)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Usuario),
                new Claim(ClaimTypes.Role, "Admin"), 
                new Claim("IdInterno", "99") // Exemplo de dado customizado
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Mantém logado mesmo fechando o navegador
                ExpiresUtc = DateTime.UtcNow.AddHours(8) // O cookie expira em 8 horas
            };

            // 3. Cria o Cookie criptografado e anexa na resposta
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new { message = "Login realizado com sucesso! Cookie criado." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Remove o cookie do navegador
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Deslogado com sucesso" });
        }
    }

    // Esta classe serve apenas para receber os dados do JSON
    public class LoginModel
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}