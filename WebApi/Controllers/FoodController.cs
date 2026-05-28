using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly RwaProjektContext _context;

        public FoodController(RwaProjektContext context)
        {
            _context = context;
        }

        private void WriteLog(string level, string message)
        {
            _context.Logs.Add(new Log
            {
                Level = level,
                Message = message
            });
            _context.SaveChanges();
        }

        [HttpGet]
        public ActionResult<IEnumerable<FoodDto>> Get()
        {
            try
            {
                var foods = _context.Foods
                    .Include(f => f.FoodCategory)
                    .Include(f => f.FoodAllergens)
                        .ThenInclude(fa => fa.Allergen)
                    .Select(f => new FoodDto
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Description = f.Description,
                        Price = f.Price,
                        ImageUrl = f.ImageUrl,
                        IsAvailable = f.IsAvailable,
                        CategoryName = f.FoodCategory.Name,
                        Allergens = f.FoodAllergens
                            .Select(fa => fa.Allergen.Name)
                            .ToList()
                    });

                WriteLog("INFO", "Food list retrieved.");
                return Ok(foods);
            }
            catch (Exception ex)
            {
                WriteLog("ERROR", $"Error retrieving food list: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<FoodDto> Get(int id)
        {
            try
            {
                var food = _context.Foods
                    .Include(f => f.FoodCategory)
                    .Include(f => f.FoodAllergens)
                        .ThenInclude(fa => fa.Allergen)
                    .FirstOrDefault(x => x.Id == id);

                if (food == null)
                {
                    WriteLog("WARN", $"Food with id={id} not found.");
                    return NotFound();
                }

                var dto = new FoodDto
                {
                    Id = food.Id,
                    Title = food.Title,
                    Description = food.Description,
                    Price = food.Price,
                    ImageUrl = food.ImageUrl,
                    IsAvailable = food.IsAvailable,
                    CategoryName = food.FoodCategory.Name,
                    Allergens = food.FoodAllergens
                        .Select(fa => fa.Allergen.Name)
                        .ToList()
                };

                WriteLog("INFO", $"Food with id={id} retrieved.");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                WriteLog("ERROR", $"Error retrieving food with id={id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Food> Post([FromBody] Food value)
        {
            try
            {
                _context.Foods.Add(value);
                _context.SaveChanges();
                WriteLog("INFO", $"Food with id={value.Id} created.");
                return Created($"/api/Food/{value.Id}", value);
            }
            catch (Exception ex)
            {
                WriteLog("ERROR", $"Error creating food: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Food> Put(int id, [FromBody] Food value)
        {
            try
            {
                var food = _context.Foods.FirstOrDefault(x => x.Id == id);
                if (food == null)
                {
                    WriteLog("WARN", $"Cannot update food with id={id}, not found.");
                    return NotFound();
                }

                food.Title = value.Title;
                food.Description = value.Description;
                food.Price = value.Price;
                food.ImageUrl = value.ImageUrl;
                food.IsAvailable = value.IsAvailable;
                food.FoodCategoryId = value.FoodCategoryId;
                _context.SaveChanges();

                WriteLog("INFO", $"Food with id={id} updated.");
                return Ok(food);
            }
            catch (Exception ex)
            {
                WriteLog("ERROR", $"Error updating food with id={id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<Food> Delete(int id)
        {
            try
            {
                var food = _context.Foods.FirstOrDefault(x => x.Id == id);
                if (food == null)
                {
                    WriteLog("WARN", $"Cannot delete food with id={id}, not found.");
                    return NotFound();
                }

                _context.Foods.Remove(food);
                _context.SaveChanges();

                WriteLog("INFO", $"Food with id={id} deleted.");
                return Ok(food);
            }
            catch (Exception ex)
            {
                WriteLog("ERROR", $"Error deleting food with id={id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<FoodDto>> Search(
            string? title = null,
            int? categoryId = null,
            int page = 1,
            int count = 10)
        {
            try
            {
                var query = _context.Foods
                    .Include(f => f.FoodCategory)
                    .Include(f => f.FoodAllergens)
                        .ThenInclude(fa => fa.Allergen)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(title))
                    query = query.Where(f => f.Title.Contains(title));

                if (categoryId.HasValue)
                    query = query.Where(f => f.FoodCategoryId == categoryId);

                var total = query.Count();
                var foods = query
                    .Skip((page - 1) * count)
                    .Take(count)
                    .Select(f => new FoodDto
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Description = f.Description,
                        Price = f.Price,
                        ImageUrl = f.ImageUrl,
                        IsAvailable = f.IsAvailable,
                        CategoryName = f.FoodCategory.Name,
                        Allergens = f.FoodAllergens
                            .Select(fa => fa.Allergen.Name)
                            .ToList()
                    })
                    .ToList();

                WriteLog("INFO", $"Food search executed. Title='{title}', CategoryId={categoryId}, Page={page}, Count={count}. Results: {total}");

                return Ok(new
                {
                    total,
                    page,
                    count,
                    data = foods
                });
            }
            catch (Exception ex)
            {
                WriteLog("ERROR", $"Error searching food: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}