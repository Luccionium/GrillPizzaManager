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

                return Ok(foods);
            }
            catch (Exception ex)
            {
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
                    return NotFound();

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

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<Food> Post([FromBody] Food value)
        {
            try
            {
                _context.Foods.Add(value);
                _context.SaveChanges();
                return Created($"/api/Food/{value.Id}", value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Food> Put(int id, [FromBody] Food value)
        {
            try
            {
                var food = _context.Foods.FirstOrDefault(x => x.Id == id);
                if (food == null)
                    return NotFound();

                food.Title = value.Title;
                food.Description = value.Description;
                food.Price = value.Price;
                food.ImageUrl = value.ImageUrl;
                food.IsAvailable = value.IsAvailable;
                food.FoodCategoryId = value.FoodCategoryId;
                _context.SaveChanges();
                return Ok(food);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Food> Delete(int id)
        {
            try
            {
                var food = _context.Foods.FirstOrDefault(x => x.Id == id);
                if (food == null)
                    return NotFound();

                _context.Foods.Remove(food);
                _context.SaveChanges();
                return Ok(food);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}