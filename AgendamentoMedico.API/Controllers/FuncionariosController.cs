using Microsoft.AspNetCore.Mvc;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            return RedirectToAction("StepUsuario");
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
        public async Task<IActionResult> StepFuncionario()
        {
            var usuarioJson = TempData.Peek("Usuario") as string;
            if (usuarioJson == null)
                return RedirectToAction(nameof(StepUsuario));

            var cargos = await _cargosService.ObterTodosCargosDescAsync();
            var selectList = cargos
                .Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                })
                .ToList();

            var vm = new FuncionarioCreateViewModel
            {
                CargosDisponiveis = selectList
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> StepFuncionario(FuncionarioCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var cargos = await _cargosService.ObterTodosCargosDescAsync();
                ViewData["Roles"] = new SelectList(cargos, "Id", "Nome");

                return View(vm);
            }

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
                return RedirectToAction("Index", "Home");
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Funcionario, FuncionarioViewModel>());

            var mapper = config.CreateMapper();

            var medicoMaped = mapper.Map<Funcionario>(medico);

            var lista = await _horarioService.ObterDisponiveisMedicoAsync(medicoMaped);

            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarHorarioDisponivel([FromBody] HorarioDisponivelViewModel vm)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var medico = await _funcionarioService.ObterPorIdAsync(Guid.Parse(userIdString));

            if (medico == null)
            {
                _toast.Error("Médico não encontrado para o usuário logado.");
                return RedirectToAction("Index", "Home");
            }

            if (!DateTime.TryParse(vm.InicioConsulta, out var dataInicio))
                return BadRequest(new { error = "Formato de data/hora inválido." });

            var novoHorario = new HorarioDisponivel
            {
                HorarioDisponivelId = Guid.NewGuid(),
                FuncionarioId = medico.Id,
                DataHora = dataInicio,
                Disponivel = true
            };

            await _horarioService.CriarAsync(novoHorario);

            return Json(new
            {
                id = novoHorario.HorarioDisponivelId,
                start = novoHorario.DataHora.ToString("s"),
                end = novoHorario.DataHora.AddMinutes(30).ToString("s")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverHorarioDisponivel(Guid id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var medico = await _funcionarioService.ObterPorIdAsync(Guid.Parse(userIdString));

            if (medico == null)
                return NotFound(new { error = "Médico não encontrado para o usuário logado." });

            var slot = await _horarioService.ObterPorId(id);

            if (slot == null)
                return NotFound(new { error = "Horário não encontrado." });

            if (slot.FuncionarioId != medico.Id)
                return Forbid();

            try
            {
                await _horarioService.Remover(id);
            }
            catch (Exception)
            {
                _toast.Error("Ocorreu um erro ao tentar remover o agendamento");
                return BadRequest();
            }

            return Ok(new { sucesso = true });
        }
    }
}
