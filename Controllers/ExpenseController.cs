using BudgetManager.DTOs.ExpensesDTOs;
using BudgetManager.DTOs.MontlySummaryDTOs;
using BudgetManager.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly BudgetManagerDbContext _context;
        public ExpenseController(BudgetManagerDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseReadDto>>> GetAll()
        {
            var items = await _context.Expenses.Include(e => e.Category).AsNoTracking().ToListAsync();
            var result = items.Select(item => new ExpenseReadDto
            {
                ExpenseID = item.ExpenseID,
                CategoryID = item.CategoryID,
                CategoryName = item.Category.Name,
                Price = item.Price,
                Comment = item.Comment,
                Date = item.Date,
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var item = await _context.Expenses.Include(e => e.Category)
                    .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ExpenseID == id);

            if (item == null)
                return NotFound();

            return Ok(new ExpenseReadDto
            {
                ExpenseID = item.ExpenseID,
                CategoryID = item.CategoryID,
                CategoryName = item.Category.Name,
                Price = item.Price,
                Comment = item.Comment,
                Date = item.Date,   
            });
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ExpenseCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = new Expense
            {
                CategoryID = dto.CategoryID,
                Price = dto.Price,
                Comment = dto.Comment,
                Date = dto.Date,
            };

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryID == dto.CategoryID);
            if (!categoryExists)
                return BadRequest("Category not found.");

            _context.Expenses.Add(item);
            await _context.SaveChangesAsync();

            var createdItem = await _context.Expenses
              .Include(e => e.Category)
              .AsNoTracking()
              .FirstOrDefaultAsync(e => e.ExpenseID == item.ExpenseID);

            var result = new ExpenseReadDto
            {
                ExpenseID = createdItem.ExpenseID,
                CategoryID = createdItem.CategoryID,
                CategoryName = createdItem.Category?.Name,
                Price = createdItem.Price,
                Comment = createdItem.Comment,
                Date = createdItem.Date,
            };
            return CreatedAtAction(nameof(GetById), new { id = result.ExpenseID }, result);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] ExpenseUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = await _context.Expenses.FindAsync(dto.ExpenseID);
            if (item == null)
                return NotFound();

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryID == dto.CategoryID);
            if (!categoryExists)
                return BadRequest("Category not found.");

            item.CategoryID = dto.CategoryID;
            item.Price = dto.Price;
            item.Comment = dto.Comment;
            item.Date = dto.Date;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _context.Expenses.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Expenses.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("filter")]
         public async Task<ActionResult<IEnumerable<ExpenseReadDto>>> Filter(
            [FromQuery] int? categoryId,
            [FromQuery] decimal? min,
            [FromQuery] decimal? max,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? sortBy,
            [FromQuery] string? order)
            {
            var query = _context.Expenses
                .Include(e => e.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryID == categoryId.Value);

            if (min.HasValue)
                query = query.Where(e => e.Price >= min.Value);

            if (max.HasValue)
                query = query.Where(e => e.Price <= max.Value);

            if (from.HasValue)
                query = query.Where(e => e.Date >= from.Value);

            if (to.HasValue)
                query = query.Where(e => e.Date <= to.Value);

            bool desc = (order ?? "").ToLower() == "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "price" => desc ? query.OrderByDescending(e => e.Price) : query.OrderBy(e => e.Price),
                    "date" => desc ? query.OrderByDescending(e => e.Date) : query.OrderBy(e => e.Date),
                    _ => query.OrderBy(e => e.ExpenseID) 
                };
            }

            var items = await query.AsNoTracking().ToListAsync();

            var result = items.Select(item => new ExpenseReadDto
            {
                ExpenseID = item.ExpenseID,
                CategoryID = item.CategoryID,
                CategoryName = item.Category?.Name ?? "",
                Price = item.Price,
                Comment = item.Comment,
                Date = item.Date
            });

            return Ok(result);
        }

        [HttpGet("monthly-summary")]
        public async Task<ActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
        {
            var result = await _context.Expenses
                .Where(e => e.Date.Year == year && e.Date.Month == month)
                .GroupBy(e => e.Category.Name)
                .Select(g => new MonthlySummaryDto
                {
                    Category = g.Key,
                    Total = g.Sum(e => e.Price)
                })
                .ToListAsync();

            return Ok(result);
        }

    }
}
