using AgendamentoMedico.Domain.Entities;
using AgendamentoMedico.Domain.Models;
using AgendamentoMedico.Services.Services.Interfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendamentoMedico.API.Controllers
{
    public class CargosController : Controller
    {
        private readonly ICargosService _service;
        private readonly INotyfService _toast;

        public CargosController(ICargosService service, INotyfService toast)
        {
            _service = service;
            _toast = toast;
        }



        // GET: Cargos
        public async Task<IActionResult> Index()
        {
            // Obtém todos os cargos via Service
            var listaCargos = await _service.ObterTodosCargos();
            return View(listaCargos);
        }

        // GET: Cargos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Como não existe método direto para buscar por ID, obtém toda a lista e filtra
            var todos = await _service.ObterTodosCargos();
            var cargo = todos.FirstOrDefault(c => c.Id == id.Value);

            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // GET: Cargos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cargos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var cargo = new IdentityRole()
                {
                    Id = Guid.NewGuid(),
                    Nome = vm.Nome,
                    Descricao = vm.Descricao
                };
                var sucesso = await _service.CargoSalvar(cargo);
                if (sucesso)
                {
                    return RedirectToAction(nameof(Index));
                }
                _toast.Error ("Não foi possível salvar o cargo.");
            }
            return View(vm);
        }

        // GET: Cargos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca o cargo para edição
            var todos = await _service.ObterTodosCargos();
            var cargo = todos.FirstOrDefault(c => c.Id == id.Value);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        // POST: Cargos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Descricao")] IdentityRole identityRole)
        {
            if (id != identityRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Como o Service não diferencia Create de Update, reaproveita CargoSalvar
                var sucesso = await _service.CargoSalvar(identityRole);
                if (sucesso)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Não foi possível atualizar o cargo.");
            }
            return View(identityRole);
        }

        // GET: Cargos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca o cargo para confirmação de exclusão
            var todos = await _service.ObterTodosCargos();
            var cargo = todos.FirstOrDefault(c => c.Id == id.Value);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // POST: Cargos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Procura o cargo antes de remover
            var todos = await _service.ObterTodosCargos();
            var cargo = todos.FirstOrDefault(c => c.Id == id);
            if (cargo != null)
            {
                var sucesso = await _service.CargoDelete(cargo);
                if (!sucesso)
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível remover o cargo.");
                    return View(cargo);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
