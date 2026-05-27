using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodCategoryController : ControllerBase
    {
        private readonly RwaProjektContext _context;

        public FoodCategoryController(RwaProjektContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FoodCategory>> Get()
        {
            try
            {
                return Ok(_context.FoodCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<FoodCategory> Get(int id)
        {
            try
            {
                var category = _context.FoodCategories.FirstOrDefault(x => x.Id == id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<FoodCategory> Post([FromBody] FoodCategory value)
        {
            try
            {
                _context.FoodCategories.Add(value);
                _context.SaveChanges();
                return Created($"/api/FoodCategory/{value.Id}", value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<FoodCategory> Put(int id, [FromBody] FoodCategory value)
        {
            try
            {
                var category = _context.FoodCategories.FirstOrDefault(x => x.Id == id);
                if (category == null)
                    return NotFound();

                category.Name = value.Name;
                _context.SaveChanges();
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<FoodCategory> Delete(int id)
        {
            try
            {
                var category = _context.FoodCategories.FirstOrDefault(x => x.Id == id);
                if (category == null)
                    return NotFound();

                _context.FoodCategories.Remove(category);
                _context.SaveChanges();
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}