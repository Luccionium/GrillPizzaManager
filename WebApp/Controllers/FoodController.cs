using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FoodController : Controller
    {
        private readonly RwaProjektContext _context;

        public FoodController(RwaProjektContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var foods = _context.Foods
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodAllergens)
                    .ThenInclude(fa => fa.Allergen)
                .ToList();
            return View(foods);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.FoodCategories, "Id", "Name");
            ViewBag.Allergens = _context.Allergens.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Food food, int[] selectedAllergens)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.FoodCategories, "Id", "Name");
                ViewBag.Allergens = _context.Allergens.ToList();
                return View(food);
            }

            var existing = _context.Foods
                .FirstOrDefault(x => x.Title == food.Title);
            if (existing != null)
            {
                ModelState.AddModelError("Title", "Jelo s tim nazivom već postoji");
                ViewBag.Categories = new SelectList(_context.FoodCategories, "Id", "Name");
                ViewBag.Allergens = _context.Allergens.ToList();
                return View(food);
            }

            _context.Foods.Add(food);
            _context.SaveChanges();

            foreach (var allergenId in selectedAllergens)
            {
                _context.FoodAllergens.Add(new FoodAllergen
                {
                    FoodId = food.Id,
                    AllergenId = allergenId
                });
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {

            var food = _context.Foods
                .Include(f => f.FoodAllergens)
                .FirstOrDefault(x => x.Id == id);

            if (food == null)
                return NotFound();

            ViewBag.Categories = new SelectList(_context.FoodCategories, "Id", "Name", food.FoodCategoryId);
            ViewBag.Allergens = _context.Allergens.ToList();
            ViewBag.SelectedAllergens = food.FoodAllergens.Select(fa => fa.AllergenId).ToList();
            return View(food);
        }

        [HttpPost]
        public IActionResult Edit(int id, Food food, int[] selectedAllergens)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.FoodCategories, "Id", "Name");
                ViewBag.Allergens = _context.Allergens.ToList();
                ViewBag.SelectedAllergens = selectedAllergens.ToList();
                return View(food);
            }

            var duplicate = _context.Foods
                .FirstOrDefault(x => x.Title == food.Title && x.Id != id);
            if (duplicate != null)
            {
                ModelState.AddModelError("Title", "Jelo s tim nazivom već postoji");
                ViewBag.Categories = new SelectList(_context.FoodCategories, "Id", "Name");
                ViewBag.Allergens = _context.Allergens.ToList();
                ViewBag.SelectedAllergens = selectedAllergens.ToList();
                return View(food);
            }

            var existing = _context.Foods
                .Include(f => f.FoodAllergens)
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return NotFound();

            existing.Title = food.Title;
            existing.Description = food.Description;
            existing.Price = food.Price;
            existing.ImageUrl = food.ImageUrl;
            existing.IsAvailable = food.IsAvailable;
            existing.FoodCategoryId = food.FoodCategoryId;

            _context.FoodAllergens.RemoveRange(existing.FoodAllergens);
            foreach (var allergenId in selectedAllergens)
            {
                _context.FoodAllergens.Add(new FoodAllergen
                {
                    FoodId = id,
                    AllergenId = allergenId
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var food = _context.Foods
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodAllergens)
                    .ThenInclude(fa => fa.Allergen)
                .FirstOrDefault(x => x.Id == id);

            if (food == null)
                return NotFound();

            return View(food);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var food = _context.Foods
                .Include(f => f.FoodAllergens)
                .FirstOrDefault(x => x.Id == id);

            if (food == null)
                return NotFound();

            _context.FoodAllergens.RemoveRange(food.FoodAllergens);
            _context.Foods.Remove(food);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}