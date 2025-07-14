using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BudgetManager.Models;
using Microsoft.EntityFrameworkCore;
using BudgetManager.DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Authorization;
namespace BudgetManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly BudgetManagerDbContext _context;
        public CategoryController(BudgetManagerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAll()
        {
            var items = await _context.Categories.AsNoTracking().ToListAsync();
            var result = items.Select(item => new CategoryReadDto
            {
                CategoryID = item.CategoryID,
                Name = item.Name,
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var item = await _context.Categories
                    .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (item == null)
                return NotFound();
         
            return Ok( new CategoryReadDto
            {
                CategoryID = item.CategoryID,
                Name = item.Name,
            });
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = new Category
            {
                Name = dto.Name,
            };
            _context.Categories.Add(item);
            await _context.SaveChangesAsync();

            var result = new CategoryReadDto
            {
                CategoryID = item.CategoryID,
                Name = item.Name,
            };
            return CreatedAtAction(nameof(GetById), new { id = result.CategoryID }, result);
        }

      [HttpPut]
        public async Task<ActionResult> Update([FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = await _context.Categories.FindAsync(dto.CategoryID);
            if (item == null)
                return NotFound();

            item.Name = dto.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _context.Categories.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Categories.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    
}
}
