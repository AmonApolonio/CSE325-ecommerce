using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using System.Linq;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public CategoriesController(EcommerceDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------
        // 1. POST: api/Categories (CREATE)
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromBody] Category category)
        {
            _context.Categories.Add(category); 
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }

        // -------------------------------------------------------------
        // 2. GET: api/Categories (READ ALL)
        // -------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound("Category set is null.");
            }
            
            return await _context.Categories.ToListAsync();
        }

        // -------------------------------------------------------------
        // 3. GET: api/Categories/{id} (READ BY ID)
        // -------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(long id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        // -------------------------------------------------------------
        // 4. PUT: api/Categories/{id} (UPDATE) ✏️
        // -------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(long id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("Category ID mismatch.");
            }

            // Marca o objeto como modificado, preparando para o Update
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Verifica se a categoria ainda existe
                if (!_context.Categories.Any(e => e.CategoryId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Retorna 204 Success (Nenhum Conteúdo)
        }

        // -------------------------------------------------------------
        // 5. DELETE: api/Categories/{id} (DELETE) 🗑️
        // -------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 Success
        }
    }
}