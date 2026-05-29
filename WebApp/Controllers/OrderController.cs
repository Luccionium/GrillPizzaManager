using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly RwaProjektContext _context;

        public OrderController(RwaProjektContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Food)
                .FirstOrDefault(x => x.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        public IActionResult Users()
        {
            var users = _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Food)
                .Where(u => u.RoleId == 2) // samo User uloga
                .ToList();

            return View(users);
        }
    }
}