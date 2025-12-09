using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SellersController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------
        // 1. POST: api/Sellers (CREATE)
        // ---------------------------------------------------------
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
// =================================================================
// 6. GET: api/Sellers/by-email/{email} (READ BY EMAIL)
// =================================================================
        
    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<Seller>> GetSellerByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email is required.");
        }

        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.Email == email);

        if (seller == null)
        {
            return NotFound($"Seller with email '{email}' not found.");
        }

        return Ok(seller);
    }
    [HttpPut("details/{id}")] 
    public async Task<IActionResult> UpdateSellerDetails(long id, SellerDetailsDto sellerDto)
    {
        // 1. ID Verification:
        if (id != sellerDto.SellerId)
        {
            return BadRequest("Seller ID mismatch.");
        }

        // 2. Locate the Seller in the DB (with tracking, but without loading collections):
        var sellerToUpdate = await _context.Sellers
            .FirstOrDefaultAsync(s => s.SellerId == id);

        if (sellerToUpdate == null)
        {
            return NotFound(); // 404
        }

        // 3. Map from DTO to the DB Model (Entity):
        // Use AutoMapper or SAFE manual mapping
        sellerToUpdate.Name = sellerDto.Name;
        sellerToUpdate.PhotoUrl = sellerDto.PhotoUrl;
        sellerToUpdate.AboutMe = sellerDto.AboutMe;
        sellerToUpdate.PhoneNumber = sellerDto.PhoneNumber;
        sellerToUpdate.Address1 = sellerDto.Address1;
        sellerToUpdate.Address2 = sellerDto.Address2;
        sellerToUpdate.City = sellerDto.City;
        sellerToUpdate.State = sellerDto.State;
        sellerToUpdate.Country = sellerDto.Country;
        sellerToUpdate.ZipCode = sellerDto.ZipCode;
        
        // NOTE: Fields like Email, PasswordHash, and CommisionRate ARE NOT TOUCHED.

        // 4. Save changes to the database:
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // ... (Concurrency handling logic if needed)
            throw;
        }
        catch (Exception)
        {
            // ... (General error handling logic)
            return StatusCode(500, "Internal server error during update.");
        }

        return NoContent(); // 204 Success (no content returned)
    }

    }
}