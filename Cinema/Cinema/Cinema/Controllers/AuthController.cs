using Cinema.Data;
using Cinema.Models;
using Cinema.Models.Enums;
using Cinema.Models.ViewModels;
using Cinema.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cinema.Controllers
{
    public class AuthController : Controller
    {
        private readonly CinemaContext _context;
        private readonly AuthService _authService;

        public AuthController(CinemaContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var person = await _context.Persons
                .FirstOrDefaultAsync(p => p.Email == model.Email);

            if (person == null || person.PasswordHash == null || !_authService.VerifyPassword(model.Password, person.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, person.FirstName ?? "Usuário"),
                new Claim(ClaimTypes.Email, person.Email!),
                new Claim(ClaimTypes.NameIdentifier, person.ID.ToString()),
                new Claim(ClaimTypes.Role, person.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirecionar baseado na Role
            if (person.Role == UserRole.Admin)
                return RedirectToAction("Index", "Home"); // Pode mudar para Dashboard Admin depois
            else if (person.Role == UserRole.Seller)
                return RedirectToAction("Index", "Ticket"); 
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Persons.AnyAsync(p => p.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Este email já está cadastrado.");
                return View(model);
            }

            var person = new Person
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                CPF = model.CPF,
                IsClient = true,
                Role = UserRole.Client,
                PasswordHash = _authService.HashPassword(model.Password)
            };

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            // Auto-login após registro
             var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, person.FirstName ?? "Usuário"),
                new Claim(ClaimTypes.Email, person.Email!),
                new Claim(ClaimTypes.NameIdentifier, person.ID.ToString()),
                new Claim(ClaimTypes.Role, person.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
