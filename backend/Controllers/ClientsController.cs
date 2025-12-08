using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // 1. POST: api/Clients (CREATE)
        // =================================================================
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient([FromBody] Client client)
        {
            // Nota: Em um sistema real, aqui você faria hash da senha antes de salvar.
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            // AJUSTE: Usando client.UserId
            return CreatedAtAction(nameof(GetClientById), new { id = client.UserId }, client);
        }

        // =================================================================
        // 2. GET: api/Clients (READ ALL)
        // =================================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            if (_context.Clients == null)
            {
                return NotFound("Client set is null.");
            }
            return await _context.Clients.ToListAsync();
        }

        // =================================================================
        // 3. GET: api/Clients/{id} (READ BY ID)
        // =================================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClientById(long id)
        {
            // O FindAsync usa a chave primária, que é UserId
            var client = await _context.Clients.FindAsync(id); 

            if (client == null)
            {
                return NotFound();
            }
            return client;
        }

        // =================================================================
        // 4. PUT: api/Clients/{id} (UPDATE)
        // =================================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(long id, [FromBody] Client client)
        {
            // AJUSTE: Comparando id com client.UserId
            if (id != client.UserId)
            {
                return BadRequest("Client ID mismatch.");
            }

            // O Entity Framework Core rastreia o objeto e marca-o como modificado.
            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // AJUSTE: Verificando se e.UserId existe
                if (!_context.Clients.Any(e => e.UserId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Retorna 204 Success
        }

        // =================================================================
        // 5. DELETE: api/Clients/{id} (DELETE)
        // =================================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(long id)
        {
            // O FindAsync usa a chave primária, que é UserId
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 Success
        }

        
// =================================================================
// 6. GET: api/Clients/by-email/{email} (READ BY EMAIL)
// =================================================================
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<Client>> GetClientByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);

            if (client == null)
            {
                return NotFound($"Client with email '{email}' not found.");
            }

            return Ok(client);
        }
// =================================================================
//PUT: api/Clients/details/{id} (UPDATE DETAILS ONLY)
// =================================================================
        [HttpPut("details/{id}")]
        public async Task<IActionResult> PutClientDetails(long id, [FromBody] ClientDetailsDto detailsDto)
        {
            if (id != detailsDto.UserId)
            {
                return BadRequest("Client ID mismatch.");
            }

            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == id);

            if (existingClient == null)
            {
                return NotFound();
            }

            // Mapeia APENAS os campos do DTO para o objeto existente
            existingClient.Name = detailsDto.Name;
            existingClient.PhoneNumber = detailsDto.PhoneNumber;
            existingClient.Address1 = detailsDto.Address1;
            existingClient.Address2 = detailsDto.Address2;
            existingClient.City = detailsDto.City;
            existingClient.State = detailsDto.State;
            existingClient.Country = detailsDto.Country;
            existingClient.ZipCode = detailsDto.ZipCode;

            // O Entity Framework rastreia as mudanças e atualiza apenas o que mudou.
            // Não precisa de EntityState.Modified se você usou FindAsync() e alterou as propriedades.
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!_context.Clients.Any(e => e.UserId == id))
            {
                return NotFound();
            }

            return NoContent(); // Retorna 204 Success
        }
    }

}