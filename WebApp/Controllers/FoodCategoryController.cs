using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FoodCategoryController : Controller
    {
        private readonly RwaProjektContext _context;

        public FoodCategoryController(RwaProjektContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.FoodCategories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(FoodCategory category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.FoodCategories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var category = _context.FoodCategories.FirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(int id, FoodCategory category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var existing = _context.FoodCategories.FirstOrDefault(x => x.Id == id);
            if (existing == null)
                return NotFound();

            existing.Name = category.Name;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var category = _context.FoodCategories.FirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.FoodCategories.FirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();

            _context.FoodCategories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}