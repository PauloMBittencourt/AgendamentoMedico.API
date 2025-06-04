using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Interfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AgendamentoMedico.API.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly IFuncionarioService _funcionarioService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICargosService _cargosService;
        private readonly IHorarioDisponivelService _horarioService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly INotyfService _toast;

        public FuncionariosController(IFuncionarioService funcionarioService, IUsuarioService usuarioService, ICargosService cargosService, IHorarioDisponivelService horarioService, IAgendamentoService agendamentoService, INotyfService toast)
        {
            _funcionarioService = funcionarioService;
            _usuarioService = usuarioService;
            _cargosService = cargosService;
            _horarioService = horarioService;
            _agendamentoService = agendamentoService;
            _toast = toast;
        }

        public async Task<IActionResult> Index()
        {
            var listaVm = await _funcionarioService.ObterTodosAsync();

            return View(listaVm);
        }

        protected async Task<FuncionarioViewModel> ObterFuncionarioLogadoAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var usuarioId))
                return null;

            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);
            if (usuario == null || usuario.FuncionarioId == null)
                return null;

            var funcionario = await _funcionarioService.ObterPorIdAsync(usuario.FuncionarioId.Id);
            return funcionario;
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

            var usuarioVm = JsonConvert.DeserializeObject<UsuarioViewModel>(usuarioJson);

            var listaDeCargos = await _cargosService.ObterTodosCargosDescAsync();

            var selectList = listaDeCargos
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
            if (!ModelState.IsValid && vm.CargosDisponiveis != null)
            {
                var listaDeCargos = await _cargosService.ObterTodosCargosDescAsync();
                vm.CargosDisponiveis = listaDeCargos
                    .Select(c => new SelectListItem
                    {
                        Value = c,
                        Text = c
                    })
                    .ToList();

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

        public async Task<IActionResult> GetMeusAgendamentos()
        {
            var medico = await ObterFuncionarioLogadoAsync();

            if (medico == null)
            {
                _toast.Error("Médico não encontrado para o usuário logado.");
                return RedirectToAction("Index", "Home");
            }

            var disponiveis = _horarioService.ObterDisponiveisMedico(medico.Id);
            Guid funcionarioId = disponiveis.FirstOrDefault()?.FuncionarioId ?? Guid.Empty;
            if (funcionarioId == Guid.Empty)
                return NotFound(new { error = "Funcionário não encontrado." });

            var agendamentos = _agendamentoService.ObterAgendamentosDoMedico(medico.Id);

            var listaJson = agendamentos.ConvertAll(a => new
            {
                id = Guid.Parse(a.DataHora + funcionarioId.ToString()),
                dataHora = a.DataHora,
                pacienteNome = a.NomePaciente,
                status = a.Status
            });

            return Json(agendamentos);
        }

        [HttpGet]
        public async Task<IActionResult> GetMeusHorariosDisponiveis()
        {

            var medico = await ObterFuncionarioLogadoAsync();

            if (medico == null)
            {
                _toast.Error("Médico não encontrado para o usuário logado.");
                return RedirectToAction("Index", "Home");
            }

            Guid funcionarioId = _horarioService
                .ObterDisponiveisMedico(medico.Id) 
                .FirstOrDefault()?.FuncionarioId ?? Guid.Empty;

            if (funcionarioId == Guid.Empty)
                return NotFound(new { error = "Funcionário não encontrado." });

            var disponiveis = _horarioService.ObterDisponiveisMedico(funcionarioId);

            var listaJson = disponiveis.ConvertAll(h => new
            {
                id = h.HorarioDisponivelId,
                start = h.DataHora.ToString("s"),
                end = h.DataHora.AddMinutes(30).ToString("s")
            });

            return Json(listaJson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarHorarioDisponivel([FromBody] HorarioDisponivelViewModel vm)
        {
            var medico = await ObterFuncionarioLogadoAsync();

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

            var medico = await ObterFuncionarioLogadoAsync();

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
