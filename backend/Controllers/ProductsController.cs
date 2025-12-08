// ProductsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using backend.Data.Entities;  
using System.Linq; // Needed for the Any() method

namespace backend.Controllers
{
    [Route("api/[controller]")] // Defines the base route as /api/products
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context; // Assuming this is your DbContext

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------
        // GET (All Products OR Filtered by Name, SellerId AND/OR CategoryId)
        // -------------------------------------------------------------
        [HttpGet]
        // Rota: /api/Products?name=termo&sellerId=123&categoryId=456
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] string name = null,
            [FromQuery] long? sellerId = null,
            [FromQuery] long? categoryId = null) // NOVO PARÂMETRO
        {
            IQueryable<Product> products = _context.Products;

            // 1. Filtrar por Nome (se fornecido)
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name)); 
            }
            
            // 2. Filtrar por SellerId (se fornecido)
            if (sellerId.HasValue)
            {
                // Filtra os resultados que já podem ter sido filtrados por nome
                products = products.Where(p => p.SellerId == sellerId.Value); 
            }

            // 3. Filtrar por CategoryId (se fornecido)
            if (categoryId.HasValue)
            {
                // Aplica a filtragem ao resultado existente.
                // Assumimos que o campo na entidade Product é Product.CategoryId
                products = products.Where(p => p.CategoryId == categoryId.Value); 
            }

            return await products.ToListAsync();
        }

        // -------------------------------------------------------------
        // POST Method to CREATE a new product
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            product.ProductId = 0;
            
            // 1. Adds the product to the database context
            _context.Products.Add(product);
            
            // 2. Saves the changes
            await _context.SaveChangesAsync();

            // 3. Returns the 201 Created code and the link to the new resource
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