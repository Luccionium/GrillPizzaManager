using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AllergenController : Controller
    {
        private readonly RwaProjektContext _context;

        public AllergenController(RwaProjektContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var allergens = _context.Allergens.ToList();
            return View(allergens);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Allergen allergen)
        {
            if (!ModelState.IsValid)
                return View(allergen);

            var existing = _context.Allergens
                .FirstOrDefault(x => x.Name == allergen.Name);
            if (existing != null)
            {
                ModelState.AddModelError("Name", "Alergen s tim nazivom već postoji");
                return View(allergen);
            }

            _context.Allergens.Add(allergen);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var allergen = _context.Allergens.FirstOrDefault(x => x.Id == id);
            if (allergen == null)
                return NotFound();
            return View(allergen);
        }

        [HttpPost]
        public IActionResult Edit(int id, Allergen allergen)
        {
            if (!ModelState.IsValid)
                return View(allergen);

            var existing = _context.Allergens
                .FirstOrDefault(x => x.Name == allergen.Name && x.Id != id);
            if (existing != null)
            {
                ModelState.AddModelError("Name", "Alergen s tim nazivom već postoji");
                return View(allergen);
            }

            var existing2 = _context.Allergens.FirstOrDefault(x => x.Id == id);
            if (existing2 == null)
                return NotFound();

            existing2.Name = allergen.Name;
            existing2.Description = allergen.Description;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var allergen = _context.Allergens.FirstOrDefault(x => x.Id == id);
            if (allergen == null)
                return NotFound();
            return View(allergen);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var allergen = _context.Allergens.FirstOrDefault(x => x.Id == id);
            if (allergen == null)
                return NotFound();

            _context.Allergens.Remove(allergen);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}