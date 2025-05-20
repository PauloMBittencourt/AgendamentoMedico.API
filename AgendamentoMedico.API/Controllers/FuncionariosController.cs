using Microsoft.AspNetCore.Mvc;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Services.Services.Concrete;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgendamentoMedico.API.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly IFuncionarioService _funcionarioService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICargosService _cargosService;

        public FuncionariosController(IFuncionarioService funcionarioService, IUsuarioService usuarioService, ICargosService cargosService)
        {
            _funcionarioService = funcionarioService;
            _usuarioService = usuarioService;
            _cargosService = cargosService;
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
    }
}
