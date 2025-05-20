using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Interfaces;
using AgendamentoMedico.Domain.Enuns;
using AgendamentoMedico.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace AgendamentoMedico.API.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUsuarioService _usuarioService;
    private readonly IHorarioDisponivelService _horarioService;
    private readonly IAgendamentoService _agendamentoService;

    public HomeController(ILogger<HomeController> logger, IUsuarioService usuarioSvc, IHorarioDisponivelService horarioSvc, IAgendamentoService agendamentoSvc)
    {
        _logger = logger;
        _usuarioService = usuarioSvc;
        _horarioService = horarioSvc;
        _agendamentoService = agendamentoSvc;
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
    public async Task<IActionResult> Agendar(Guid horarioId)
    {
        var usuario = await _usuarioService.ObterPorNomeAsync(User.Identity.Name!);
        if (usuario == null) return Unauthorized();

        await _agendamentoService.MarkAsync(horarioId, usuario.Id);
        return Ok();
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
