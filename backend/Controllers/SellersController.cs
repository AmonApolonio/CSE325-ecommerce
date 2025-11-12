using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using System.Linq;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public SellersController(EcommerceDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------
        // 1. POST: api/Sellers (CREATE)
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Seller>> PostSeller([FromBody] Seller seller)
        {
            // 1. Add the seller to the context
            _context.Sellers.Add(seller);
            
            // 2. Save changes to the database
            await _context.SaveChangesAsync();

            // 3. Return 201 Created status and the created object
            return CreatedAtAction(nameof(GetSellerById), new { id = seller.SellerId }, seller);
        }

        // -------------------------------------------------------------
        // 2. GET: api/Sellers (READ ALL) 📄
        // -------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seller>>> GetSellers()
        {
            if (_context.Sellers == null)
            {
                return NotFound("Seller set is null.");
            }
            
            // Fetches all sellers asynchronously
            return await _context.Sellers.ToListAsync();
        }

        // -------------------------------------------------------------
        // 3. GET: api/Sellers/{id} (READ BY ID)
        // -------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Seller>> GetSellerById(long id)
        {
            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }
            return seller;
        }
        
        // -------------------------------------------------------------
        // 4. PUT: api/Sellers/{id} (UPDATE) ✏️
        // -------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSeller(long id, [FromBody] Seller seller)
        {
            if (id != seller.SellerId)
            {
                return BadRequest("Seller ID mismatch.");
            }

            // Mark the object as modified for updating
            _context.Entry(seller).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check if the seller still exists
                if (!_context.Sellers.Any(e => e.SellerId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Returns 204 Success
        }

        // -------------------------------------------------------------
        // 5. DELETE: api/Sellers/{id} (DELETE) 🗑️
        // -------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeller(long id)
        {
            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }

            _context.Sellers.Remove(seller);
            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 Success
        }
    }
}