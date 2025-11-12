// ProductsController.cs

using Microsoft.AspNetCore.Mvc;
using backend.Models; 
using Microsoft.EntityFrameworkCore; 
using backend.Data;  
using System.Linq; // Needed for the Any() method

namespace backend.Controllers
{
    [Route("api/[controller]")] // Defines the base route as /api/products
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly EcommerceDbContext _context; // Assuming this is your DbContext

        public ProductsController(EcommerceDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------
        // GET (All Products)
        // -------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            // Retrieves all products from the database
            return await _context.Products.ToListAsync();
        }

        // -------------------------------------------------------------
        // POST Method to CREATE a new product
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            // 1. Adds the product to the database context
            _context.Products.Add(product);
            
            // 2. Saves the changes
            await _context.SaveChangesAsync();

            // 3. Returns the 201 Created code and the link to the new resource
            // The name of the GetProductById method must match your GET method by ID.
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }
        
        // Example of the GET method by ID, referenced by CreatedAtAction:
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // -------------------------------------------------------------
        // 🔑 PUT Method to UPDATE an existing product
        // -------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, [FromBody] Product product)
        {
            // Check if the ID in the URL matches the ID in the request body
            if (id != product.ProductId)
            {
                return BadRequest(); // Returns HTTP 400
            }
            
            // Marks the entity as modified so EF Core knows to update it
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                // Saves the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check if the product actually exists before throwing a concurrency error
                if (!_context.Products.Any(e => e.ProductId == id))
                {
                    return NotFound(); // Returns HTTP 404
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Returns HTTP 204 (Success, no content to return)
        }
        
        // -------------------------------------------------------------
        // 🔑 DELETE Method to DELETE a product
        // -------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            // 1. Find the product by ID
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound(); // Returns HTTP 404 if the product doesn't exist
            }

            // 2. Remove the product from the context
            _context.Products.Remove(product);
            
            // 3. Save the changes to the database
            await _context.SaveChangesAsync();

            return NoContent(); // Returns HTTP 204 (Success, no content to return)
        }
    }
}