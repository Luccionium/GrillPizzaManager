using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Security;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly RwaProjektContext _context;

        public UserController(RwaProjektContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var loginVm = new LoginVM { ReturnUrl = returnUrl };
            return View(loginVm);
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVm)
        {
            if (!ModelState.IsValid)
                return View(loginVm);

            var existingUser = _context.Users
                .Include(u => u.Role)  // dodat ćemo using
                .FirstOrDefault(x => x.Username == loginVm.Username);

            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(loginVm);
            }

            var hash = PasswordHashProvider.GetHash(loginVm.Password!, existingUser.PwdSalt);
            if (hash != existingUser.PwdHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(loginVm);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.Username),
                new Claim(ClaimTypes.Role, existingUser.Role.Name)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            Task.Run(async () =>
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties)
            ).GetAwaiter().GetResult();

            if (loginVm.ReturnUrl != null)
                return LocalRedirect(loginVm.ReturnUrl);
            else if (existingUser.Role.Name == "Admin")
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM registerVm)
        {
            if (!ModelState.IsValid)
                return View(registerVm);

            var existingUser = _context.Users
                .FirstOrDefault(x => x.Username == registerVm.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(registerVm);
            }

            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(registerVm.Password!, salt);

            var user = new User
            {
                Username = registerVm.Username!,
                FirstName = registerVm.FirstName!,
                LastName = registerVm.LastName!,
                Email = registerVm.Email!,
                Phone = registerVm.Phone,
                PwdHash = hash,
                PwdSalt = salt,
                RoleId = 2
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            Task.Run(async () =>
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme)
            ).GetAwaiter().GetResult();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Forbidden()
        {
            return View();
        }
    }
}