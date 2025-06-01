using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgendamentoMedico.Domain.Entities;
using Newtonsoft.Json;
using AgendamentoMedico.Utils.Encrypt;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Interfaces;

namespace AgendamentoMedico.API.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IUsuarioService _usuarioService;

        public ClientesController(IClienteService clienteService, IUsuarioService usuarioService)
        {
            _clienteService = clienteService;
            _usuarioService = usuarioService;
        }



        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var vmList = await _clienteService.ObterTodosAsync();
            return View(vmList);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await _clienteService.ObterPorIdAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_usuarioService.ObterTodosAsync().Result, "Id", "NomeUsuario");
            return View("StepUsuario");
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

            return RedirectToAction(nameof(StepCliente));
        }

        [HttpGet]
        public IActionResult StepCliente()
        {
            var usuarioJson = TempData.Peek("Usuario") as string;
            if (usuarioJson == null) return RedirectToAction(nameof(StepUsuario));
            var usuarioVm = JsonConvert.DeserializeObject<UsuarioViewModel>(usuarioJson);
            return View(new ClienteViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> StepCliente(ClienteViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var usuarioVm = JsonConvert.DeserializeObject<UsuarioViewModel>(
                TempData.Peek("Usuario")!.ToString()!);

            await _clienteService.CriarAsync(usuarioVm, vm);

            return RedirectToAction(nameof(Finish));
        }

        public IActionResult Finish()
        {
            return RedirectToAction("Index");
        }
        #endregion

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteService.ObterPorIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_usuarioService.ObterTodosAsync().Result, "Id", "NomeUsuario", cliente.Id);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Email,Telefone")] Cliente cliente)
        {
            if (id != cliente.Id) return BadRequest();
            if (!ModelState.IsValid) return View(cliente);

            ClienteViewModel vm = new()
            {
                Nome = cliente.Nome,
                Telefone = cliente.Telefone
            };

            await _clienteService.AtualizarAsync(vm);
            ViewData["Id"] = new SelectList(_usuarioService.ObterTodosAsync().Result, "Id", "NomeUsuario", cliente.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteService.ObterPorIdAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _clienteService.ExcluirAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
