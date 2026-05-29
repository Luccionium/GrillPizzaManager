using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly RwaProjektContext _context;

        public MenuController(RwaProjektContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId = null, string? title = null)
        {
            var categories = _context.FoodCategories.ToList();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchTitle = title;
            return View();
        }

        [Authorize]
        public IActionResult Cart()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult PlaceOrder([FromBody] PlaceOrderDto orderDto)
        {
            try
            {
                var username = HttpContext.User.Identity?.Name;
                var user = _context.Users.FirstOrDefault(x => x.Username == username);
                if (user == null)
                    return Unauthorized();

                if (string.IsNullOrEmpty(orderDto.DeliveryAddress))
                    return BadRequest("Delivery address is required");

                var order = new Order
                {
                    UserId = user.Id,
                    TotalPrice = orderDto.Items.Sum(x => x.Price * x.Quantity),
                    Note = orderDto.Note,
                    DeliveryAddress = orderDto.DeliveryAddress!
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                foreach (var item in orderDto.Items)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        FoodId = item.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });
                }

                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        public IActionResult OrderConfirmation()
        {
            return View();
        }

        [Authorize]
        public IActionResult MyOrders()
        {
            var username = HttpContext.User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user == null)
                return Unauthorized();

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        public IActionResult Details(int id)
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

        public IActionResult SearchPartial(int? categoryId = null, string? title = null, int page = 1, int count = 8)
        {
            var query = _context.Foods
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodAllergens)
                    .ThenInclude(fa => fa.Allergen)
                .Where(f => f.IsAvailable)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(f => f.FoodCategoryId == categoryId);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(f => f.Title.Contains(title));

            var total = query.Count();
            var foods = query
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / count);
            ViewBag.CategoryId = categoryId;
            ViewBag.SearchTitle = title;
            ViewBag.Count = count;

            return PartialView("_MenuPartial", foods);
        }
    }
}