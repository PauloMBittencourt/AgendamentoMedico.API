using AgendamentoMedico.API.Models;
using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendamentoMedico.API.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _context;

        public AuthController(ILogger<AuthController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Funcionario func = _context.Funcionarios
                        .FirstOrDefault(f => f.Nome == login.NomeUsuario && f.Senha == login.Senha);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao realizar o login: " + ex.Message);
                }
                return View();
            }
        }
    }
}
