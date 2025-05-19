using AgendamentoMedico.API.Models;
using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Infra.Data;
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
        private readonly AppDbContext _context;
        private readonly EncryptUtils _encryptUtils;

        public AuthController(ILogger<AuthController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
            _encryptUtils = new EncryptUtils();
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var senhaDecrypt = EncryptUtils.DecryptPasswordBase64(login.Senha);
            senhaDecrypt = EncryptUtils.DecryptPassword(senhaDecrypt);

            if (ModelState.IsValid)
            {
                try
                {
                    var func = await _context.Funcionarios
                        .Include(f => f.UsuarioFuncionarioId)
                        .ThenInclude(u => u.FuncionarioId)
                        .Where(f =>
                        f.UsuarioFuncionarioId.Senha == senhaDecrypt &&
                        f.UsuarioFuncionarioId.NomeUsuario == login.NomeUsuario)
                        .FirstOrDefaultAsync();

                    if (func == null)
                    {
                        ModelState.AddModelError(string.Empty, "O usuário não foi encontrado, por favor tente novamente");
                        return View(func);
                    }

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao realizar o login: " + ex.Message);
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel createVw)
        {


            return RedirectToAction("Index", "Relatorios");
        }
    }
}
