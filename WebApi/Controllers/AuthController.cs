using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.DTOs;
using WebApi.Models;
using WebApi.Security;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RwaProjektContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(RwaProjektContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register-admin")]
        public ActionResult RegisterAdmin([FromBody] UserRegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Dozvoli samo ako nema nijednog admina u bazi
                var adminExists = _context.Users.Any(x => x.RoleId == 1);
                if (adminExists)
                    return BadRequest("Admin already exists");

                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(dto.Password!, salt);

                var user = new User
                {
                    Username = dto.Username!,
                    FirstName = dto.FirstName!,
                    LastName = dto.LastName!,
                    Email = dto.Email!,
                    Phone = dto.Phone,
                    PwdHash = hash,
                    PwdSalt = salt,
                    RoleId = 1 // Admin uloga
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok("Admin registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Provjeri postoji li već korisnik s tim usernameom
                var existingUser = _context.Users
                    .FirstOrDefault(x => x.Username == dto.Username);
                if (existingUser != null)
                    return BadRequest("Username already exists");

                // Hashiraj lozinku
                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(dto.Password!, salt);

                // Kreiraj novog korisnika
                var user = new User
                {
                    Username = dto.Username!,
                    FirstName = dto.FirstName!,
                    LastName = dto.LastName!,
                    Email = dto.Email!,
                    Phone = dto.Phone,
                    PwdHash = hash,
                    PwdSalt = salt,
                    RoleId = 2 // User uloga
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] UserLoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Pronađi korisnika
                var existingUser = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(x => x.Username == dto.Username);
                if (existingUser == null)
                    return Unauthorized("Invalid username or password");

                // Provjeri lozinku
                var hash = PasswordHashProvider.GetHash(dto.Password!, existingUser.PwdSalt);
                if (hash != existingUser.PwdHash)
                    return Unauthorized("Invalid username or password");

                // Kreiraj JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, existingUser.Username),
                        new Claim(ClaimTypes.Role, existingUser.Role.Name)
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}