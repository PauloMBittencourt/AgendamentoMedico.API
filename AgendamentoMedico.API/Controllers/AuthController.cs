using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Concrete;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Utils.Encrypt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoMedico.API.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthServices _authServices;

        public AuthController(ILogger<AuthController> logger, IAuthServices authServices)
        {
            _logger = logger;
            _authServices = authServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var valid = await _authServices.ValidateCredentialsAsync(login);

            if (!valid)
            {
                ModelState.AddModelError(string.Empty,
                    "Usuário ou senha inválidos.");
                return View(login);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
