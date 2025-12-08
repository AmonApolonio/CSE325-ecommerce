
using Microsoft.AspNetCore.Mvc;
using backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Authenticates a user (Client or Seller) and returns a JWT token.
        /// </summary>
        /// <param name="request">The login request containing email, password, and user type.</param>
        /// <returns>JWT token and user information if authentication succeeds.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserType))
                return BadRequest(new { Message = "UserType is required (Client or Seller)" });

            if (request.UserType.Equals("Client", StringComparison.OrdinalIgnoreCase))
            {
                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Email == request.Email && c.PasswordHash == request.Password);

                if (client != null)
                {
                    var token = _tokenService.GenerateToken(client.Email, "Client", client.UserId);
                    return Ok(new LoginResponse
                    {
                        Token = token,
                        Email = client.Email,
                        Role = "Client"
                    });
                }
            }
            else if (request.UserType.Equals("Seller", StringComparison.OrdinalIgnoreCase))
            {
                var seller = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.Email == request.Email && s.PasswordHash == request.Password);

                if (seller != null)
                {
                    var token = _tokenService.GenerateToken(seller.Email, "Seller", seller.SellerId);
                    return Ok(new LoginResponse
                    {
                        Token = token,
                        Email = seller.Email,
                        Role = "Seller"
                    });
                }
            }

            return Unauthorized(new { Message = "Invalid credentials" });
        }

    }
}
