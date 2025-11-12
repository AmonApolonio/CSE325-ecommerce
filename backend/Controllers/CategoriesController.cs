using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using System.Linq;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // 1. O nome da classe deve ser Category:
    public class CategoriesController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public CategoriesController(EcommerceDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------
        // POST: api/Categories (Create Category)
        // -------------------------------------------------------------
        [HttpPost]
        // 2. O objeto de entrada e o retorno devem ser Category:
        public async Task<ActionResult<Category>> PostCategory([FromBody] Category category)
        {
            // 3. Deve usar a coleção Categories do DbContext:
            _context.Categories.Add(category); 
            
            await _context.SaveChangesAsync();

            // 4. O retorno deve usar o nome correto do método GET (GetCategoryById)
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }

        // -------------------------------------------------------------
        // GET: api/Categories/{id} (Required for CreatedAtAction)
        // -------------------------------------------------------------
        [HttpGet("{id}")]
        // 5. O objeto de retorno deve ser Category:
        public async Task<ActionResult<Category>> GetCategoryById(long id)
        {
            // 6. Deve buscar na coleção Categories:
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return category;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            // Checks if the Categories table is empty (or if the DbSet is null)
            if (_context.Categories == null)
            {
                return NotFound("Category set is null.");
            }
            
            // 1. Fetches all records from the Categories table
            // 2. ToListAsync() executes the query in the database asynchronously
            return await _context.Categories.ToListAsync();
        }
    }
}