using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using System.Linq; // Adicione este using para o método .Any()

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public ClientsController(EcommerceDbContext context)
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
    }
}