using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi.Data;
using RaceDayApi.Helpers;
using RaceDayApi.Models;
using RaceDayApi.Models.DTOs;

namespace RaceDayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cats = await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    DistanceKM = c.DistanceKM
                }).ToListAsync();

            return Ok(cats);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return NotFound(new { message = "Category not found." });

            return Ok(new CategoryDto
            {
                CategoryId = c.CategoryID,
                CategoryName = c.CategoryName,
                Description = c.Description,
                DistanceKM = c.DistanceKM
            });
        }

        // POST: api/categories
        [HttpPost]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _context.Categories.AnyAsync(c => c.CategoryName == dto.CategoryName))
                return Conflict(new { message = "Category name already exists." });

            var cat = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                DistanceKM = dto.DistanceKM
            };

            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = cat.CategoryID }, new CategoryDto
            {
                CategoryId = cat.CategoryID,
                CategoryName = cat.CategoryName,
                Description = cat.Description,
                DistanceKM = cat.DistanceKM
            });
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound(new { message = "Category not found." });

            if (!string.IsNullOrEmpty(dto.CategoryName))
            {
                var exists = await _context.Categories.AnyAsync(c => c.CategoryName == dto.CategoryName && c.CategoryID != id);
                if (exists) return Conflict(new { message = "Category name already exists." });
                cat.CategoryName = dto.CategoryName;
            }

            if (dto.Description != null) cat.Description = dto.Description;
            if (dto.DistanceKM.HasValue) cat.DistanceKM = dto.DistanceKM.Value;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Category updated." });
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        [RoleAuthorize("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound(new { message = "Category not found." });

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}