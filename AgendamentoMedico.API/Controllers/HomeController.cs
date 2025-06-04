using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Enuns;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Concrete;
using AgendamentoMedico.Services.Services.Interfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AgendamentoMedico.API.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUsuarioService _usuarioService;
    private readonly IHorarioDisponivelService _horarioService;
    private readonly IAgendamentoService _agendamentoService;
    private readonly IClienteService _clienteService;
    private readonly INotyfService _toast;

    public HomeController(ILogger<HomeController> logger, IUsuarioService usuarioService, IHorarioDisponivelService horarioService, IAgendamentoService agendamentoService, IClienteService clienteService, INotyfService toast)
    {
        _logger = logger;
        _usuarioService = usuarioService;
        _horarioService = horarioService;
        _agendamentoService = agendamentoService;
        _clienteService = clienteService;
        _toast = toast;
    }

    protected async Task<Cliente> ObterClienteLogadoAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var usuarioId))
            return null;

        var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);
        if (usuario == null || usuario.ClienteId == null)
            return null;

        var cliente = await _clienteService.ObterPorIdAsync(usuario.ClienteId.Id);
        return cliente;
    }

    public async Task<IActionResult> Index()
    {
        var usuario = await _usuarioService.ObterPorNomeAsync(User.Identity.Name!);
        if (usuario == null)
            return RedirectToAction("Login", "Auth");

        if ((usuario.FuncionarioId != null && usuario.Cargos.Where(c => c.CargosIdentityFk.Nome == "Medico").Any()) 
            || usuario.Cargos.Where(c => c.CargosIdentityFk.Nome == "Administrador").Any())
        {
            return View("MedicoDashboard", await _horarioService.ObterTodosPorFuncionarioAsync(usuario.Id));
        }

        return View("ClienteDashboard");
    }

    [Authorize(Roles = "Medico,Administrador")]
    [HttpPost]
    public async Task<IActionResult> AddHorario(DateTime dataHora)
    {
        var usuario = await _usuarioService.ObterPorNomeAsync(User.Identity.Name!);
        if (usuario == null) return Unauthorized();

        var hd = new HorarioDisponivel
        {
            HorarioDisponivelId = Guid.NewGuid(),
            FuncionarioId = usuario.Id,
            DataHora = dataHora,
            Disponivel = true
        };
        await _horarioService.CriarAsync(hd);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet]
    public async Task<JsonResult> GetHorariosDisponiveis()
    {
        var list = await _horarioService.ObterDisponiveisAsync();

        var events = list
            .Select(h => new {
                id = h.HorarioDisponivelId,
                title = "Disponível",
                start = h.DataHora.ToString("s"),
                allDay = false
            })
            .ToList();

        return Json(events);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost]
    public async Task<IActionResult> Agendar([FromBody] AgendarViewModel agendar)
    {
        var usuario = await _usuarioService.ObterPorNomeAsync(User.Identity.Name!);
        if (usuario == null) return Unauthorized();

        var cliente = await ObterClienteLogadoAsync();

        try
        {
            await _agendamentoService.AgendarConsultaAsync(agendar.horarioId, cliente.Id);
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            _toast.Error("Erro ai cadastrar agendamento, favor tentar novamente.");
            throw;
        }
    }

    [HttpGet]
    [Authorize(Roles = "Cliente")]
    public async Task<JsonResult> GetMeusAgendamentos()
    {
        var usuarioLogado = await _usuarioService.ObterPorNomeAsync(User.Identity.Name!);
        if (usuarioLogado == null)
            return Json(new object[] { });

        var agendList = _agendamentoService.ObterAgendamentosDoPaciente(usuarioLogado.Id);
        var json = agendList.Select(a => new {
            dataHora = a.DataHora,
            nomeMedico = a.NomeMedico,
            status = a.Status
        }).ToList();
        return Json(json);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
