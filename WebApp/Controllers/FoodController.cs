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

        private void WriteLog(string level, string message)
        {
            _context.Logs.Add(new Log
            {
                Level = level,
                Message = message
            });
            _context.SaveChanges();
        }

        public IActionResult Index(string? title = null, int? categoryId = null, int page = 1, int count = 10)
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
                .ToList();

            ViewBag.SearchTitle = title;
            ViewBag.CategoryId = categoryId;
            ViewBag.Page = page;
            ViewBag.Count = count;
            ViewBag.Total = total;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / count);
            ViewBag.Categories = new SelectList(_context.FoodCategories, "Id", "Name", categoryId);

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

            WriteLog("INFO", $"Food '{food.Title}' with id={food.Id} created.");
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
            WriteLog("INFO", $"Food '{existing.Title}' with id={id} updated.");
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

            WriteLog("INFO", $"Food '{food.Title}' with id={id} deleted.");
            return RedirectToAction("Index");
        }
    }
}