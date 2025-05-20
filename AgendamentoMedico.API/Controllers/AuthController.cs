using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Concrete;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Utils.Encrypt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AgendamentoMedico.API.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthServices _authServices;
        private readonly ICargosService _cargosService;
        private readonly IUsuarioService _usuarioService;

        public AuthController(ILogger<AuthController> logger, IAuthServices authServices, ICargosService cargosService, IUsuarioService usuarioService)
        {
            _logger = logger;
            _authServices = authServices;
            _cargosService = cargosService;
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            var usuarios = _usuarioService.ObterTodosAsync();

            var usuariosPerfis = new List<UsuarioRoleViewModel>();

            foreach (var item in usuarios.Result)
            {
                var perfisUsuario = _cargosService.ObterCargosUsuarioPorIdAsync(item.Id);

                usuariosPerfis.Add(new UsuarioRoleViewModel
                {
                    UsuarioId = item.Id,
                    Nome = item.NomeUsuario,
                    Roles = perfisUsuario
                });
            }

            ViewBag.Perfis = _cargosService.ObterTodosCargosDescAsync();

            var perfis = _cargosService.ObterTodosCargosDescAsync(true);

            return View(usuariosPerfis);
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

            var usuario = await _usuarioService
                .ObterUsuarioPorSenha(login.NomeUsuario, login.Senha);

            AutenticarIdentity(usuario, login.RememberMe);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Cargos(Guid id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            var cargoUsuario = _cargosService
                .ObterCargosUsuarioPorIdAsync(usuario.Id);

            var cargos = _cargosService
                .ObterTodosCargosDescAsync();

            var usuarioCargo = new UsuarioRoleViewModel
            {
                UsuarioId = usuario.Id,
                Nome = usuario.NomeUsuario,
                Roles = cargoUsuario
            };

            ViewBag.Cargos = cargos.Except(cargoUsuario).ToList();

            return View(usuarioCargo);
        }

        [HttpPost]
        public async Task<bool> Cargos(Guid usuarioId, string[] cargosUsuario)
        {
            try
            {
                var listaCargosUsuarios = _cargosService
                    .ObterCargosUsuariosPorId(usuarioId);

                var result = _cargosService.SalvarListaCargos_Usuario(listaCargosUsuarios);

                var cargosList = _cargosService.ObterTodosCargos().Result;

                for (int i = 0; i < cargosUsuario.Length; i++)
                {
                    var cargosId = cargosList
                        .Where(p => p.Descricao == cargosUsuario[i])
                        .Select(p => p.Id)
                        .First();

                    var perfisFuncionarios = new IdentityRole_Usuario
                    {
                        UsuarioId = usuarioId,
                        CargosIdentityId = cargosId,
                    };

                    var resultFn = await _cargosService.SalvarCargos_Usuario(perfisFuncionarios);

                    if (resultFn == false)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Erro ao salvar cargos do usuário.");
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                ModelState.AddModelError(string.Empty,
                            "Um erro ocorreu durante a execução. Por favor tente novamente ou contate o administrador");
                return false;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePerfilUsuario(Guid usuarioId, string Cargos, bool active)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);

            if (usuario != null)
            {
                if (active)
                {
                    AddToPerfil(usuarioId, Cargos);
                }
                else
                {
                    RemoveFromPerfil(usuarioId, Cargos);
                }
            }

            return Json("ok");
        }

        #region Metódos Auxiliares
        private async void AutenticarIdentity(Usuario usuario, bool RememberMe)
        {
            try
            {
                List<Claim> claims = [
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, usuario.NomeUsuario)
                    ];

                if (!usuario.Cargos.IsNullOrEmpty())
                {
                    foreach (var claim in usuario.Cargos)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, claim.CargosIdentityFk.Nome));
                    }
                }

                var authScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                var identity = new ClaimsIdentity(claims, authScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(authScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = RememberMe
                    });
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void AddToPerfil(Guid UsuariosId, string Cargos)
        {
            try
            {
                var cargoFind = _cargosService.ObterCargoPorDesc(Cargos).Result;

                if (cargoFind != null)
                {
                    var cargo = new IdentityRole_Usuario
                    {
                        UsuarioId = UsuariosId,
                        CargosIdentityId = cargoFind.Id
                    };

                    _cargosService.SalvarCargos_Usuario(cargo);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void RemoveFromPerfil(Guid UsuariosId, string Cargos)
        {
            try
            {
                var cargoFind = _cargosService.ObterCargoPorDesc(Cargos).Result;

                if (cargoFind != null)
                {
                    var cargoUsuario = _cargosService
                        .ObterUsuarioComCargosPorId(UsuariosId, cargoFind.Id).Result;

                    if (cargoUsuario != null)
                    {
                        _cargosService.RemoverCargos_Usuario(cargoUsuario);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
