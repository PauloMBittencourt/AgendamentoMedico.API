using Microsoft.AspNetCore.Mvc;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Services.Services.Concrete;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using AgendamentoMedico.Domain.Entities;

namespace AgendamentoMedico.API.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly IFuncionarioService _funcionarioService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICargosService _cargosService;
        private readonly IHorarioDisponivelService _horarioService;
        private readonly INotyfService _toast;

        public FuncionariosController(IFuncionarioService funcionarioService, IUsuarioService usuarioService, ICargosService cargosService, IHorarioDisponivelService horarioService, INotyfService toast)
        {
            _funcionarioService = funcionarioService;
            _usuarioService = usuarioService;
            _cargosService = cargosService;
            _horarioService = horarioService;
            _toast = toast;
        }

        public async Task<IActionResult> Index()
        {
            var listaVm = await _funcionarioService.ObterTodosAsync();
            
            return View(listaVm);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await _funcionarioService.ObterPorIdAsync(id);
            if (vm is null) return NotFound();
            return View(vm);
        }

        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_usuarioService.ObterTodosAsync().Result, "Id", "NomeUsuario");
            return View("StepUsuario");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var vm = await _funcionarioService.ObterPorIdAsync(id);
            if (vm is null) return NotFound();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FuncionarioViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);
            await _funcionarioService.AtualizarAsync(id, vm);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var vm = await _funcionarioService.ObterPorIdAsync(id);
            if (vm is null) return NotFound();
            return View(vm);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _funcionarioService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }

        #region Cadastro de Funcionarios
        [HttpGet]
        public IActionResult StepUsuario()
        {
            ViewData["Roles"] = new SelectList(_cargosService.ObterTodosCargosDescAsync(true), "Id", "Nome");
            return View(new UsuarioViewModel());
        }

        [HttpPost]
        public IActionResult StepUsuario(UsuarioViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            TempData["Usuario"] = JsonConvert.SerializeObject(vm);
            TempData.Keep("Usuario");

            return RedirectToAction(nameof(StepFuncionario));
        }

        [HttpGet]
        public IActionResult StepFuncionario()
        {
            var usuarioJson = TempData.Peek("Usuario") as string;
            if (usuarioJson == null) return RedirectToAction(nameof(StepUsuario));
            var usuarioVm = JsonConvert.DeserializeObject<UsuarioViewModel>(usuarioJson);
            return View(new FuncionarioCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> StepFuncionario(FuncionarioCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var usuarioVm = JsonConvert.DeserializeObject<UsuarioViewModel>(
                TempData.Peek("Usuario")!.ToString()!);

            await _funcionarioService.CriarAsync(usuarioVm, vm);

            return RedirectToAction(nameof(Finish));
        }

        public IActionResult Finish()
        {
            return RedirectToAction("Index");
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> GetMeusHorariosDisponiveis()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var medico = await _funcionarioService.ObterPorIdAsync(Guid.Parse(userIdString));

            if (medico == null)
            {
                _toast.Error("Funcionário não encontrado para o usuário logado.");
                return NotFound();
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Funcionario, FuncionarioViewModel>());

            var mapper = config.CreateMapper();

            var medicoMaped = mapper.Map<Funcionario>(medico);

            var lista = await _horarioService.ObterDisponiveisMedicoAsync(medicoMaped);

            return Json(lista);
        }

    }
}
