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
        // POST: api/Sellers (Create Seller)
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Seller>> PostSeller([FromBody] Seller seller)
        {
            // 1. Add the seller to the context
            _context.Sellers.Add(seller);
            
            // 2. Save changes to the database
            await _context.SaveChangesAsync();

            // 3. Return 201 Created status and the created object
            // You will need a GET method for the CreatedAtAction to work correctly.
            return CreatedAtAction(nameof(GetSellerById), new { id = seller.SellerId }, seller);
        }

        // -------------------------------------------------------------
        // GET: api/Sellers/{id} (Required for CreatedAtAction)
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
    }
}