using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergenController : ControllerBase
    {
        private readonly RwaProjektContext _context;

        public AllergenController(RwaProjektContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Allergen>> Get()
        {
            try
            {
                return Ok(_context.Allergens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Allergen> Get(int id)
        {
            try
            {
                var allergen = _context.Allergens.FirstOrDefault(x => x.Id == id);
                if (allergen == null)
                    return NotFound();

                return Ok(allergen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Allergen> Post([FromBody] Allergen value)
        {
            try
            {
                _context.Allergens.Add(value);
                _context.SaveChanges();
                return Created($"/api/Allergen/{value.Id}", value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Allergen> Put(int id, [FromBody] Allergen value)
        {
            try
            {
                var allergen = _context.Allergens.FirstOrDefault(x => x.Id == id);
                if (allergen == null)
                    return NotFound();

                allergen.Name = value.Name;
                allergen.Description = value.Description;
                _context.SaveChanges();
                return Ok(allergen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<Allergen> Delete(int id)
        {
            try
            {
                var allergen = _context.Allergens.FirstOrDefault(x => x.Id == id);
                if (allergen == null)
                    return NotFound();

                _context.Allergens.Remove(allergen);
                _context.SaveChanges();
                return Ok(allergen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}